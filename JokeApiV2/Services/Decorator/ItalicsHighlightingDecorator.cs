using JokeApiV2.Models;
using JokeApiV2.Services.Decorator;

namespace JokeApiV2.Services.Decorator
{
    public class ItalicsHighlightingDecorator : IHighlightingDecorator
    {
        public string Decorate(string jokeText, string searchTerm)
        {
            // Add italics decoration to the searched term in the joke text
            var decoratedText = jokeText.Replace(searchTerm, $"<i>{searchTerm}</i>");

            // Return the decorated text
            return decoratedText;
        }
    }
}