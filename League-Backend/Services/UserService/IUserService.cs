using League_Backend.Models;

namespace League_Backend.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResult<string>> GetPuuidAsync(string gameName, string tagLine);
    }
}
