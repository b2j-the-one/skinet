using API.Configuration;

namespace API.Extensions;

public static class OptionsExtensions
{
    public static IServiceCollection AddApplicationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // this IServiceCollection services (transforme cette méthode en méthode d'extension)
        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        services.Configure<SmtpOptions>(
            configuration.GetSection("Smtp"));

        services.Configure<RedisOptions>(
            configuration.GetSection("Redis"));

        services.Configure<StripeOptions>(
            configuration.GetSection("Stripe"));

        services.Configure<AzureStorageOptions>(
            configuration.GetSection("AzureStorage"));

        services.Configure<ElasticSearchOptions>(
            configuration.GetSection("ElasticSearch"));

        services.Configure<RabbitMqOptions>(
            configuration.GetSection("RabbitMq"));

        return services;
    }
}