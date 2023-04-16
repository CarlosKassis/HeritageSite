
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    [ApiController]
    [Route("PrivateHistory/[Controller]")]
    public class HistoryPostController : ControllerBase
    {
        private readonly IHistoryPostService _historyPostService;

        public HistoryPostController(IHistoryPostService historyPostService)
        {
            _historyPostService = historyPostService;
        }

        [HttpGet]
        public async Task<string> GetImageBatchAfter(int index)
        {
            var results = await _historyPostService.GetFirstBatchGreaterEqualThanIndex(index);
            return JsonConvert.SerializeObject(results);
        }
    }
}
