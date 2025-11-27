using Microsoft.AspNetCore.Identity;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using TestProject.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHostedService<TripSchedulerService>();
builder.Services.AddHostedService<TripStatusUpdaterService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.AllowedUserNameCharacters =
   "������������������������������������������������������������abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
});

var app = builder.Build();

using (var conn = new NpgsqlConnection(postgresConnectionString))
{
    conn.Open();
    using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM MyTable", conn);
    var count = (long)cmd.ExecuteScalar();
    Console.WriteLine($"Rows in PostgreSQL table: {count}");
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await ApplicationDbSeed.SeedDataAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
//app.MapControllers();

app.Run();

// add icons from mom

// backgruond service for removing request that can't be accepted anymore (trip is in progress or finished)
// fix foulders and files
// fix the project name
// fix the role to be passenger not tourist

// trips pictures aren't deleted when it is changed in edit form
//back to list fucntion - javascript:history.back()

// AIs and other useful info
// v0 ; napkin ai ; wireframe ; sonnet-claude ; heruUi ai ; NotebookLM ; ai agents (gemini) ; figma ; eraser ; paper ; codepen ; cursor ai ; bolt(.new) ai ; saveWeb2Zip ; b12 website builder ai ; 
// Localend ; ShareX ; TranslucentTB ; Revo Uninstaller ; Obuntu ; JavaScript ; 
