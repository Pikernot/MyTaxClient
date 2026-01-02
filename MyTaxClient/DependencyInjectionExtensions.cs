using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyTaxClient;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMyTaxClient(
        this IServiceCollection services,
        IConfiguration? configuration = null,
        Action<MyTaxClientOptions>? configure = null)
    {
        var optionsBuilder = services.AddOptions<MyTaxClientOptions>();
        if (configuration is not null)
        {
            optionsBuilder.Bind(configuration.GetSection(MyTaxClientOptions.Section));
        }
        if (configure is not null)
        {
            optionsBuilder.Configure(configure);
        }
        optionsBuilder.PostConfigure(options =>
        {
            options.ApiUrl ??= "https://lknpd.nalog.ru/api/v1/";
            options.DeviceIdPrefix ??= string.Empty;
        });

        optionsBuilder.Validate(
            options =>
                !string.IsNullOrWhiteSpace(options.Username) &&
                !string.IsNullOrWhiteSpace(options.Password),
            $"{nameof(MyTaxReceiptsClient)} requires {nameof(MyTaxClientOptions.Username)} and {nameof(MyTaxClientOptions.Password)} to be configured.");
        optionsBuilder.ValidateOnStart();
        
        services.AddSingleton<MyTaxReceiptsClient>();
        return services;
    }
}