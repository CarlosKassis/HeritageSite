namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    [ApiController]
    [Route("PrivateHistory/[Controller]")]
    public class FamilyController : ControllerBase
    {
        private readonly IFamilyService _familyService;

        public FamilyController(IFamilyService familyService)
        {
            _familyService = familyService;
        }

        [HttpGet]
        [Route("All")]
        public async Task<List<FamilyDocument>> GetAll()
        {
            var results = await _familyService.GetAll();
            return results.ToList();
        }
    }
}
