using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace TestProject.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } 

        public string UserId { get; set; } 
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Range(1,5, ErrorMessage =("Стойността трябва да е от 1 до 5"))]

        public int Score { get; set; }

        public string? Comment { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

    }
}
