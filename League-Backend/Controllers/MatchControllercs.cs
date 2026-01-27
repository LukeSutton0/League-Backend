using League_Backend.Models;
using League_Backend.Services.MatchService;
using League_Backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace League_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchController: ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ILogger<UserController> _logger;

        public MatchController(ILogger<UserController> logger, IMatchService matchService)
        {
            _logger = logger;
            _matchService = matchService;
        }

        [HttpPost("GetLast100Matches", Name = "GetLast100Matches")]
        public async Task<IActionResult> GetLast100MatchIds(string userPuuid)
        {
            ServiceResult<List<string>> serviceResult = await _matchService.GetLast100MatchIdsAsync(userPuuid);
            if (!serviceResult.IsSuccessful)
            {
                return StatusCode((int)serviceResult.StatusCode, serviceResult.Errors);
            }
            return Ok(serviceResult.Data);
        }

        [HttpPost("StartTrackingUsersLP", Name = "StartTrackingUsersLP")]
        public async Task<IActionResult> StartTrackingUsersLP(string userPuuid)
        {
            var serviceResult = await _matchService.GetUserProfileStats(userPuuid);
            if (!serviceResult.IsSuccessful)
            {
                return StatusCode((int)serviceResult.StatusCode, serviceResult.Errors);
            }
            return Ok(serviceResult.Data);
        }
    }
}
