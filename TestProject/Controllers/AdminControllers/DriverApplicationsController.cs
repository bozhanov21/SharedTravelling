using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TestProject.Data;
using TestProject.Models;

[Authorize(Roles = "Admin")]
public class DriverApplicationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly SignInManager<ApplicationUser> _signinManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DriverApplicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, SignInManager<ApplicationUser> signinManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _signinManager = signinManager;
        _roleManager = roleManager;
    }

    // GET: DriverApplications
    public async Task<IActionResult> Index(string? returnUrl)
    {
        var requests = await _context.RequestDrivers
            .Include(a => a.User)
            .Where(a => a.StatusRequest == RequestStatus.Pending)
            .ToListAsync();

        ViewBag.ReturnUrl = returnUrl;
        return View(requests);
    }

    // POST: DriverApplications/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? returnUrl)
    {
        var request = await _context.RequestDrivers.FindAsync(id);
        if (request == null || request.StatusRequest != RequestStatus.Pending)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        _context.RequestDrivers.RemoveRange(request);

      //  await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Set user role to "Driver"
        await _userManager.AddToRoleAsync(user, "Driver");

        // Update RequestDriver status
        request.StatusRequest = RequestStatus.Accepted;
        user.DateOfDriverAcceptance = DateTime.Now;

        user.Position = "Driver";
        
        if (string.IsNullOrEmpty(user.ImagePath))
        {
            user.ImagePath = "/images/drivers/default-image-Driver.jpg"; // Default image path
        }

        //await _signinManager.RefreshSignInAsync(user);

        await _userManager.UpdateAsync(user);

        await _context.SaveChangesAsync();

        
      
        //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return Redirect(returnUrl); // Redirects back to the previous page
        //}

        /* return RedirectToAction("MyRequests", "PassengerRequests")*/
        ;
        return RedirectToAction("Index");
    }

    // POST: DriverApplications/Deny/5
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Deny(int id, string? returnUrl)
    {
        var request = await _context.RequestDrivers.FindAsync(id);
        if (request == null || request.StatusRequest != RequestStatus.Pending)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user != null && !string.IsNullOrEmpty(user.ImagePath))
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ImagePath);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            user.ImagePath = null;
            await _userManager.UpdateAsync(user);
            await _signinManager.RefreshSignInAsync(user);
        }

        // Update RequestDriver status
        request.StatusRequest = RequestStatus.Rejected;

        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            ViewBag.ReturnUrl = returnUrl;
            return Redirect(returnUrl); // Redirects back to the previous page
        }

        //return RedirectToAction("MyRequests", "PassengerRequests");
        return RedirectToAction("Index");

    }
   
    // POST: EditRole
    [AllowAnonymous]
    public async Task<IActionResult> EditRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Remove all existing roles
        var currentRoles = await _userManager.GetRolesAsync(user);

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

        await _userManager.RemoveFromRoleAsync(user, "Driver");

        // Assign new role
        await _userManager.AddToRoleAsync(user, "Tourist");

        // Update user position and other properties based on role
        if (currentRoles.Contains("Admin"))
        {
            user.Position = "Admin";
        }
        else
        {
            user.Position = "Tourist";
        }

        // Save changes to user
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();

        // Sign the user out and back in to refresh the role
        await _signinManager.SignOutAsync();
        await _signinManager.SignInAsync(user, isPersistent: false);

        return Redirect("/Identity/Account/Manage");
    }
}