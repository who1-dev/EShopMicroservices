using System.Security.Claims;

namespace BuildingBlocks.Authentication.Normalizers;

public sealed class CognitoNormalizer : IClaimsNormalizer
{
    public IEnumerable<Claim> Normalize(ClaimsPrincipal p)
    {
        if (p.FindFirst("sub")?.Value is { } sub)
            yield return new Claim(EshopClaimTypes.UserId, sub);

        if (p.FindFirst("username")?.Value is { } name)
            yield return new Claim(EshopClaimTypes.Name, name);

        foreach (var group in p.FindAll("cognito:groups"))
            yield return new Claim(EshopClaimTypes.Role, group.Value);
    }
}