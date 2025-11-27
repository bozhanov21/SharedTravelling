using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using TestProject.Data;
using TestProject.Models;
using static System.Formats.Asn1.AsnWriter;

namespace TestProject.Services
{
    public class TripSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TripSchedulerService> _logger;

        public TripSchedulerService(IServiceProvider services, ILogger<TripSchedulerService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for recurring trips at {Time}", DateTime.UtcNow);

                using (var scope = _services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var Now = DateTime.UtcNow;

                    var allRecurringTrips = await context.Trips
                     .Where(t => t.IsRecurring && t.RecurrenceInterval != "00:00:00" && t.NextRunDate.HasValue)
                     .ToListAsync(); 

                    var recurringTrips = allRecurringTrips
                        .Where(t => TimeSpan.TryParse(t.RecurrenceInterval, out TimeSpan interval)
                         && t.tripSchedule.Add(interval) <= Now)
                        .ToList();

                    foreach (var trip in recurringTrips)
                    {
                        _logger.LogInformation("Processing recurring trip {TripId}", trip.Id);

                        var newDepartureTime = trip.NextStart;
                        TimeSpan reccurenceInterval = TimeSpan.Parse(trip.RecurrenceInterval);

                        if (await UserHasNoOverlappingTrips(trip))
                        {
                            var newTrip = new Trip
                            {
                                Driver = trip.Driver,
                                DriversId = trip.DriversId,
                                StartPosition = trip.StartPosition,
                                Destination = trip.Destination,
                                DepartureTime = newDepartureTime,
                                ReturnTime = newDepartureTime.Add(trip.ReturnTime - trip.DepartureTime),
                                Price = trip.Price,
                                TotalSeats = trip.TotalSeats,
                                FreeSeats = trip.TotalSeats,
                                CarModel = trip.CarModel,
                                PlateNumber = trip.PlateNumber,
                                ImagePath = trip.ImagePath,
                                StatusTrip = TripStatus.Upcoming,
                                CreatedDate = Now,
                                IsRecurring = false 
                            };
                            context.Trips.Add(newTrip);
                        }

                        trip.NextStart = trip.NextStart.Add(reccurenceInterval);
                        trip.tripSchedule = trip.tripSchedule.Add(reccurenceInterval);

                        await context.SaveChangesAsync();
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        private async Task<bool> UserHasNoOverlappingTrips(Trip trip)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var overlappingTrips = await context.Trips
                    .Where(t => t.DriversId == trip.DriversId &&
                                (t.StatusTrip == TripStatus.Upcoming || t.StatusTrip == TripStatus.Ongoing))
                    .ToListAsync(); 

                var NewDepartureTime = trip.NextStart;
                var NewReturnTime = NewDepartureTime.Add(trip.ReturnTime - trip.DepartureTime);

                bool hasOverlap = overlappingTrips.Any(t =>
                    (NewDepartureTime >= t.DepartureTime && NewDepartureTime <= t.ReturnTime) || 
                    (NewReturnTime >= t.DepartureTime && NewReturnTime <= t.ReturnTime) || 
                    (NewDepartureTime <= t.DepartureTime && NewReturnTime >= t.ReturnTime) 
                );

                return !hasOverlap; 
            }
        }

    }
}

