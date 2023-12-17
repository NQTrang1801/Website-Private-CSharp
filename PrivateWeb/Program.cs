using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using PrivateWeb.Areas.Identity.Data;
using PrivateWeb.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PrivateWebContextConnection") ?? throw new InvalidOperationException("Connection string 'PrivateWebContextConnection' not found.");

builder.Services.AddDbContext<PrivateWebContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<PrivateWebUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<PrivateWebContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;

});

// Tùy chỉnh cấu hình RazorViewEngineOptions
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("~/Views/Home/Index/{0}.cshtml");
    options.ViewLocationFormats.Add("~/Views/Home/Product/{0}.cshtml");
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


app.Run();
