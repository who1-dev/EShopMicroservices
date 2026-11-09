using System.Security.Claims;

namespace BuildingBlocks.Authentication.Normalizers;

public interface IClaimsNormalizer
{
    IEnumerable<Claim> Normalize(ClaimsPrincipal principal);
}
