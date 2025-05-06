using System.Net.Http.Headers;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Handlers
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IStorageService _storageService;

        public JwtAuthorizationMessageHandler(IStorageService storageService)
        {
            _storageService = storageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _storageService.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
