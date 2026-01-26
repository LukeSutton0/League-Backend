using League_Backend.Models;
using System.Text.Json;

namespace League_Backend.Services.MatchService
{
    public class MatchService : IMatchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MatchService> _logger;
        private readonly string _riotApiKey;

        public MatchService(ILogger<MatchService> logger, IHttpClientFactory httpClientFactory, IConfiguration config) {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _riotApiKey = config["APIKeys:RiotApi"] ?? throw new Exception("API Key can't be null");
        }

        public async Task<ServiceResult<List<string>>> GetLast100MatchIdsAsync(string userPuuid)
        {
            string encodedUserPuuid = Uri.EscapeDataString(userPuuid);
            string url = $"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/{encodedUserPuuid}/ids?start=0&count=100";
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, url);
            httpRequestMessage.Headers.Add("X-Riot-Token", _riotApiKey);
            HttpClient httpClient = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
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
    }
}