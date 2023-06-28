using System;
using Newtonsoft.Json;

namespace JokeApiV2.Models
{
    public class JokeSearchResponse
    {
        [JsonProperty("results")]
        public List<JokeResponse> Results { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }
    }
}

