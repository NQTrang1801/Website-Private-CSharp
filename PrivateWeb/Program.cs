using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Areas.Identity.Data;
using PrivateWeb.Hubs;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PrivateWebContextConnection") ?? throw new InvalidOperationException("Connection string 'PrivateWebContextConnection' not found.");

builder.Services.AddDbContext<PrivateWeb.Data.PrivateWebContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<PrivateWeb.Models.PrivateWebContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<PrivateWebUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PrivateWeb.Data.PrivateWebContext>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ClientOnly", policy => policy.RequireRole("Client"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddSignalR();
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;

});

// Tùy chỉnh cấu hình RazorViewEngineOptions
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("~/Views/Home/Index/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Views/Home/Cart/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Views/Home/Checkout/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Views/Home/OrderHistories/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Views/Home/Profile/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Views/Home/ProductDetails/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Areas/Identity/Pages/Account/{0}.cshtml");

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
            name: "getSlug",
            pattern: "getSlug",
            defaults: new { controller = "Utility", action = "GetSlug" });


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Manager", "Member" };
    foreach(var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }    
    }    
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<PrivateWebUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var _userStore = scope.ServiceProvider.GetRequiredService<IUserStore<PrivateWebUser>>();

    var users = new[]
    {
        new PrivateWebUser { FirstName = "User", LastName = "Admin"},
        new PrivateWebUser { FirstName = "User", LastName = "Manager"},
        new PrivateWebUser { FirstName = "User", LastName = "Member"}
    };
    await _userStore.SetUserNameAsync(users[0], "admin@gmail.com", CancellationToken.None);
    await _userStore.SetUserNameAsync(users[1], "manager@gmail.com", CancellationToken.None);
    await _userStore.SetUserNameAsync(users[2], "member@gmail.com", CancellationToken.None);
    foreach (var user in users)
    {
        var result = await userManager.CreateAsync(user, "Usersys123!");
        if (result.Succeeded)
        {
            if (user.UserName == "admin@gmail.com")
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else if (user.UserName == "manager@gmail.com")
            {
                await userManager.AddToRoleAsync(user, "Manager");
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else if (user.UserName == "member@gmail.com")
            {
                await userManager.AddToRoleAsync(user, "Member");
            }
        }
    }
}

app.MapHub<ChatHub>("/chatHub");

app.Run();