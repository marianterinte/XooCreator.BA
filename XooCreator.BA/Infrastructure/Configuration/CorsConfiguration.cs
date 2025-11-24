namespace XooCreator.BA.Infrastructure.Configuration; 
 
public static class CorsConfiguration 
{ 
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy
                    .SetIsOriginAllowed(_ => true) // acceptă orice Origin; va întoarce Originul cererii (nu "*")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromHours(24))
            );
        });

        return services;
    }
}
