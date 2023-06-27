using System.ComponentModel.DataAnnotations;

namespace JokeApiV2.Models
{
    public class Joke
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
        public int Length { get; set; }
    }
}