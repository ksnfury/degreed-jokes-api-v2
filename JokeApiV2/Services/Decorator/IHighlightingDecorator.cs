using JokeApiV2.Models;

namespace JokeApiV2.Services.Decorator;

public interface IHighlightingDecorator
{
    string Decorate(string text, string searchTerm);
}