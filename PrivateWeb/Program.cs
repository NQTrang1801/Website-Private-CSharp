using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Areas.Identity.Data;
using PrivateWeb.Data;
using PrivateWeb.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PrivateWebContextConnection") ?? throw new InvalidOperationException("Connection string 'PrivateWebContextConnection' not found.");

builder.Services.AddDbContext<PrivateWeb.Data.PrivateWebContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<PrivateWeb.Models.PrivateWebContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<PrivateWebUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<PrivateWeb.Data.PrivateWebContext>();



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;

});

// Tùy chỉnh cấu hình RazorViewEngineOptions
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("~/Views/Home/Index/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Views/Home/Product/{0}.cshtml");
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


app.Run();