

using JokeApiV2.Models;

namespace JokeApiV2.Data
{
    public class DbInitializer
    {

        private readonly JokeDbContext _context;

        public DbInitializer(JokeDbContext context)
        {
            _context = context;
        }
       

        public static void Initialize(JokeDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Jokes.Any())
            {
                // Seed the database with initial data
                var fallbackJokes = JokeHelper.RetrieveFallbackJokesFromPropertiesFile();
                context.Jokes.AddRange(fallbackJokes);
                context.SaveChanges();
            }
        }
    }
}