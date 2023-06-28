using JokeApiV2.Enums;
using JokeApiV2.Models;
using JokeApiV2.Services.Decorator;
using Newtonsoft.Json;

namespace JokeApiV2.Services
{
    public class ExternalJokeService : IExternalJokeService
    {
        private readonly HttpClient _httpClient;

        private readonly IHighlightingDecorator _highlightingDecorator;

        public ExternalJokeService(HttpClient httpClient, IHighlightingDecorator highlightingDecorator)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _highlightingDecorator = highlightingDecorator;

        }

        public async Task<Joke> GetRandomJoke()
        {
            string url = "https://icanhazdadjoke.com";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve a random joke from the external API.");
            }

            var jokeResponse = await DeserializeJokeResponse(response);
            return MapToJoke(jokeResponse);
        }

        public async Task<Dictionary<JokeLengthCategory, List<Joke>>> SearchJokes(string searchTerm, int limit = 30)
        {

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException("Search term cannot be null or empty.", nameof(searchTerm));
            }

            string url = $"https://icanhazdadjoke.com/search?search_term={searchTerm}&limit={limit}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve jokes from the external API.");
            }

            var searchResponse = await DeserializeJokeSearchResponse(response);

            
            return MapToCategorizedJokes(searchResponse.Results, searchTerm);
        }

        private async Task<JokeResponse> DeserializeJokeResponse(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JokeResponse>(content);
        }

        private async Task<JokeSearchResponse> DeserializeJokeSearchResponse(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JokeSearchResponse>(content);
        }

        private Joke MapToJoke(JokeResponse jokeResponse, string searchTerm)
        {
            return new Joke
            {
                Id = jokeResponse.Id,
                Text = _highlightingDecorator.Decorate(jokeResponse.Joke, searchTerm),
                Length = jokeResponse.Joke.Length
            };
        }

        private Joke MapToJoke(JokeResponse jokeResponse)
        {
            return new Joke
            {
                Id = jokeResponse.Id,
                Text = jokeResponse.Joke,
                Length = jokeResponse.Joke.Length
            };
        }

        private Dictionary<JokeLengthCategory, List<Joke>> MapToCategorizedJokes(List<JokeResponse> jokeResponses, string searchTerm)
        {
            var categorizedJokes = new Dictionary<JokeLengthCategory, List<Joke>>();

            foreach (var jokeResponse in jokeResponses)
            {
                var joke = MapToJoke(jokeResponse, searchTerm);

                var jokeLengthCategory = GetJokeLengthCategory(joke.Length);

                if (!categorizedJokes.ContainsKey(jokeLengthCategory))
                {
                    categorizedJokes[jokeLengthCategory] = new List<Joke>();
                }

                categorizedJokes[jokeLengthCategory].Add(joke);
            }

            return categorizedJokes;
        }

        private JokeLengthCategory GetJokeLengthCategory(int length)
        {
            if (length < 10)
                return JokeLengthCategory.Short;
            else if (length < 20)
                return JokeLengthCategory.Medium;
            else
                return JokeLengthCategory.Long;
        }
    }
}
