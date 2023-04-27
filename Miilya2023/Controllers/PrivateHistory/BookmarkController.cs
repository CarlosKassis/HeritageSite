
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Miilya2023.Shared;
    using System;
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    [ApiController]
    [Route("api/PrivateHistory/[Controller]")]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService _imageService;

        public BookmarkController(IBookmarkService bookmarkService)
        {
            _imageService = bookmarkService;
        }

        [HttpGet]
        [Route("Add/{index:int?}")]
        public async Task<IActionResult> AddBookmark(int? index)
        {
            if (index == null)
            {
                throw new ArgumentException("No history post index was specified");
            }

            await _imageService.AddBookmark(HttpContext.Items["User"] as UserDocument, index.Value);
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

            await _imageService.RemoveBookmark(HttpContext.Items["User"] as UserDocument, index.Value);
            return Ok();
        }
    }
}
