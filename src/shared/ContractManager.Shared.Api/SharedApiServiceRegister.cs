using ContractManager.Shared.Api.Extensions;
using ContractManager.Shared.Api.Swagger;
using ContractManager.Shared.Application;
using ContractManager.Shared.Application.Security.Configuration;
using ContractManager.Shared.Core.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace ContractManager.Shared.Api;
public static class SharedApiServiceRegister
{
    public static IServiceCollection AddSharedApiServices(
        this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry();

        services.AddSharedApplicationServices();

        services.AddCors(options =>
        {
            options.AddPolicy(name: CorsPolicyBuilderExtensions.DefaultCorsPolicyName,
                policy => { policy.AddDefaultCors(); });
        });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        CurrentEnvironment environment,
        IConfiguration configuration,
        JwtSettings jwtSettings)
    {
        var useCognito = configuration.GetValue<bool>("JwtSettings:UseCognito");

        if (useCognito)
        {
            var region = configuration["AWS:Region"];
            var userPoolId = configuration["AWS:UserPoolId"];
            var audience = configuration["AWS:ClientId"];
            var authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = !environment.IsLocal();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false, // Cognito valida la firma internamente
                    NameClaimType = "email",
                    RoleClaimType = "cognito:groups"
                };
            });
        }
        else
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    NameClaimType = "email",
                    RoleClaimType = "role"
                };
            });
        }

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddCommonSwaggerServices<T>(this IServiceCollection services)
    {
        services.AddSwaggerExamplesFromAssemblyOf<T>();

        return services;
    }

    public static SwaggerGenOptions AddCommonSwaggerOptions(this SwaggerGenOptions options)
    {
        options.ExampleFilters();
        options.DocumentFilter<AddDefaultAuthTokenFilter>();

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] {}
            }
        });

        return options;
    }
}
