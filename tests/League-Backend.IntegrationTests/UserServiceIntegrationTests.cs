using League_Backend.Models;
using League_Backend.Services.UserService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace League_Backend.IntegrationTests;

public class UserServiceIntegrationTests
{
    private readonly UserService _userService;

    public UserServiceIntegrationTests()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        string? apiKey = config["APIKeys:RiotApi"];

        IHttpClientFactory factory = new ServiceCollection()
            .AddHttpClient("RiotApiClient", client =>
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", apiKey);
            })
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IHttpClientFactory>();

        NullLogger<UserService> logger = NullLogger<UserService>.Instance;
        
        _userService = new UserService(logger, factory, config);
    }

    [Fact]
    public async Task GetPuuidAsync_WithRealApi_ReturnsValidPuuid()
    {
        ServiceResult<string> result = await _userService.GetPuuidAsync("Spookylukee", "EUW");
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
