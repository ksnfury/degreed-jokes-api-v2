using System;
using Newtonsoft.Json;

namespace JokeApiV2.Models
{
    public class JokeResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("joke")]
        public string Joke { get; set; }
    }

}

