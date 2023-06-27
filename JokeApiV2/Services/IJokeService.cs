using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

