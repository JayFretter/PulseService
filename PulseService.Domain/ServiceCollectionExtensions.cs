using Microsoft.Extensions.DependencyInjection;
using PulseService.Domain.Handlers;
using PulseService.Domain.Providers;
using PulseService.Domain.Validation;

namespace PulseService.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPulseHandler, PulseHandler>()
            .AddSingleton<IUserHandler, UserHandler>()
            .AddSingleton<IDiscussionHandler, DiscussionHandler>()
            .AddSingleton<IProfileHandler, ProfileHandler>()
            .AddSingleton<IUserValidationService, UserValidationService>()
            .AddSingleton<IPulseValidationService, PulseValidationService>()
            .AddSingleton<IArgumentValidationService, ArgumentValidationService>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}