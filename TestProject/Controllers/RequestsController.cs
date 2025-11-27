using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models;
using TestProject.Models.ViewModels;

namespace TestProject.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendJoinRequest(int tripId, string? returnUrl, int numberOfSeats)
        {
            //var userId = _userManager.GetUserId(User); // Get the current user's ID
            var userId = User.Id();
            var trip = await _context.Trips.FindAsync(tripId);

            if (trip == null)
            {
                return NotFound(); // No free seats or trip doesn't exist
            }

            if (numberOfSeats <= 0)
            {
                TempData["ErrorMessage"] = "Броят места трябва да бъде поне 1.";
                return RedirectToAction("Details", "Trips", new { id = tripId, returnUrl = returnUrl });
            }

            if (trip.FreeSeats < numberOfSeats)
            {
                TempData["ErrorMessage"] = $"Няма достатъчно свободни места. Налични: {trip.FreeSeats}.";
                return RedirectToAction("Details", "Trips", new { id = tripId, returnUrl = returnUrl });
            }

            // Check if the user has already sent a request for this trip
            var existingRequest = await _context.Requests
                .FirstOrDefaultAsync(r => r.TripId == tripId && r.UserId == userId);

            if (existingRequest != null)
            {
                //ModelState.AddModelError(string.Empty, "You have already sent a request for this trip.");
                TempData["ErrorMessage"] = "Вече имате заявка за това пътуване.";

                var tripViewModel = new TripViewModel
                {
                    Id = trip.Id,
                    DriversId = trip.DriversId,
                    DriverName = trip.Driver?.UserName,
                    StartPosition = trip.StartPosition,
                    Destination = trip.Destination,
                    DepartureTime = trip.DepartureTime,
                    ReturnTime = trip.ReturnTime,
                    Price = trip.Price,
                    TotalSeats = trip.TotalSeats,
                    FreeSeats = trip.FreeSeats,
                    CarModel = trip.CarModel,
                    PlateNumber = trip.PlateNumber,
                    ImagePath = trip.ImagePath,
                    StatusTrip = trip.StatusTrip,
                };

                ViewBag.ReturnUrl = returnUrl;
                return View("~/Views/Trips/Details.cshtml", tripViewModel);
            }


            // Check for overlapping trips
            if (await HasOverlappingTrips(userId, tripId))
            {
                TempData["ErrorMessage"] = "Не можете да се присъедините към това пътуване, тъй като имате друго пътуване в този времеви интервал.";

                return RedirectToAction("Details", "Trips", new { id = tripId, returnUrl = returnUrl });
            }

            // Create a new request
            var request = new Request
            {
                TripId = tripId,
                UserId = userId,
                StatusRequest = RequestStatus.Pending,
                Date = DateTime.UtcNow,
                NumberOfSeats = numberOfSeats 
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заявката е изпратена успешно.";

            ViewBag.ReturnUrl = returnUrl;
            return RedirectToAction("Details", "Trips", new { id = tripId, returnUrl = returnUrl });
        }

        private async Task<bool> HasOverlappingTrips(string userId, int tripId)
        {
            // Get the trip the user wants to join
            var tripToJoin = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (tripToJoin == null)
            {
                return false;
            }

            // Get user's existing trips (as driver or passenger)
            var userTripsAsDriver = await _context.Trips
                .Where(t => t.DriversId == userId &&
                       (t.StatusTrip == TripStatus.Upcoming || t.StatusTrip == TripStatus.Ongoing))
                .ToListAsync();

            var userTripsAsPassenger = await _context.TripParticipants
                .Where(tp => tp.UserId == userId)
                .Include(tp => tp.Trip)
                .Where(tp => tp.Trip.StatusTrip == TripStatus.Upcoming || tp.Trip.StatusTrip == TripStatus.Ongoing)
                .Select(tp => tp.Trip)
                .ToListAsync();

            // Combine both lists
            var userTrips = userTripsAsDriver.Concat(userTripsAsPassenger).ToList();

            // Check for overlaps
            foreach (var trip in userTrips)
            {
                // Check if there's an overlap
                if ((tripToJoin.DepartureTime >= trip.DepartureTime && tripToJoin.DepartureTime <= trip.ReturnTime) ||
                    (tripToJoin.ReturnTime >= trip.DepartureTime && tripToJoin.ReturnTime <= trip.ReturnTime) ||
                    (tripToJoin.DepartureTime <= trip.DepartureTime && tripToJoin.ReturnTime >= trip.ReturnTime))
                {
                    return true;
                }
            }

            return false;
        }


        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var request = await _context.Requests.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null || request.StatusRequest != RequestStatus.Pending)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(request.TripId);
            if (trip == null || trip.FreeSeats < request.NumberOfSeats)
            {
                // Not enough seats available anymore
                TempData["ErrorMessage"] = $"Няма достатъчно свободни места за тази заявка. Заявката е отказана.";
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();

                return RedirectToAction("PendingRequests", "DriverRequests");
            }

            if (await HasOverlappingTrips(request.User.Id, trip.Id) == false)
            {
                // Add user to TripParticipants
                var tripParticipant = new TripParticipant
                {
                    TripId = request.TripId,
                    UserId = request.UserId,
                    NumberOfSeats = request.NumberOfSeats
                };
                _context.TripParticipants.Add(tripParticipant);

                // Decrement FreeSeats
                trip.FreeSeats -= request.NumberOfSeats;

                // Update trip status if FreeSeats becomes 0
                if (trip.FreeSeats == 0)
                {
                    trip.StatusTrip = TripStatus.Booked;
                }

                // Update request status
                request.StatusRequest = RequestStatus.Accepted;
            }
            else
            {
                _context.Requests.Remove(request);
            }

            await _context.SaveChangesAsync();


            // Check and remove any requests that can no longer be fulfilled due to insufficient seats
            var pendingRequests = await _context.Requests
                .Where(r => r.TripId == trip.Id && r.StatusRequest == RequestStatus.Pending && r.NumberOfSeats > trip.FreeSeats)
                .ToListAsync();

            foreach (var pendingRequest in pendingRequests)
            {
                _context.Requests.Remove(pendingRequest);
                await _context.SaveChangesAsync();
            }

            var requests = await _context.Requests.Include(r => r.User).Where(r => r.UserId == request.User.Id && r.StatusRequest == RequestStatus.Pending).ToListAsync();

            foreach (var request1 in requests)
            {
                var trip1 = await _context.Trips.FindAsync(request1.TripId);

                if (await HasOverlappingTrips(request1.User.Id, trip1!.Id))
                {
                    _context.Requests.Remove(request1);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingRequests", "DriverRequests");
        }

        public async Task<IActionResult> DenyRequest(int requestId)
        {
            var request = await _context.Requests.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.StatusRequest != RequestStatus.Pending)
            {
                return NotFound();
            }
            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("PendingRequests", "DriverRequests");
        }


        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Requests.Include(r => r.Trip).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.Trip)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TripId,UserId,StatusRequest,Date")] Request request)
        {
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", request.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", request.UserId);
            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", request.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", request.UserId);
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TripId,UserId,StatusRequest,Date")] Request request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", request.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", request.UserId);
            return View(request);
        }


        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
            }

            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl); // Redirects back to the previous page
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
