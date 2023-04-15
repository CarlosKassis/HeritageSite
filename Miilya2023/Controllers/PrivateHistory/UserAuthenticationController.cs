
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[Controller]")]
    public class UserAuthenticationController : ControllerBase
    {
        private readonly IHistoryPostService _historyPostService;

        public UserAuthenticationController(IHistoryPostService historyPostService)
        {
            _historyPostService = historyPostService;
        }

        /// <summary>
        /// Receives a Google login response and returns a login jwt
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Google/Login")]
        public async Task<string> GoogleLogin()
        {
            return await Task.FromResult("ASD");
        }
    }
}
