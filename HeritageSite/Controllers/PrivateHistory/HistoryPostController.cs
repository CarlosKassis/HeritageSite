﻿
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


        [HttpPost("{startIndex:int?}")]
        public async Task<IActionResult> GetHistoryPostsBatchStartingFromIndex(int? startIndex)
        {
            var user = Request.HttpContext.Items["User"] as UserDocument;
            var searchText = Request.Form["searchText"].FirstOrDefault();

            var historyPosts = await _historyPostService.GetFirstBatchLowerEqualThanIndex(startIndex, batchSize: 20, searchText);
            var bookmarkedPosts = (await _bookmarkService.GetUserBookmarks(user))?.Select(bookmark => bookmark.HistoryPostIndex)?.ToHashSet();

            return Content(JsonConvert.SerializeObject(historyPosts.Select(historyPost =>
            {
                var historyPostExternal = _mapper.Map<HistoryPostDocumentExternal>(historyPost);
                historyPostExternal.Control = user.IsAdmin ? 2 : historyPost.UserId == user.Id ? 1 : 0;
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

        [HttpPost("Delete/{index:int?}")]
        public async Task<IActionResult> DeleteHistoryPost(int? index)
        {
            var user = Request.HttpContext.Items["User"] as UserDocument;
            await _historyPostService.DeleteHistoryPost(user, index.Value);

            return Ok();
        }
    }
}
