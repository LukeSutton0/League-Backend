using League_Backend.Models;
using System.Text.Json;

namespace League_Backend.Services.MatchService
{
    public class MatchService : IMatchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MatchService> _logger;

        public MatchService(ILogger<MatchService> logger, IHttpClientFactory httpClientFactory, IConfiguration config) {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ServiceResult<List<string>>> GetLast100MatchIdsAsync(string userPuuid)
        {
            string encodedUserPuuid = Uri.EscapeDataString(userPuuid);
            string url = $"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/{encodedUserPuuid}/ids?start=0&count=100";
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
                    return ServiceResult<List<string>>.Failure($"Riot API returned {(int)response.StatusCode} ({response.StatusCode}), {message}", response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to deserialise error response {ex}", ex);
                    return ServiceResult<List<string>>.Failure($"Riot API returned {(int)response.StatusCode} ({response.StatusCode})", response.StatusCode);
                }
            }
            List<string>? matchIds; //returns raw ids
            try
            {
                matchIds = JsonSerializer.Deserialize<List<string>>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to deserialise success response {ex}", ex);
                return ServiceResult<List<string>>.NotFound("PUUID not found / incorrect");
            }
            return ServiceResult<List<string>>.Success(matchIds ?? []);
        }

        public async Task<ServiceResult<string>> GetUserProfileStats(string userPuuid)
        {
            string encodedUserPuuid = Uri.EscapeDataString(userPuuid);
            string url = $"https://euw1.api.riotgames.com/lol/league/v4/entries/by-puuid/{encodedUserPuuid}";
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
            List<UserProfileStatResponse>? stats;
            try
            {
                stats = await response.Content.ReadFromJsonAsync<List<UserProfileStatResponse>>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to deserialise success response {ex}", ex);
                return ServiceResult<string>.NotFound("PUUID not found / incorrect");
            }
            return ServiceResult<string>.Success(JsonSerializer.Serialize(stats));
        }

        public void AddUserToStartTracking(string userPuuid)
        {
            

        }
    }
}