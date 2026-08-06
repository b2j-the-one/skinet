namespace API.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AngularPolicy", policy =>
            {
                policy.WithOrigins(configuration.GetSection("Cors:AngularOrigins").Get<string[]>()!)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });

            options.AddPolicy("PublicApiPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    // Puis ton Program.cs devient beaucoup plus lisible :
    // builder.Services.AddCorsPolicies(builder.Configuration);
    // ...
    // app.UseCors("AngularPolicy");
} 