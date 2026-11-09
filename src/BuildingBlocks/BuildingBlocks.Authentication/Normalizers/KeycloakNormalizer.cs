using System.Security.Claims;
using System.Text.Json;

namespace BuildingBlocks.Authentication.Normalizers;

public sealed class KeycloakNormalizer : IClaimsNormalizer
{
    public IEnumerable<Claim> Normalize(ClaimsPrincipal principal)
    {
        if (principal.FindFirst("sub")?.Value is { } sub)
            yield return new Claim(EshopClaimTypes.UserId, sub);

        if (principal.FindFirst("preferred_username")?.Value is { } name)
            yield return new Claim(EshopClaimTypes.Name, name);

        // realm_access is a JSON object: {"roles":["user","admin"]}
        var realmAccess = principal.FindFirst("realm_access")?.Value;
        if (realmAccess is null) yield break;

        using var doc = JsonDocument.Parse(realmAccess);
        if (doc.RootElement.TryGetProperty("roles", out var roles))
            foreach (var role in roles.EnumerateArray())
                yield return new Claim(EshopClaimTypes.Role, role.GetString()!);
    }

}

