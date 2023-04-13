

namespace Miilya2023.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("[controller]")]
    public class PrivateHistoryAuthenticationController : ControllerBase
    {
        private readonly ILogger<PrivateHistoryAuthenticationController> _logger;

        public PrivateHistoryAuthenticationController(ILogger<PrivateHistoryAuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "asd";
        }
    }
}
