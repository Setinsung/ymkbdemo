using Microsoft.Kiota.Abstractions.Authentication;

namespace YmKB.UI.Providers;

public class KiotaTokenProvider : IAccessTokenProvider
{
    public AllowedHostsValidator AllowedHostsValidator { get; }

    public Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult("<insert_bearer_token>");
    }
}