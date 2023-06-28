using JokeApiV2.Enums;
using JokeApiV2.Models;

namespace JokeApiV2.Services
{
    public interface IExternalJokeService
    {
        Task<Joke> GetRandomJoke();
        Task<Dictionary<JokeLengthCategory, List<Joke>>> SearchJokes(string searchTerm, int limit = 30);
    }
}
