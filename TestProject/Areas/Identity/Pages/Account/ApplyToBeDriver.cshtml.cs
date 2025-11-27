using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;



public class InputModel
{
    public IFormFile? ImageFile { get; set; }
}

namespace TestProject.Areas.Identity.Pages.Account
{
    public class ApplyToBeDriverModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public ApplyToBeDriverModel(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }


        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // Save driver application
            var request = new RequestDriver
            {
                UserId = user.Id,
                StatusRequest = RequestStatus.Pending,
                Date = DateTime.UtcNow
            };
            _context.RequestDrivers.Add(request);
            await _context.SaveChangesAsync();

            // Handle image upload (optional)
            if (Input.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "drivers");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await Input.ImageFile.CopyToAsync(stream);

                user.ImagePath = $"/images/drivers/{uniqueFileName}";
                await _userManager.UpdateAsync(user);
            }
            else
            {
                user.ImagePath = "/images/drivers/default-image-Driver.jpg";
                await _userManager.UpdateAsync(user);
            }

                return RedirectToPage("/Account/Manage/Index");
        }
    }
}
