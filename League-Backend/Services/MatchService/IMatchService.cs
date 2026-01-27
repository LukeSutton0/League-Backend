using League_Backend.Models;

namespace League_Backend.Services.MatchService
{
    public interface IMatchService
    {
        public Task<ServiceResult<List<string>>> GetLast100MatchIdsAsync(string userPuuid);
        public Task<ServiceResult<string>> GetUserProfileStats(string userPuuid);

    }
}
