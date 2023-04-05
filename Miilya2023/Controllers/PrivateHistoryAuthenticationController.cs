using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miilya2023.Controllers
{
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
