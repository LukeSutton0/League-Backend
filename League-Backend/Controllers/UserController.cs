using League_Backend.Helpers;
using League_Backend.Models;
using League_Backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace League_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger,  IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }


        [HttpPost(Name = "GetUserPUUID")]
        public async Task<IActionResult> GetUserPUUID(string gameName, string tagLine)
        {
            ServiceResult<string> serviceResult = await _userService.GetPuuidAsync(gameName, tagLine);
            if (!serviceResult.IsSuccessful)
            {
                return StatusCode((int)serviceResult.StatusCode, serviceResult.Errors);
            }
            return Ok(serviceResult.Data);
        }
    }
}
