using League_Backend.Models;
using League_Backend.Services.UserService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace League_Backend.IntegrationTests;

public class UserServiceIntegrationTests
{
    private readonly UserService _sut;
    private readonly string _mockAPIKey = "RGAPI-246162b8-5827-456a-95ec-d230debca576";

    public UserServiceIntegrationTests()
    {
        IHttpClientFactory factory = new ServiceCollection()
            .AddHttpClient("RiotApiClient", client =>
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", _mockAPIKey);
            })
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IHttpClientFactory>();

        NullLogger<UserService> logger = NullLogger<UserService>.Instance;

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "APIKeys:RiotApi", _mockAPIKey }
            })
            .Build();

        _sut = new UserService(logger, factory, config);
    }

    [Fact]
    public async Task GetPuuidAsync_WithRealApi_ReturnsValidPuuid()
    {
        ServiceResult<string> result = await _sut.GetPuuidAsync("Spookylukee", "EUW");
        if (result.IsSuccessful)
        {
            if (result.Data == "CYy2rN8aXfI-2_No11eSk7Ap6dUpBtaRf3AQOgyJiv_g5bfv9qfc3NzAs_E05Rh-5DdZe4ZSZoKyXA")//My id
            {  
                Assert.True(result.IsSuccessful);
            }
            else
            {
                Assert.Fail("Incorrect user Id");
            }
        }
        Assert.False(string.IsNullOrEmpty(result.Data));
    }
}
