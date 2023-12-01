using System.Net.Http.Headers;
using System.Net.Http.Json;
using OpenIddict.Abstractions;

namespace ServersTests;

public static class MyClient
{
    private const string AuthServerHost = "https://localhost:7225";
    private const string ResourceServerHost = "https://localhost:7243";
    
    // private const string AuthServerHost = "http://localhost:5147";
    // private const string ResourceServerHost = "http://localhost:5165";
        
    /// <summary>
    /// 向 AuthServer 取得 token
    /// </summary>
    /// <param name="grantType"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<string> GetTokenAsync(string grantType, string clientId, string clientSecret)
    {
        // POST token endpoint
        var request = new HttpRequestMessage(HttpMethod.Post, $"{AuthServerHost}/connect/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = grantType,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        });

        using var client = new HttpClient();
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        var payload = await response.Content.ReadFromJsonAsync<OpenIddictResponse>();
        if (!string.IsNullOrEmpty(payload.Error))
        {
            throw new InvalidOperationException("An error occurred while retrieving an access token.");
        }

        return payload.AccessToken; // 將取得的 access token 回傳
    }

    /// <summary>
    /// 向 Resource Server 取得資料
    /// </summary>
    /// <param name="token"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task<string> GetResourceAsync(string token, string action)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{ResourceServerHost}/{action}");
        // 帶入從 AuthServer 取得的 access token
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var client = new HttpClient();
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}