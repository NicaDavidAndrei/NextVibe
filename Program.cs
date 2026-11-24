using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextVibe.Data;
using NextVibe.Models;

using NextVibe.Repositories;
using NextVibe.Repositories.Interfaces;

using NextVibe.Services;
using NextVibe.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// REPOSITORIES
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMoodRepository, MoodRepository>();
builder.Services.AddScoped<IArtistMoodRepository, ArtistMoodRepository>();
builder.Services.AddScoped<IArtistAlbumRepository, ArtistAlbumRepository>();
builder.Services.AddScoped<IAlbumTrackRepository, AlbumTrackRepository>();
builder.Services.AddScoped<IUserFavoriteArtistRepository, UserFavoriteArtistRepository>();

// SERVICES
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMoodService, MoodService>();
builder.Services.AddScoped<IArtistMoodService, ArtistMoodService>();
builder.Services.AddScoped<IArtistAlbumService, ArtistAlbumService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAlbumTrackService, AlbumTrackService>();
builder.Services.AddScoped<IUserFavoriteArtistService, UserFavoriteArtistService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IColorPaletteService, ColorPaletteService>();
builder.Services.AddScoped<IAudioPreviewService, AudioPreviewService>();
builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

// SEED DATA + ROLES
await SeedData.InitializeAsync(app.Services, app.Configuration);

// POPULATE ALBUM COLORS
using (var scope = app.Services.CreateScope())
{
    var colorService = scope.ServiceProvider.GetRequiredService<IColorPaletteService>();
    await colorService.EnsureAlbumColorsPopulatedAsync();
}

// PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();