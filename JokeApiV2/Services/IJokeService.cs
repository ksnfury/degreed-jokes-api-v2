
using JokeApiV2.Models;
using JokeApiV2.Enums;



namespace JokeApiV2.Services
{
    public interface IJokeService
    {
        Joke GetRandomJoke();
        Dictionary<JokeLengthCategory, List<Joke>> SearchJokes(string searchTerm, int limit = 30);
    }
}

