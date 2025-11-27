using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TestProject.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        public int TripId { get; set; }
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; } 

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } 

        public RequestStatus StatusRequest { get; set; } 

        public DateTime Date { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Броят места трябва да бъде поне 1")]
        public int NumberOfSeats { get; set; } = 1; 
    }
}
