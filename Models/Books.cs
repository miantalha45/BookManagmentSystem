using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookManagmentSystem.Models
{
    public class Books
    {
        [JsonIgnore]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public int PublicationYear { get; set; }
        [Required]
        public string AuthorName { get; set; } = null!;
        [JsonIgnore]
        public int ViewsCount { get; set; }
        [NotMapped]
        [JsonIgnore]
        public double PopularityCount { get; set; }
    }
}
