
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;

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
            var results = await _historyPostService.GetFirstBatchLowerEqualThanIndex(startIndex, batchSize: 8);
            return Content(JsonConvert.SerializeObject(results));
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitHistoryPost()
        {
            var formFile = Request.Form.Files["image"];
            if (formFile == null)
            {
                throw new ArgumentException("Image wasn't supplied");
            }

            Image image = null;
            try
            {
                using var stream = formFile.OpenReadStream();
                image = await Image.LoadAsync(stream);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            var title = Request.Form["title"].FirstOrDefault();
            var description = Request.Form["description"].FirstOrDefault();

            await _historyPostService.InsertHistoryPost(title, description, image);

            return Ok();
        }

    }
}
