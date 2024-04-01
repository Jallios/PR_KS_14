namespace api.Options
{
    public record class JwtOptions(
    string Issuer,
    string Audience,
    string Key,
    int ExpirationSeconds
);
}
