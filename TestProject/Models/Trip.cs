using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TestProject.Controllers;

namespace TestProject.Models
{
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Trip
    {
        [Key]
        public int Id { get; set; }

        public string? DriversId { get; set; } = string.Empty;

        [ForeignKey(nameof(DriversId))]
        public ApplicationUser? Driver { get; set; }

        [Required(ErrorMessage = "Началната позиция е задължителна")]
        public string StartPosition { get; set; }

        [Required(ErrorMessage = "Дестинацията е задължителна")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Часът на заминаване е задължителен")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Часът на връщане е задължителен")]
        public DateTime ReturnTime { get; set; }

        [Required(ErrorMessage = "Цената е задължителна")]
        [Range(1, int.MaxValue, ErrorMessage = "Цената трябва да е положително число")]
       
        public int Price { get; set; }

        [Required(ErrorMessage = "Общият брой места е задължителен")]
        [Range(1, int.MaxValue, ErrorMessage = "Броят на местата трябва да е положително число")]

        public int TotalSeats { get; set; }

        public int FreeSeats { get; set; }

        [Required(ErrorMessage = "Моделът на автомобила е задължителен")]
        public string CarModel { get; set; }

        [Required(ErrorMessage = "Регистрационният номер е задължителен")]
        public string PlateNumber { get; set; }

        [Url(ErrorMessage = "Невалиден URL за изображение")]
        public string? ImagePath { get; set; }

        public TripStatus StatusTrip { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime tripSchedule { get; set; } = DateTime.UtcNow;
        public DateTime NextStart { get; set; } = DateTime.UtcNow;

        public bool IsRecurring { get; set; }

        public String? RecurrenceInterval { get; set; } = "00:00:00";

        public DateTime? NextRunDate { get; set; } = DateTime.UtcNow;

        public ICollection<TripParticipant>? TripParticipants { get; set; }
        public ICollection<Request>? Requests { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
    }

}
