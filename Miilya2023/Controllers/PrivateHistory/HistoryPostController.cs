
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using AutoMapper;
    using static Miilya2023.Services.Utils.DocumentsExternal;
    using static Miilya2023.Services.Utils.Documents;

    [ApiController]
    [Route("api/PrivateHistory/[Controller]")]
    public class HistoryPostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHistoryPostService _historyPostService;
        private readonly IUserService _userService;
        private readonly IBookmarkService _bookmarkService;

        public HistoryPostController(IMapper mapper, IHistoryPostService historyPostService, IUserService userService, IBookmarkService bookmarkService)
        {
            _mapper = mapper;
            _historyPostService = historyPostService;
            _userService = userService;
            _bookmarkService = bookmarkService;
        }

        [HttpGet]
        [Route("{startIndex:int?}")]
        public async Task<IActionResult> GetHistoryPostsBatchStartingFromIndex(int? startIndex)
        {
            var user = Request.HttpContext.Items["User"] as UserDocument;

            var historyPosts = await _historyPostService.GetFirstBatchLowerEqualThanIndex(startIndex, batchSize: 8);
            var bookmarkedPosts = (await _bookmarkService.GetUserBookmarks(user))?.BookmarkedHistoryPostsIndexes?.ToHashSet();

            return Content(JsonConvert.SerializeObject(historyPosts.Select(historyPost =>
            {
                var historyPostExternal = _mapper.Map<HistoryPostDocumentExternal>(historyPost);
                historyPostExternal.MyPost = historyPost.UserId == user.Id;
                historyPostExternal.Bookmarked = bookmarkedPosts?.Contains(historyPost.Index) ?? false;
                return historyPostExternal;
            })));
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitHistoryPost()
        {
            var user = Request.HttpContext.Items["User"] as UserDocument;

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

            await _historyPostService.InsertHistoryPost(user, title, description, image);

            return Ok();
        }

    }
}
