using JokeApiV2.Models;
using JokeApiV2.Enums;

namespace JokeApiV2.Services.Cache
{
    public interface IJokeCache
    {
        Dictionary<JokeLengthCategory, List<Joke>> Get(string key);

        void Set(string key, Dictionary<JokeLengthCategory, List<Joke>> value);
    }
}