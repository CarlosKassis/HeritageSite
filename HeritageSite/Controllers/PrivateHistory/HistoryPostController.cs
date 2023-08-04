
namespace HeritageSite.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using HeritageSite.Services.Abstract;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using AutoMapper;
    using static HeritageSite.Services.Utils.DocumentsExternal;
    using static HeritageSite.Services.Utils.Documents;
    using System.Collections.Generic;
    using System.Globalization;

    [ApiController]
    [Route("api/PrivateHistory/[Controller]")]
    public class HistoryPostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHistoryPostService _historyPostService;
        private readonly IUserService _userService;

        public HistoryPostController(IMapper mapper, IHistoryPostService historyPostService, IUserService userService)
        {
            _mapper = mapper;
            _historyPostService = historyPostService;
            _userService = userService;
        }


        [HttpPost("{startIndex:int?}")]
        public async Task<IActionResult> GetHistoryPostsBatchStartingFromIndex(int? startIndex)
        {
            var user = Request.HttpContext.Items["User"] as UserDocument;
            var searchText = Request.Form["searchText"].FirstOrDefault();

            var historyPosts = await _historyPostService.GetFirstBatchLowerEqualThanIndex(startIndex, batchSize: 20, searchText);
            var bookmarkedPosts = (await _historyPostService.GetUserBookmarkPostIndexes(user.Id))?.ToHashSet();

            return Content(JsonConvert.SerializeObject(historyPosts.Select(historyPost =>
            {
                var historyPostExternal = _mapper.Map<HistoryPostDocumentExternal>(historyPost);
                historyPostExternal.Control = user.IsAdmin ? 2 : historyPost.PosterId == user.Id ? 1 : 0;
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

            var titleString = Request.Form["title"].FirstOrDefault();
            var descriptionString = Request.Form["description"].FirstOrDefault();
            var imageDateString = Request.Form["imageDate"].FirstOrDefault();

            if (!DateOnly.TryParseExact(imageDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var imageDate))
            {
                throw new InvalidOperationException("Invalid image date");
            }

            await _historyPostService.InsertHistoryPost(user.Id, titleString, descriptionString, image, imageDate);

            return Ok();
        }

        [HttpPost("Delete/{index:int?}")]
        public async Task<IActionResult> DeleteHistoryPost(int? index)
        {
            var user = Request.HttpContext.Items["User"] as UserDocument;
            await _historyPostService.DeleteHistoryPost(user.Id, index.Value, user.IsAdmin);

            return Ok();
        }
    }
}
