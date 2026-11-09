using System.Security.Claims;

namespace BuildingBlocks.Authentication.Normalizers;

public sealed class EntraIdNormalizer : IClaimsNormalizer
{
    public IEnumerable<Claim> Normalize(ClaimsPrincipal p)
    {
        // oid = the same user across all apps in the tenant (Entra's sub is per-app)
        if (p.FindFirst("oid")?.Value is { } oid)
            yield return new Claim(EshopClaimTypes.UserId, oid);

        if (p.FindFirst("preferred_username")?.Value is { } name)
            yield return new Claim(EshopClaimTypes.Name, name);

        foreach (var role in p.FindAll("roles"))
            yield return new Claim(EshopClaimTypes.Role, role.Value);
    }
}