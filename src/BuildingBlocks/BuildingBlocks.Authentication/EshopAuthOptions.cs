namespace BuildingBlocks.Authentication;

public sealed class EshopAuthOptions
{
    public const string SectionName = "Authentication";
    public string Provider { get; set; } = "Keycloak";
    public string Authority { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public bool RequireHttpsMetadata { get; set; } = false;
}
