using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models.ViewModels;
using TestProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;

[Authorize(Roles = "Admin")]
public class ListAllUsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ListAllUsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IWebHostEnvironment webHostEnvironment,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _signInManager = signInManager;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Users(string roleFilter, string searchTerm, int? pageNumber)
    {


        // Fetch all roles for the dropdown
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", roleFilter);
        ViewBag.CurrentRoleFilter = roleFilter;

        // Get all users with fresh data
        var users = await _userManager.Users.AsNoTracking().ToListAsync();

        // Create a list to store user view models
        var userViewModels = new List<UserViewModel>();

        // For each user, get their current role (this ensures we always have the latest role)
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            //var role = roles.FirstOrDefault() ?? "No Role";
            var role = "";

            if (roles.Contains("Admin"))
            {
                role = "Admin";
            }
            else if (roles.Contains("Driver"))
            {
                role = "Driver";
            }
            else if (roles.Contains("Tourist"))
            {
                role = "Tourist";
            }
            else
            {
                role = "No Role";
            }


                userViewModels.Add(new UserViewModel
                {
                    User = user,
                    Role = role
                });
        }

        // Calculate total users before applying filters
        var totalUsers = userViewModels.Count;
        ViewBag.UserCountMessage = $"Общо потребители: {totalUsers}";

        // Apply role filter
        if (!string.IsNullOrEmpty(roleFilter))
        {
            userViewModels = userViewModels.Where(u => u.Role == roleFilter).ToList();
            ViewBag.UserCountMessage = $"Потребитери с роля {roleFilter}: {userViewModels.Count}";
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            userViewModels = userViewModels.Where(u =>
                u.User.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.User.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.User.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        ViewBag.FilteredUserCountMessage = $"Филтрирани резултати: {userViewModels.Count}";

        // Pagination
        int pageSize = 5; // Increased from 3 to 10 for better usability
        var paginatedUsers = PaginatedList<UserViewModel>.CreateFromList(userViewModels, pageNumber ?? 1, pageSize);

        return View(paginatedUsers);
    }
  
    // POST: Admin/EditRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Validate role exists
        var role = await _roleManager.FindByNameAsync(newRole);
        if (role == null)
        {
            ModelState.AddModelError("", "Invalid role selected.");
            return RedirectToAction(nameof(Users));
        }

        // Remove all existing roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        if (newRole == "Driver" && !currentRoles.Contains("Driver"))
        {
            user.DateOfDriverAcceptance = DateTime.UtcNow;

            var requestDrivers = await _context.RequestDrivers.Where(rd => rd.UserId == user.Id).ToListAsync();

            if (!requestDrivers.Any())
            {
                user.ImagePath = "/images/drivers/default-image-Driver.jpg";
            }

            _context.RequestDrivers.RemoveRange(requestDrivers);
        }
        else if (newRole == "Tourist")
        {
            user.DateOfDriverAcceptance = null;

            // Check if the current image is not the default image before deleting
            if (!string.IsNullOrEmpty(user.ImagePath) && !user.ImagePath.Equals("/images/drivers/default-image-Driver.jpg", StringComparison.OrdinalIgnoreCase))
            {
                var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, user.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImage))
                {
                    System.IO.File.Delete(oldImage);
                }
            }

            user.ImagePath = null;

            var trips = await _context.Trips.Where(rd => rd.DriversId == user.Id && rd.StatusTrip != TripStatus.Finished).ToListAsync();
            _context.Trips.RemoveRange(trips);

            var tripsFinished = await _context.Trips.Where(rd => rd.DriversId == user.Id && rd.StatusTrip == TripStatus.Finished).ToListAsync();
            foreach (var trip in tripsFinished)
            {
                trip.IsRecurring = false;
            }
            _context.Trips.UpdateRange(tripsFinished); // update or updateRange?

            var requests = await _context.Requests.Where(r => r.Trip.DriversId == user.Id && r.Trip.StatusTrip != TripStatus.Finished).ToListAsync();
            _context.Requests.RemoveRange(requests);

            var tripParticipants = await _context.TripParticipants.Where(tp => tp.Trip.DriversId == user.Id && tp.Trip.StatusTrip != TripStatus.Finished).ToListAsync();
            _context.TripParticipants.RemoveRange(tripParticipants);

        }

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Assign new role
        await _userManager.AddToRoleAsync(user, newRole);

        // Update user position and other properties based on role
        user.Position = newRole;

        // Save changes to user
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Users));
    }

    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Remove related data
        var trips = await _context.Trips.Where(t => t.DriversId == user.Id).ToListAsync();
        _context.Trips.RemoveRange(trips);

        var requests = await _context.Requests.Where(r => r.UserId == user.Id).ToListAsync();
        _context.Requests.RemoveRange(requests);

        var requestDrivers = await _context.RequestDrivers.Where(rd => rd.UserId == user.Id).ToListAsync();
        _context.RequestDrivers.RemoveRange(requestDrivers);

        var tripParticipants = await _context.TripParticipants.Where(tp => tp.UserId == user.Id).ToListAsync();
        _context.TripParticipants.RemoveRange(tripParticipants);

        // Delete the user
        await _userManager.DeleteAsync(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Users));
    }
}