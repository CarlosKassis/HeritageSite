
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Newtonsoft.Json;
    using System.Linq;
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
        [Route("{startIndex:int?}")]
        public async Task<IActionResult> GetHistoryPostsBatchStartingFromIndex(int? startIndex)
        {
            var results = await _historyPostService.GetFirstBatchLowerEqualThanIndex(startIndex);
            return Content(JsonConvert.SerializeObject(results));
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitHistoryPost()
        {
            // TODO: assert image
            var title = Request.Form["title"].FirstOrDefault();
            var description = Request.Form["description"].FirstOrDefault();
            var image = Request.Form.Files["image"];
            await _historyPostService.InsertHistoryPost(title, description, image);
            return Ok();
        }

    }
}
