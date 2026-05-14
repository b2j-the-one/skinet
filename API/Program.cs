using API.Middleware;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt => 
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddCors();
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connString = builder.Configuration.GetConnectionString("Redis") 
        ?? throw new Exception("Impossible d'obtenir la chaîne de connexion Redis");
    var configuration = ConfigurationOptions.Parse(connString, true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddAuthorization();
// builder.Services.AddIdentityApiEndpoints<AppUser>()
//     .AddRoles<IdentityRole>()
//     .AddEntityFrameworkStores<StoreContext>();
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<StoreContext>()
.AddDefaultTokenProviders();
// Configuration du cookie d'authentification
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None; // Obligatoire pour Angular
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS obligatoire 
    
    // Durée de validité du cookie
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // (7 jours)

    // // Pour que le cookie soit renouvelé à chaque requête
    // options.SlidingExpiration = true;

    options.LoginPath = "/api/account/login";
    options.LogoutPath = "/api/account/logout";
});
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(policy => policy
    .WithOrigins("http://localhost:4200", "https://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    );

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// /**
// * MapIdentityApi<AppUser>() appartient au mode Identity API Endpoints (JWT, REST),
// * alors qu'on utilise maintenant Identity classique + cookies + AccountController
// **/
// app.MapGroup("api").MapIdentityApi<AppUser>(); // api/login

try 
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex) 
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
