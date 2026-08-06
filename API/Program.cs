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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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
// builder.Services.AddScoped<IJwtService, JwtService>();
// builder.Services.AddSingleton<ICacheService, RedisCacheService>();
// builder.Services.AddScoped<IEmailService, SmtpEmailService>();

#region Lecture de la configuration avec IOptions<T>
// builder.Services.AddApplicationOptions(builder.Configuration);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
#region Depuis .NET 8, Microsoft s'est appuyé sur la RFC 7807 (HTTP Problem Details).
// // On devrait faire :
// builder.Services.AddProblemDetails();
// // Puis :
// app.UseExceptionHandler();
// // au lieu d'utiliser app.UseMiddleware<T>() pour gérer des exceptions personnalisées
// builder.Services.AddProblemDetails(options =>
// {
//     options.CustomizeProblemDetails = context =>
//     {
//         context.ProblemDetails.Extensions["traceId"] =
//             context.HttpContext.TraceIdentifier;

//         context.ProblemDetails.Extensions["timestamp"] =
//             DateTime.UtcNow;
//         // // Pour gérer différentes exceptions
//         // if (context.Exception is OrderNotFoundException)
//         // {
//         //     context.ProblemDetails.Title = "Order not found";
//         //     context.ProblemDetails.Status = StatusCodes.Status404NotFound;
//         // }
//     };
// });
// // // On obtient :
// // {
// //     "type":"...",
// //     "title":"Internal Server Error",
// //     "status":500,
// //     "traceId":"0HMS....",
// //     "timestamp":"2026-07-26T10:00:00Z"
// // }
#endregion

app.UseCors(policy => policy
    .WithOrigins("http://localhost:4200", "https://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    );
#region Sur de gros projets, on évite d'avoir la configuration du CORPS directement dans Program.cs
// // Le principe est simple : au lieu d'avoir une seule politique CORS, tu définis plusieurs politiques nommées, chacune adaptée à un cas d'usage.
// builder.Services.AddCors(options =>
// {
//     // Front Angular
//     options.AddPolicy("AngularPolicy", policy =>
//     {
//         policy.WithOrigins(
//                 "http://localhost:4200",
//                 "https://localhost:4200")
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials();
//     });

//     // Swagger
//     options.AddPolicy("SwaggerPolicy", policy =>
//     {
//         policy.WithOrigins("https://localhost:5001")
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//     });

//     // API publique
//     options.AddPolicy("PublicApiPolicy", policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//     });
// });

// // Ensuite, dans le pipeline : app.UseCors("AngularPolicy");
// // Ici, toute ton API utilisera la politique AngularPolicy.
// // Mais à quoi servent les autres politiques ? -> On peux appliquer une politique uniquement sur certains Controllers ou certaines Actions.
// // Ici ProductsController sera accessible depuis n'importe quel site.
// // [ApiController]
// // [Route("api/[controller]")]
// // [EnableCors("PublicApiPolicy")]
// // public class ProductsController : ControllerBase
// // {
// //     [HttpGet]
// //     public IActionResult GetProducts()
// //     {
// //         ...
// //     }
// // }
// // Alors que ton contrôleur de paiement sera uniquement accessible depuis ton application Angular.
// // [ApiController]
// // [Route("api/[controller]")]
// // [EnableCors("AngularPolicy")]
// // public class PaymentController : ControllerBase
// // {
// // }


// // Adaptation des politiques selon l'environnement :
// // Angular (Développement) -> http://localhost:4200
// // Angular (Recette) -> https://recette.maboutique.fr
// // Angular (Production) -> https://maboutique.fr
// if (builder.Environment.IsDevelopment())
// {
//     builder.Services.AddCors(options =>
//     {
//         options.AddPolicy("AngularPolicy", policy =>
//         {
//             policy.WithOrigins("http://localhost:4200")
//                 .AllowAnyHeader()
//                 .AllowAnyMethod()
//                 .AllowCredentials();
//         });
//     });
// }
// else if (builder.Environment.IsStaging())
// {
//    builder.Services.AddCors(options =>
//     {
//         options.AddPolicy("AngularPolicy", policy =>
//         {
//             policy.WithOrigins("https://recette.maboutique.fr")
//                 .AllowAnyHeader()
//                 .AllowAnyMethod()
//                 .AllowCredentials();
//         });
//     }); 
// }
// else
// {
//     builder.Services.AddCors(options =>
//     {
//         options.AddPolicy("AngularPolicy", policy =>
//         {
//             policy.WithOrigins("https://maboutique.fr")
//                 .AllowAnyHeader()
//                 .AllowAnyMethod()
//                 .AllowCredentials();
//         });
//     });
// }
// // Encore mieux, les URLs peuvent être lues depuis appsettings.json pour éviter de modifier le code lors d'un changement d'environnement.
// // Une bonne pratique consiste à créer une classe d'extension, par exemple : CorsExtensions
#endregion

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
