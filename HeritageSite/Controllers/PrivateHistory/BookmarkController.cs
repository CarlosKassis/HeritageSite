
namespace HeritageSite.Controllers.PrivateHistory
{
    using HeritageSite.Services.Abstract;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    [ApiController]
    [Route("api/PrivateHistory/[Controller]")]
    public class BookmarkController : ControllerBase
    {
        private readonly IHistoryPostService _historyPostService;

        public BookmarkController(IHistoryPostService historyPostService)
        {
            _historyPostService = historyPostService;
        }

        [HttpGet]
        [Route("Add/{index:int?}")]
        public async Task<IActionResult> AddBookmark(int? index)
        {
            if (index == null)
            {
                throw new ArgumentException("No history post index was specified");
            }

            var user = HttpContext.Items["User"] as UserDocument;
            await _historyPostService.AddBookmark(user.Id, index.Value);
            return Ok();
        }

        [HttpGet]
        [Route("Remove/{index:int?}")]
        public async Task<IActionResult> RemoveBookmark(int? index)
        {
            if (index == null)
            {
                throw new ArgumentException("No history post index was specified");
            }

            var user = HttpContext.Items["User"] as UserDocument;
            await _historyPostService.RemoveBookmark(user.Id, index.Value);
            return Ok();
        }
    }
}
