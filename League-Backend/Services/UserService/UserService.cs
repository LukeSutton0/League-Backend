using League_Backend.Helpers;
using League_Backend.Models;
using League_Backend.Models.RiotAccountPuuidResponses;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace League_Backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserService> _logger;
        private readonly string _riotApiKey;

        public UserService(ILogger<UserService> logger, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _riotApiKey = config["APIKeys:RiotApi"] ?? throw new Exception("API Key can't be null");
        }

        public async Task<ServiceResult<string>> GetPuuidAsync(string gameName, string tagLine)
        {
            string encodedGameName = Uri.EscapeDataString(gameName);
            tagLine = TagLineHelper.TryStripHashtagFromTagLine(tagLine);
            string encodedTagLine = Uri.EscapeDataString(tagLine);

            string url = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{encodedGameName}/{encodedTagLine}";
            HttpClient httpClient = _httpClientFactory.CreateClient("RiotApiClient");
            HttpResponseMessage response = await httpClient.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                RiotErrorResponse? riotErrorResponse;
                try
                {
                    riotErrorResponse = JsonSerializer.Deserialize<RiotErrorResponse>(json);
                    string message = riotErrorResponse?.status?.message ?? "Unknown Riot error";
                    return ServiceResult<string>.Failure($"Riot API returned {(int)response.StatusCode} ({response.StatusCode}), {message}", response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to deserialise error response {ex}", ex);
                    return ServiceResult<string>.Failure($"Riot API returned {(int)response.StatusCode} ({response.StatusCode})", response.StatusCode);
                }
            }

            RiotAccountPuuidSuccessResponse? riotSuccessResponse;
            try
            {
                riotSuccessResponse = JsonSerializer.Deserialize<RiotAccountPuuidSuccessResponse>(json);
            }
            catch(Exception ex)
            {
                _logger.LogError("Failed to deserialise success response {ex}", ex);  
                return ServiceResult<string>.NotFound("PUUID not found for given Riot Name + Tagline combo");
            }
            if (riotSuccessResponse?.puuid == null)
            {
                return ServiceResult<string>.NotFound("PUUID not found for given Riot Name + Tagline combo");
            }
            return ServiceResult<string>.Success(riotSuccessResponse.puuid);
        }
    }
}
