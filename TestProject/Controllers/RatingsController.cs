using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;
using TestProject.Models.ViewModels;

namespace TestProject.Controllers
{
    [Authorize]
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _userManager = userManager;
        }


        // GET: Ratings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ratings.Include(r => r.Trip).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Trip)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Ratings/Create
        // GET: Ratings/Create/5
        public async Task<IActionResult> Create(int tripId, string? returnUrl, string? returnUrlOriginal)
        {
           
            var trip = await _context.Trips
                .Include(t => t.TripParticipants)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            var user = await _userManager.GetUserAsync(User);

            if (trip == null || user == null ||
                trip.StatusTrip != TripStatus.Finished ||
                !trip.TripParticipants.Any(tp => tp.UserId == user.Id))
            {
                return RedirectToAction("Details", "Trips", new { id = tripId });
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ReturnUrlOriginal = returnUrlOriginal;

            ViewData["TripId"] = tripId;
            return View();
        }

        // POST: Ratings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int tripId, [Bind("Score,Comment")] Rating rating, string? returnUrl, string? returnUrlOriginal)
        {
            var trip = await _context.Trips
                 .Include(t => t.TripParticipants)
                 .FirstOrDefaultAsync(t => t.Id == tripId);

            var user = await _userManager.GetUserAsync(User);
            if (trip == null || user == null ||
                            trip.StatusTrip != TripStatus.Finished ||
                            !trip.TripParticipants.Any(tp => tp.UserId == user.Id))
            {
                Console.WriteLine("Error");
                return RedirectToAction("Details", "Trips", new { id = tripId });
            }

            rating.UserId = user.Id;
            rating.Date = DateTime.UtcNow;
            rating.TripId = tripId;

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrlOriginal) && Url.IsLocalUrl(returnUrlOriginal))
            {
                return Redirect($"{returnUrl}?returnUrl={Uri.EscapeDataString(returnUrlOriginal)}"); // Redirects back to the previous page with returnUrl as a query parameter
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Details", "Trips", new { id = tripId, returnUrl = returnUrl });

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
            }

            await _context.SaveChangesAsync();
            //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            //{
            //    return Redirect(returnUrl); // Redirects back to the previous page
            //}

            return RedirectToAction("Details", "Trips", new { id = rating.TripId, returnUrl = returnUrl });
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
