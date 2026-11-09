namespace BuildingBlocks.Authentication;

public static class EshopClaimTypes
{
    public const string UserId = "eshop.sub";
    public const string Name = "eshop.name";
    public const string Role = "eshop.role";
}

public static class EshopPolicies
{
    public const string Authenticated = "authenticated";
    public const string Admin = "admin";
}
