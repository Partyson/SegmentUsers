using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Extensions;

public static class HttpClientFactoryExtensions
{
    public static HttpClient? CreateAuthorizedHttpClient(this IHttpClientFactory factory, HttpContext context, ApiSettings apiSettings)
    {
        var token = context.User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;
        if (string.IsNullOrEmpty(token))
            return null;
        
        var client = factory.CreateClient();
        client.BaseAddress = new Uri(apiSettings.BaseUrl);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}