using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.OptionsModels;
using AspNetCoreIdentityApp.Web.Permissions;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.Web.Seeds;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddIdentityWithExt();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AntalyaPolicy", policy =>
    {
        policy.RequireClaim("city", "Antalya");
    });
    
    opt.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });
    
    opt.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement() { ThresholdAge = 18 });
    });
    
    opt.AddPolicy("OrderPermissionReadAndDelete", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Read);
        policy.RequireClaim("Permission", Permission.Order.Delete);
        policy.RequireClaim("Permission", Permission.Stock.Delete);
    });

    opt.AddPolicy("Permission.Order.Read", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Read);
    });
    opt.AddPolicy("Permission.Order.Delete", policy =>
    {
        policy.RequireClaim("Permission", Permission.Order.Delete);
    });
    opt.AddPolicy("Permission.Stock.Delete", policy =>
    {
        policy.RequireClaim("Permission", Permission.Stock.Delete);
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "IdentityAppCookie";
    options.LoginPath = new PathString("/Home/SignIn");
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");
    options.Cookie = cookieBuilder;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);

    //kullanýcý 60 gün boyuunca 1 kere bile giriþ yapsa bilgiler tutulur
    //giriþ yaptýgý gün Cookie süresi tekrar 60 gün olarak setlenir.
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();

builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();

builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    await PermissionSeed.Seed(roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
