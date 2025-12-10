using Project_Manassas.Service;

namespace Nash_Manassas.utils;

public static class ApplicationServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IEmailSenderService, SendGridEmailService>();
        
        return services;
    }
}