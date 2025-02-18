using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RunningClub.Interfaces;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<RaceCompletionUpdateService>();
builder.Services.AddScoped<RaceRepository>();
builder.Services.AddScoped<PhotoService>();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddScoped<DashboardRepository>();
// builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ClubRepository, ClubRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    #if DEBUG
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection"));
    #else
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureConnection"));//TODO how tf does this work bro it's in secret
    #endif  
});
builder.Services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    UserManager<AppUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    ClubRepository clubRepository = scope.ServiceProvider.GetRequiredService<ClubRepository>();
    RaceRepository raceRepository = scope.ServiceProvider.GetRequiredService<RaceRepository>();
    int clubsAmount = 7;
    int raceAmount = 10;
    await Seed.InitializeRoles(roleManager);
    await Seed.InitializeUsers(roleManager, userManager);
    await Seed.InitializeClubs(userManager,clubRepository,clubsAmount);
    await Seed.InitializeRaces(userManager, raceRepository, clubRepository,raceAmount);
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

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{clubId?}");

app.Run();
