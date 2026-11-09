using System.Security.Claims;

namespace BuildingBlocks.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(EshopClaimTypes.UserId)
        ?? throw new UnauthorizedAccessException("Missing user id claim.");

    // Keycloak sub, Cognito sub and Entra oid are all GUIDs
    public static Guid GetUserGuid(this ClaimsPrincipal user) => Guid.Parse(user.GetUserId());

    public static string? GetUserName(this ClaimsPrincipal user) =>
        user.FindFirstValue(EshopClaimTypes.Name);
}