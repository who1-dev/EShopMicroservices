using BuildingBlocks.Authentication.Normalizers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace BuildingBlocks.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddEshopAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var opts = configuration.GetSection(EshopAuthOptions.SectionName).Get<EshopAuthOptions>()
            ?? throw new InvalidOperationException("Missing 'Authentication' configuration section.");

        if (string.IsNullOrWhiteSpace(opts.Authority))
            throw new InvalidOperationException("Authentication:Authority is required.");

        IClaimsNormalizer normalizer = opts.Provider switch
        {
            "Keycloak" => new KeycloakNormalizer(),
            "Cognito" => new CognitoNormalizer(),
            "EntraId" => new EntraIdNormalizer(),
            _ => throw new InvalidOperationException($"Unknown auth provider '{opts.Provider}'.")
        };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                // Downloads /.well-known/openid-configuration -> issuer + signing keys
                o.Authority = opts.Authority;
                o.RequireHttpsMetadata = opts.RequireHttpsMetadata;

                // Don't rename "sub" -> long Microsoft URI claim types
                o.MapInboundClaims = false;

                // User.Identity.Name and User.IsInRole() read our normalized claims
                o.TokenValidationParameters.NameClaimType = EshopClaimTypes.Name;
                o.TokenValidationParameters.RoleClaimType = EshopClaimTypes.Role;

                if (opts.Provider == "Cognito")
                    o.TokenValidationParameters.ValidateAudience = false; // no 'aud' in Cognito access tokens
                else
                    o.Audience = opts.Audience;

                o.Events = new JwtBearerEvents
                {
                    // Runs after signature, issuer, lifetime and audience have been validated
                    OnTokenValidated = ctx =>
                    {
                        var principal = ctx.Principal!;

                        if (opts.Provider == "Cognito" &&
                            principal.FindFirst("client_id")?.Value != opts.Audience)
                        {
                            ctx.Fail("Invalid client_id.");
                            return Task.CompletedTask;
                        }

                        // ToList(): materialize before we modify the identity's claims
                        var normalized = normalizer.Normalize(principal);
                        if (!normalized.Any(c => c.Type == EshopClaimTypes.UserId))
                        {
                            ctx.Fail("Token has no user id claim.");
                            return Task.CompletedTask;
                        }

                        ((ClaimsIdentity)principal.Identity!).AddClaims(normalized);
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(a =>
        {
            a.AddPolicy(EshopPolicies.Authenticated, p => p.RequireAuthenticatedUser());
            a.AddPolicy(EshopPolicies.Admin, p => p.RequireRole("admin"));
        });

        return services;
    }
}