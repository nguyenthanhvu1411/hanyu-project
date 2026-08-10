using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace HanYu.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddHanYuSwagger(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.FullName);

            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "HanYu API",
                    Version = "v1",
                    Description = "HanYu Chinese Learning System API"
                });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT access token."
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        });

        return services;
    }

    public static WebApplication UseHanYuSwagger(
        this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "HanYu API v1");

            options.DisplayRequestDuration();
        });

        return app;
    }
}