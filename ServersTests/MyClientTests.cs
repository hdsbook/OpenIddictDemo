using FluentAssertions;

namespace ServersTests
{
    public class MyClientTests
    {
        [TestCase("client_credentials", "my-console-app", "388D45FA-B36B-4988-BA59-B187D329C207")]
        public async Task GetResource_ByClientCredentials_WorksCorrectly(string grantType, string clientId, string clientSecret)
        {
            // when get access token
            var token = await MyClient.GetTokenAsync(grantType, clientId, clientSecret);
            
            // then assert
            token.Should().NotBeNullOrEmpty();
            
            // when call Weather API
            var resource = await MyClient.GetResourceAsync(token, "WeatherForecast");
            
            // then assert
            resource.Should().NotBeNullOrEmpty();
        }
    }
}