using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Data
{
    public static class ApplicationDbSeed
    {
        public static async Task SeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            string[] roleNames = { "Admin", "Driver", "Tourist" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user if it doesn't exist
            var adminUser = new ApplicationUser
            {
                UserName = "Админ",
                Email = "admin@admin.com",
                FirstName = "Админ",
                LastName = "Админов",
                Position = "Driver",
                PhoneNumber = "+359878123456",
                DateOfDriverAcceptance = DateTime.UtcNow.AddYears(-1),
                ImagePath = "/images/drivers/default-image-Driver.jpg",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "админ123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "Driver");
                }
                  else
    {
        Console.WriteLine("Failed to create admin user:");
        foreach (var error in result.Errors)
        {
            Console.WriteLine($" - {error.Code}: {error.Description}");
        }
    }
            }

            // Create regular users if they don't exist
            var users = new List<(ApplicationUser User, string Password, string Role)>
            {
                // Drivers
                (new ApplicationUser { UserName = "ИванПетров", Email = "ivan.petrov@example.com", FirstName = "Иван", LastName = "Петров", Position = "Driver", PhoneNumber = "+359878234567", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-6), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "иванпетров", "Driver"),
                (new ApplicationUser { UserName = "МарияИванова", Email = "maria.ivanova@example.com", FirstName = "Мария", LastName = "Иванова", Position = "Driver", PhoneNumber = "+359878345678", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-8), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "марияиванова", "Driver"),
                (new ApplicationUser { UserName = "ГеоргиДимитров", Email = "georgi.dimitrov@example.com", FirstName = "Георги", LastName = "Димитров", Position = "Driver", PhoneNumber = "+359878456789", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-4), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "георгидимитров", "Driver"),
                (new ApplicationUser { UserName = "ПетърСтоянов", Email = "petar.stoyanov@example.com", FirstName = "Петър", LastName = "Стоянов", Position = "Driver", PhoneNumber = "+359878567890", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-10), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "петърстоянов", "Driver"),
                (new ApplicationUser { UserName = "ДимитърАнгелов", Email = "dimitar.angelov@example.com", FirstName = "Димитър", LastName = "Ангелов", Position = "Driver", PhoneNumber = "+359878678901", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-7), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "димитърангелов", "Driver"),
                (new ApplicationUser { UserName = "НиколайКолев", Email = "nikolay.kolev@example.com", FirstName = "Николай", LastName = "Колев", Position = "Driver", PhoneNumber = "+359878789012", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-5), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "николайколев", "Driver"),
                (new ApplicationUser { UserName = "ВикторияГеоргиева", Email = "viktoriya.georgieva@example.com", FirstName = "Виктория", LastName = "Георгиева", Position = "Driver", PhoneNumber = "+359878890123", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-3), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "викториягеоргиева", "Driver"),
                (new ApplicationUser { UserName = "БориславИванов", Email = "borislav.ivanov@example.com", FirstName = "Борислав", LastName = "Иванов", Position = "Driver", PhoneNumber = "+359878901234", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-9), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "бориславиванов", "Driver"),
                (new ApplicationUser { UserName = "КалинаПетрова", Email = "kalina.petrova@example.com", FirstName = "Калина", LastName = "Петрова", Position = "Driver", PhoneNumber = "+359879012345", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-2), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "калинапетрова", "Driver"),
                (new ApplicationUser { UserName = "СтоянТодоров", Email = "stoyan.todorov@example.com", FirstName = "Стоян", LastName = "Тодоров", Position = "Driver", PhoneNumber = "+359879123456", DateOfDriverAcceptance = DateTime.UtcNow.AddMonths(-11), ImagePath = "/images/drivers/default-image-Driver.jpg" }, "стоянтодоров", "Driver"),
                
                // Tourists
                (new ApplicationUser { UserName = "ЕленаМаркова", Email = "elena.markova@example.com", FirstName = "Елена", LastName = "Маркова", Position = "Tourist", PhoneNumber = "+359879234567" }, "еленамаркова", "Tourist"),
                (new ApplicationUser { UserName = "БоянПетров", Email = "boyan.petrov@example.com", FirstName = "Боян", LastName = "Петров", Position = "Tourist", PhoneNumber = "+359879345678" }, "боянпетров", "Tourist"),
                (new ApplicationUser { UserName = "СилвияНиколова", Email = "silviya.nikolova@example.com", FirstName = "Силвия", LastName = "Николова", Position = "Tourist", PhoneNumber = "+359879456789" }, "силвияниколова", "Tourist"),
                (new ApplicationUser { UserName = "ЛюбомирИлиев", Email = "lyubomir.iliev@example.com", FirstName = "Любомир", LastName = "Илиев", Position = "Tourist", PhoneNumber = "+359879567890" }, "любомирилиев", "Tourist"),
                (new ApplicationUser { UserName = "ДесиславаПопова", Email = "desislava.popova@example.com", FirstName = "Десислава", LastName = "Попова", Position = "Tourist", PhoneNumber = "+359879678901" }, "десиславапопова", "Tourist"),
                (new ApplicationUser { UserName = "КристиянВасилев", Email = "kristiyan.vasilev@example.com", FirstName = "Кристиян", LastName = "Василев", Position = "Tourist", PhoneNumber = "+359879789012" }, "кристиянвасилев", "Tourist"),
                (new ApplicationUser { UserName = "РадостинаИванова", Email = "radostina.ivanova@example.com", FirstName = "Радостина", LastName = "Иванова", Position = "Tourist", PhoneNumber = "+359879890123" }, "радостинаиванова", "Tourist"),
                (new ApplicationUser { UserName = "ВладимирПетков", Email = "vladimir.petkov@example.com", FirstName = "Владимир", LastName = "Петков", Position = "Tourist", PhoneNumber = "+359879901234" }, "владимирпетков", "Tourist"),
                (new ApplicationUser { UserName = "ГабриелаСтоянова", Email = "gabriela.stoyanova@example.com", FirstName = "Габриела", LastName = "Стоянова", Position = "Tourist", PhoneNumber = "+359880012345" }, "габриеластоянова", "Tourist"),
                (new ApplicationUser { UserName = "ЕмилДимитров", Email = "emil.dimitrov@example.com", FirstName = "Емил", LastName = "Димитров", Position = "Tourist", PhoneNumber = "+359880123456" }, "емилдимитров", "Tourist"),
                (new ApplicationUser { UserName = "ЯнаКостова", Email = "yana.kostova@example.com", FirstName = "Яна", LastName = "Костова", Position = "Tourist", PhoneNumber = "+359880234567" }, "янакостова", "Tourist"),
                (new ApplicationUser { UserName = "МартинГеоргиев", Email = "martin.georgiev@example.com", FirstName = "Мартин", LastName = "Георгиев", Position = "Tourist", PhoneNumber = "+359880345678" }, "мартингеоргиев", "Tourist"),
                (new ApplicationUser { UserName = "НадеждаИлиева", Email = "nadezhda.ilieva@example.com", FirstName = "Надежда", LastName = "Илиева", Position = "Tourist", PhoneNumber = "+359880456789" }, "надеждаилиева", "Tourist"),
                (new ApplicationUser { UserName = "ИвайлоПетров", Email = "ivaylo.petrov@example.com", FirstName = "Ивайло", LastName = "Петров", Position = "Tourist", PhoneNumber = "+359880567890" }, "ивайлопетров", "Tourist"),
                (new ApplicationUser { UserName = "ТеодораМаринова", Email = "teodora.marinova@example.com", FirstName = "Теодора", LastName = "Маринова", Position = "Tourist", PhoneNumber = "+359880678901" }, "теодорамаринова", "Tourist"),
                (new ApplicationUser { UserName = "АлександърНиколов", Email = "aleksandar.nikolov@example.com", FirstName = "Александър", LastName = "Николов", Position = "Tourist", PhoneNumber = "+359880789012" }, "александърниколов", "Tourist"),
                (new ApplicationUser { UserName = "МилаДимитрова", Email = "mila.dimitrova@example.com", FirstName = "Мила", LastName = "Димитрова", Position = "Tourist", PhoneNumber = "+359880890123" }, "миладимитрова", "Tourist"),
                (new ApplicationUser { UserName = "БорисИванов", Email = "boris.ivanov@example.com", FirstName = "Борис", LastName = "Иванов", Position = "Tourist", PhoneNumber = "+359880901234" }, "борисиванов", "Tourist"),
                (new ApplicationUser { UserName = "ЛилияПетрова", Email = "liliya.petrova@example.com", FirstName = "Лилия", LastName = "Петрова", Position = "Tourist", PhoneNumber = "+359881012345" }, "лилияпетрова", "Tourist"),
                (new ApplicationUser { UserName = "ВасилГеоргиев", Email = "vasil.georgiev@example.com", FirstName = "Васил", LastName = "Георгиев", Position = "Tourist", PhoneNumber = "+359881123456" }, "василгеоргиев", "Tourist"),
                (new ApplicationUser { UserName = "ЦветелинаСтоянова", Email = "tsvetelina.stoyanova@example.com", FirstName = "Цветелина", LastName = "Стоянова", Position = "Tourist", PhoneNumber = "+359881234567" }, "цветелинастоянова", "Tourist"),
                (new ApplicationUser { UserName = "ДаниелИванов", Email = "daniel.ivanov@example.com", FirstName = "Даниел", LastName = "Иванов", Position = "Tourist", PhoneNumber = "+359881345678" }, "даниеливанов", "Tourist"),
                (new ApplicationUser { UserName = "АнтонияПетрова", Email = "antoniya.petrova@example.com", FirstName = "Антония", LastName = "Петрова", Position = "Tourist", PhoneNumber = "+359881456789" }, "антонияпетрова", "Tourist"),
                (new ApplicationUser { UserName = "ХристоДимитров", Email = "hristo.dimitrov@example.com", FirstName = "Христо", LastName = "Димитров", Position = "Tourist", PhoneNumber = "+359881567890" }, "христодимитров", "Tourist"),
                (new ApplicationUser { UserName = "ЕкатеринаИванова", Email = "ekaterina.ivanova@example.com", FirstName = "Екатерина", LastName = "Иванова", Position = "Tourist", PhoneNumber = "+359881678901" }, "екатеринаиванова", "Tourist"),
                (new ApplicationUser { UserName = "ПламенГеоргиев", Email = "plamen.georgiev@example.com", FirstName = "Пламен", LastName = "Георгиев", Position = "Tourist", PhoneNumber = "+359881789012" }, "пламенгеоргиев", "Tourist"),
                (new ApplicationUser { UserName = "ЙорданкаПетрова", Email = "yordanka.petrova@example.com", FirstName = "Йорданка", LastName = "Петрова", Position = "Tourist", PhoneNumber = "+359881890123" }, "йорданкапетрова", "Tourist"),
                (new ApplicationUser { UserName = "ВенциславИванов", Email = "ventsislav.ivanov@example.com", FirstName = "Венцислав", LastName = "Иванов", Position = "Tourist", PhoneNumber = "+359881901234" }, "венциславиванов", "Tourist"),
                (new ApplicationUser { UserName = "РумянаДимитрова", Email = "rumyana.dimitrova@example.com", FirstName = "Румяна", LastName = "Димитрова", Position = "Tourist", PhoneNumber = "+359882012345" }, "румянадимитрова", "Tourist"),
                (new ApplicationUser { UserName = "ЗдравкоПетров", Email = "zdravko.petrov@example.com", FirstName = "Здравко", LastName = "Петров", Position = "Tourist", PhoneNumber = "+359882123456" }, "здравкопетров", "Tourist"),
            };

            foreach (var (user, password, role) in users)
            {
                if (await userManager.FindByNameAsync(user.UserName) == null)
                {
                    user.EmailConfirmed = true;
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            // Create driver requests for some tourists (pending driver applications)
            if (!context.RequestDrivers.Any())
            {
                var touristUsers = (await userManager.GetUsersInRoleAsync("Tourist")).ToList();
                var pendingDriverRequests = touristUsers.Take(3)
                    .Select((user, index) => new RequestDriver
                    {
                        UserId = user.Id,
                        StatusRequest = RequestStatus.Pending,
                        Date = DateTime.UtcNow.AddDays(-(5 - 2 * index)) // -5, -3, -1
                    }).ToList();


                await context.RequestDrivers.AddRangeAsync(pendingDriverRequests);
                await context.SaveChangesAsync();
            }

            // Create trips if they don't exist
            if (!context.Trips.Any())
            {
                var drivers = await userManager.GetUsersInRoleAsync("Driver");
                var carModels = new string[]
                {
                    "VW Golf 4", "Toyota Corolla", "Skoda Octavia", "Renault Megane", "Ford Focus",
                    "Opel Astra", "Peugeot 308", "Audi A4", "BMW 320", "Mercedes C200",
                    "Citroen C4", "Seat Leon", "Hyundai i30", "Kia Ceed", "Mazda 3"
                };

                var plateNumbers = new string[]
                {
                    "СА1234ВХ", "СВ5678МН", "ВТ9012КР", "РВ3456СТ", "ЕВ7890АХ",
                    "ВР1234МК", "СМ5678НР", "ПВ9012ТХ", "ВН3456КМ", "ТХ7890СР",
                    "СО1234АВ", "ВР5678СТ", "ПК9012МН", "ВВ3456АХ", "СС7890КР"
                };

                var cities = new string[]
                {
                    "София", "Пловдив", "Варна", "Бургас", "Русе", "Стара Загора", "Плевен",
                    "Добрич", "Сливен", "Шумен", "Перник", "Хасково", "Ямбол", "Пазарджик",
                    "Благоевград", "Велико Търново", "Враца", "Габрово", "Асеновград", "Видин",
                    "Казанлък", "Кърджали", "Кюстендил", "Монтана", "Търговище", "Силистра",
                    "Ловеч", "Смолян", "Разград", "Свищов", "Петрич", "Троян", "Севлиево",
                    "Карлово", "Банско", "Несебър", "Созопол", "Сандански", "Велинград", "Самоков"
                };

                var random = new Random();
                var trips = new List<Trip>();

                // Dictionary to track driver's trips and their time periods
                var driverTrips = new Dictionary<string, List<(DateTime Start, DateTime End)>>();

                // Initialize the dictionary for each driver
                foreach (var driver in drivers)
                {
                    driverTrips[driver.Id] = new List<(DateTime Start, DateTime End)>();
                }

                // Create 50 trips with different statuses
                for (int i = 0; i < 50; i++)
                {
                    var startCity = cities[random.Next(cities.Length)];
                    string destinationCity;
                    do
                    {
                        destinationCity = cities[random.Next(cities.Length)];
                    } while (destinationCity == startCity);

                    var totalSeats = random.Next(2, 6);
                    var freeSeats = random.Next(0, totalSeats + 1);
                    var price = random.Next(20, 101);
                    var carModel = carModels[random.Next(carModels.Length)];
                    var plateNumber = plateNumbers[random.Next(plateNumbers.Length)];
                    var isRecurring = random.Next(2) == 0; // 50% chance of being recurring

                    // Set recurrence interval based on isRecurring
                    string recurrenceInterval = "00:00:00"; // Default for non-recurring trips
                    if (isRecurring)
                    {
                        int days = random.Next(5, 15); // 5-14 days
                        recurrenceInterval = $"{days}.00:00:00"; // Format: days.hours:minutes:seconds
                    }

                    // Determine trip status and dates based on the index
                    TripStatus status;
                    DateTime departureTime;
                    DateTime returnTime;

                    if (i < 15) // 15 upcoming trips
                    {
                        if (freeSeats == 0)
                        {
                            status = TripStatus.Booked; // If no free seats, mark as Booked
                        }
                        else
                        {
                            status = TripStatus.Upcoming;
                        }
                        departureTime = DateTime.UtcNow.AddDays(random.Next(1, 15));
                        returnTime = departureTime.AddHours(random.Next(2, 8));
                    }
                    else if (i < 25) // 10 booked trips
                    {
                        status = TripStatus.Booked;
                        departureTime = DateTime.UtcNow.AddDays(random.Next(1, 15));
                        returnTime = departureTime.AddHours(random.Next(2, 8));
                        freeSeats = 0; // Booked trips have no free seats
                    }
                    else if (i < 35) // 10 ongoing trips
                    {
                        status = TripStatus.Ongoing;
                        departureTime = DateTime.UtcNow.AddDays(-random.Next(1, 3));
                        returnTime = DateTime.UtcNow.AddDays(random.Next(10, 15)); // 2-week long trips
                    }
                    else // 15 finished trips
                    {
                        status = TripStatus.Finished;
                        departureTime = DateTime.UtcNow.AddDays(-random.Next(15, 60));
                        returnTime = departureTime.AddHours(random.Next(2, 8));
                    }

                    // Calculate NextRunDate based on departureTime and recurrenceInterval
                    DateTime? nextRunDate = null;
                    DateTime nextStart = DateTime.UtcNow;
                    if (isRecurring)
                    {
                        // Parse days from recurrenceInterval
                        int days = int.Parse(recurrenceInterval.Split('.')[0]);
                        nextRunDate = departureTime.AddDays(days);
                        nextStart = departureTime.AddDays(days);
                    }

                    // Find a driver who doesn't have overlapping trips
                    string driverId = null;
                    var shuffledDrivers = drivers.OrderBy(x => random.Next()).ToList();

                    foreach (var driver in shuffledDrivers)
                    {
                        bool hasOverlap = false;

                        // Check if this driver has any overlapping trips
                        foreach (var (start, end) in driverTrips[driver.Id])
                        {
                            // Check if the new trip overlaps with any existing trip
                            if ((departureTime <= end && returnTime >= start))
                            {
                                hasOverlap = true;
                                break;
                            }
                        }

                        if (!hasOverlap)
                        {
                            driverId = driver.Id;
                            driverTrips[driverId].Add((departureTime, returnTime));
                            break;
                        }
                    }

                    // If no driver is available without overlap, adjust the dates
                    if (driverId == null)
                    {

                         if (shuffledDrivers.Count == 0)
                            {
                                // Still no drivers? Skip this trip
                                continue;
                            }

                        // Take the first driver and adjust dates to avoid overlap
                        driverId = shuffledDrivers[0].Id;

                        // Find a time slot that doesn't overlap
                        bool foundSlot = false;
                        for (int attempt = 0; attempt < 10; attempt++)
                        {
                            // Try a different time period
                            if (status == TripStatus.Ongoing)
                            {
                                departureTime = DateTime.UtcNow.AddDays(-random.Next(1, 3)).AddHours(random.Next(24));
                                returnTime = departureTime.AddDays(random.Next(1, 3));
                            }
                            else if (status == TripStatus.Upcoming || status == TripStatus.Booked)
                            {
                                departureTime = DateTime.UtcNow.AddDays(random.Next(1, 30));
                                returnTime = departureTime.AddHours(random.Next(2, 8));
                            }
                            else // Finished
                            {
                                departureTime = DateTime.UtcNow.AddDays(-random.Next(15, 60));
                                returnTime = departureTime.AddHours(random.Next(2, 8));
                            }

                            // Check if this new time slot overlaps with any existing trips
                            bool hasOverlap = false;
                            foreach (var (start, end) in driverTrips[driverId])
                            {
                                if ((departureTime <= end && returnTime >= start))
                                {
                                    hasOverlap = true;
                                    break;
                                }
                            }

                            if (!hasOverlap)
                            {
                                foundSlot = true;
                                driverTrips[driverId].Add((departureTime, returnTime));
                                break;
                            }
                        }

                        // If still can't find a non-overlapping slot, skip this trip
                        if (!foundSlot)
                        {
                            continue;
                        }
                    }

                    var trip = new Trip
                    {
                        DriversId = driverId,
                        StartPosition = startCity,
                        Destination = destinationCity,
                        DepartureTime = departureTime,
                        ReturnTime = returnTime,
                        Price = price,
                        TotalSeats = totalSeats,
                        FreeSeats = freeSeats,
                        CarModel = carModel,
                        PlateNumber = plateNumber,
                        ImagePath = "/images/trips/default-image.jpg",
                        StatusTrip = status,
                        CreatedDate = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                        IsRecurring = isRecurring,
                        RecurrenceInterval = recurrenceInterval,
                        NextRunDate = nextRunDate,
                        NextStart = nextStart
                    };

                    trips.Add(trip);
                }

                // Create some trips for admin
                var adminId = (await userManager.FindByNameAsync("Админ")).Id;

                // Create 3 trips for admin (1 upcoming, 1 ongoing, 1 finished)
                var adminTripStatuses = new[] { TripStatus.Upcoming, TripStatus.Ongoing, TripStatus.Finished };

                for (int i = 0; i < 3; i++)
                {
                    var startCity = cities[random.Next(cities.Length)];
                    string destinationCity;
                    do
                    {
                        destinationCity = cities[random.Next(cities.Length)];
                    } while (destinationCity == startCity);

                    var totalSeats = random.Next(2, 6);
                    var freeSeats = random.Next(1, totalSeats + 1); // Ensure at least 1 free seat for upcoming trips
                    var price = random.Next(20, 101);
                    var carModel = carModels[random.Next(carModels.Length)];
                    var plateNumber = plateNumbers[random.Next(plateNumbers.Length)];
                    var isRecurring = random.Next(2) == 0;

                    string recurrenceInterval = "00:00:00";
                    if (isRecurring)
                    {
                        int days = random.Next(5, 15);
                        recurrenceInterval = $"{days}.00:00:00";
                    }

                    TripStatus status = adminTripStatuses[i];
                    DateTime departureTime;
                    DateTime returnTime;

                    // Create different status trips for admin
                    if (status == TripStatus.Upcoming)
                    {
                        departureTime = DateTime.UtcNow.AddDays(random.Next(1, 15));
                        returnTime = departureTime.AddHours(random.Next(2, 8));
                        if (freeSeats == 0)
                        {
                            status = TripStatus.Booked;
                        }
                    }
                    else if (status == TripStatus.Ongoing)
                    {
                        // Make sure it doesn't overlap with any existing admin trips
                        departureTime = DateTime.UtcNow.AddDays(-random.Next(1, 3));
                        returnTime = DateTime.UtcNow.AddDays(random.Next(1, 3));

                        // Check for overlap with existing admin trips
                        bool hasOverlap = false;
                        foreach (var (start, end) in driverTrips[adminId])
                        {
                            if ((departureTime <= end && returnTime >= start))
                            {
                                hasOverlap = true;
                                break;
                            }
                        }

                        // If overlap, adjust dates
                        if (hasOverlap)
                        {
                            departureTime = DateTime.UtcNow.AddDays(3); // Set to future date
                            returnTime = departureTime.AddDays(2);
                            status = TripStatus.Upcoming; // Change status to upcoming
                        }
                    }
                    else // Finished
                    {
                        departureTime = DateTime.UtcNow.AddDays(-random.Next(15, 60));
                        returnTime = departureTime.AddHours(random.Next(2, 8));
                    }

                    // Calculate NextRunDate
                    DateTime? nextRunDate = null;
                    DateTime nextStart = DateTime.UtcNow;
                    if (isRecurring)
                    {
                        int days = int.Parse(recurrenceInterval.Split('.')[0]);
                        nextRunDate = departureTime.AddDays(days);
                        nextStart = departureTime.AddDays(days);
                    }

                    var adminTrip = new Trip
                    {
                        DriversId = adminId,
                        StartPosition = startCity,
                        Destination = destinationCity,
                        DepartureTime = departureTime,
                        ReturnTime = returnTime,
                        Price = price,
                        TotalSeats = totalSeats,
                        FreeSeats = freeSeats,
                        CarModel = carModel,
                        PlateNumber = plateNumber,
                        ImagePath = "/images/trips/default-image.jpg",
                        StatusTrip = status,
                        CreatedDate = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                        IsRecurring = isRecurring,
                        RecurrenceInterval = recurrenceInterval,
                        NextRunDate = nextRunDate,
                        NextStart = nextStart
                    };

                    trips.Add(adminTrip);
                    driverTrips[adminId].Add((departureTime, returnTime));
                }

                await context.Trips.AddRangeAsync(trips);
                await context.SaveChangesAsync();
            }

            // Create trip participants and requests
            if (!context.TripParticipants.Any())
            {
                var allUsers = await userManager.Users.ToListAsync();
                var tourists = await userManager.GetUsersInRoleAsync("Tourist");
                var drivers = await userManager.GetUsersInRoleAsync("Driver");
                var admin = await userManager.FindByNameAsync("Админ");
                var trips = await context.Trips.ToListAsync();
                var random = new Random();
                var tripParticipants = new List<TripParticipant>();
                var requests = new List<Request>();

                if (!drivers.Any())
                {
                    Console.WriteLine("No drivers available to assign trips. Skipping trip creation.");
                    return; // exit the trip creation block
                }

                // Dictionary to track user's trip participations and their time periods
                var userParticipations = new Dictionary<string, List<(DateTime Start, DateTime End)>>();

                // Initialize the dictionary for each user
                foreach (var user in allUsers)
                {
                    userParticipations[user.Id] = new List<(DateTime Start, DateTime End)>();
                }

                // For each trip that's not upcoming, add some participants
                foreach (var trip in trips.Where(t => t.StatusTrip != TripStatus.Upcoming))
                {
                    // Determine how many participants to add (between 1 and total seats)
                    int participantsCount = random.Next(1, trip.TotalSeats + 1);

                    // Get a mix of tourists and drivers (excluding the trip's driver)
                    var potentialParticipants = allUsers
                        .Where(u => u.Id != trip.DriversId)
                        .OrderBy(x => random.Next())
                        .ToList();

                    int addedParticipants = 0;

                    var maxParticipants = Math.Min(participantsCount, potentialParticipants.Count);
                    var selectedParticipants = potentialParticipants.Take(maxParticipants);

                    foreach (var participant in selectedParticipants)
                    {
                        // Check if this user has any overlapping trips
                        bool hasOverlap = false;
                        foreach (var (start, end) in userParticipations[participant.Id])
                        {
                            if ((trip.DepartureTime <= end && trip.ReturnTime >= start))
                            {
                                hasOverlap = true;
                                break;
                            }
                        }

                        if (!hasOverlap)
                        {
                            // Create an accepted request first
                            var request = new Request
                            {
                                TripId = trip.Id,
                                UserId = participant.Id,
                                StatusRequest = RequestStatus.Accepted,
                                Date = trip.CreatedDate.AddDays(random.Next(1, 5)),
                                NumberOfSeats = random.Next(1, 3) // 1 or 2 seats
                            };
                            requests.Add(request);

                            // Then create a trip participant
                            var tripParticipant = new TripParticipant
                            {
                                TripId = trip.Id,
                                UserId = participant.Id,
                                NumberOfSeats = request.NumberOfSeats
                            };
                            tripParticipants.Add(tripParticipant);

                            // Add this participation to the user's list
                            userParticipations[participant.Id].Add((trip.DepartureTime, trip.ReturnTime));

                            addedParticipants++; // optional, if you want to track count
                        }
                    }

                }

                // Make admin participate in some trips
                var tripsForAdmin = trips
                    .Where(t => t.DriversId != admin.Id && (t.StatusTrip == TripStatus.Finished || t.StatusTrip == TripStatus.Ongoing))
                    .OrderBy(x => random.Next())
                    .Take(Math.Min(1, trips.Count()))
                    .ToList();

                foreach (var trip in tripsForAdmin)
                {
                    // Check if admin has any overlapping trips
                    bool hasOverlap = false;
                    foreach (var (start, end) in userParticipations[admin.Id])
                    {
                        if ((trip.DepartureTime <= end && trip.ReturnTime >= start))
                        {
                            hasOverlap = true;
                            break;
                        }
                    }

                    if (!hasOverlap)
                    {
                        // Create an accepted request for admin
                        var request = new Request
                        {
                            TripId = trip.Id,
                            UserId = admin.Id,
                            StatusRequest = RequestStatus.Accepted,
                            Date = trip.CreatedDate.AddDays(random.Next(1, 5)),
                            NumberOfSeats = 1
                        };
                        requests.Add(request);

                        // Create trip participant for admin
                        var tripParticipant = new TripParticipant
                        {
                            TripId = trip.Id,
                            UserId = admin.Id,
                            NumberOfSeats = 1
                        };
                        tripParticipants.Add(tripParticipant);

                        // Add this participation to admin's list
                        userParticipations[admin.Id].Add((trip.DepartureTime, trip.ReturnTime));

                        // Only add one trip for admin to avoid too many
                        break;
                    }
                }

                // For upcoming trips, create some pending requests
                foreach (var trip in trips.Where(t => t.StatusTrip == TripStatus.Upcoming && t.FreeSeats > 0))
                {
                    // Determine how many requests to add
                    int requestsCount = random.Next(2, 6);

                    // Get a mix of tourists and drivers (excluding the trip's driver)
                    var potentialRequesters = allUsers
                        .Where(u => u.Id != trip.DriversId)
                        .OrderBy(x => random.Next())
                        .Take(requestsCount)
                        .ToList();

                    var maxRequesters = Math.Min(requestsCount, potentialRequesters.Count);
                    var finalRequesters = potentialRequesters.Take(maxRequesters).ToList();


                    foreach (var requester in finalRequesters)
                    {
                        // For pending requests, we'll check for overlaps with accepted trips
                        bool hasOverlap = false;
                        foreach (var (start, end) in userParticipations[requester.Id])
                        {
                            if ((trip.DepartureTime <= end && trip.ReturnTime >= start))
                            {
                                hasOverlap = true;
                                break;
                            }
                        }

                        // For demonstration purposes, we'll allow some overlapping pending requests
                        // but not for users who already have accepted trips during that time
                        if (!hasOverlap || random.Next(10) < 3) // 30% chance to create overlapping request for demo
                        {
                            var request = new Request
                            {
                                TripId = trip.Id,
                                UserId = requester.Id,
                                StatusRequest = RequestStatus.Pending,
                                Date = DateTime.UtcNow.AddDays(-random.Next(1, 5)),
                                NumberOfSeats = random.Next(1, Math.Min(3, trip.FreeSeats + 1)) // 1 or 2 seats, not exceeding free seats
                            };
                            requests.Add(request);
                        }
                    }
                }

                // Create pending requests for admin
                var upcomingTripsForAdmin = trips
                    .Where(t => t.DriversId != admin.Id && t.StatusTrip == TripStatus.Upcoming && t.FreeSeats > 0)
                    .OrderBy(x => random.Next())
                    .Take(2)
                    .ToList();

                foreach (var trip in upcomingTripsForAdmin)
                {
                    // Check if admin has any overlapping trips
                    bool hasOverlap = false;
                    foreach (var (start, end) in userParticipations[admin.Id])
                    {
                        if ((trip.DepartureTime <= end && trip.ReturnTime >= start))
                        {
                            hasOverlap = true;
                            break;
                        }
                    }

                    // For admin, we'll only create non-overlapping requests
                    if (!hasOverlap)
                    {
                        var request = new Request
                        {
                            TripId = trip.Id,
                            UserId = admin.Id,
                            StatusRequest = RequestStatus.Pending,
                            Date = DateTime.UtcNow.AddDays(-random.Next(1, 3)),
                            NumberOfSeats = 1
                        };
                        requests.Add(request);
                    }
                }

                // Create overlapping requests for demonstration (only for upcoming trips)
                // This is intentional to show how the system handles overlapping requests
                var upcomingTrips = trips
                    .Where(t => t.StatusTrip == TripStatus.Upcoming && t.FreeSeats > 0)
                    .Take(3)
                    .ToList();

                if (upcomingTrips.Count >= 2)
                {
                    // Create overlapping requests for a few tourists
                    foreach (var tourist in tourists.Take(3))
                    {
                        // First trip request
                        var request1 = new Request
                        {
                            TripId = upcomingTrips[0].Id,
                            UserId = tourist.Id,
                            StatusRequest = RequestStatus.Pending,
                            Date = DateTime.UtcNow.AddDays(-2),
                            NumberOfSeats = 1
                        };
                        requests.Add(request1);

                        // Second trip request (overlapping time)
                        var request2 = new Request
                        {
                            TripId = upcomingTrips[1].Id,
                            UserId = tourist.Id,
                            StatusRequest = RequestStatus.Pending,
                            Date = DateTime.UtcNow.AddDays(-1),
                            NumberOfSeats = 1
                        };
                        requests.Add(request2);
                    }
                }

                await context.Requests.AddRangeAsync(requests);
                await context.TripParticipants.AddRangeAsync(tripParticipants);
                await context.SaveChangesAsync();
            }

            // Create ratings for finished trips
            if (!context.Ratings.Any())
            {
                var finishedTrips = await context.Trips
                    .Where(t => t.StatusTrip == TripStatus.Finished)
                    .Include(t => t.TripParticipants)
                    .ToListAsync();

                var random = new Random();
                var ratings = new List<Rating>();

                var reviewComments = new string[]
                {
                    "Пътуването беше страхотно! Шофьорът беше много внимателен.",
                    "Много приятно пътуване, ще пътувам отново с този шофьор.",
                    "Колата беше чиста и удобна, препоръчвам!",
                    "Шофьорът беше точен и любезен, нямам забележки.",
                    "Приятна компания и безопасно шофиране.",
                    "Малко закъсня на тръгване, но иначе всичко беше наред.",
                    "Добро съотношение цена-качество, препоръчвам.",
                    "Шофьорът караше малко бързо, но иначе всичко беше наред.",
                    "Много приятен разговор по време на пътуването.",
                    "Пристигнахме навреме, всичко мина по план.",
                    "Колата беше малко тясна за толкова хора, но иначе всичко беше наред.",
                    "Шофьорът беше много отзивчив и помогна с багажа.",
                    "Пътуването мина гладко, без никакви проблеми.",
                    "Малко неудобни седалки, но за кратко пътуване е ок.",
                    "Шофьорът беше много внимателен на пътя, чувствах се в безопасност."
                };

                foreach (var trip in finishedTrips)
                {
                    // For each participant in the trip, create a rating
                    foreach (var participant in trip.TripParticipants.Where(p => !string.IsNullOrEmpty(p.UserId)))
                    {
                        // 80% chance to leave a rating
                        if (random.Next(10) < 8)
                        {
                            var rating = new Rating
                            {
                                TripId = trip.Id,
                                UserId = participant.UserId,
                                Score = random.Next(2, 6), // Ratings between 3 and 5
                                Comment = reviewComments[random.Next(reviewComments.Length)],
                                Date = trip.ReturnTime.AddDays(random.Next(1, 5))
                            };
                            ratings.Add(rating);
                        }
                    }
                }

                await context.Ratings.AddRangeAsync(ratings);
                await context.SaveChangesAsync();
            }

            Console.WriteLine("Database has been seeded successfully!");
        }
    }
}