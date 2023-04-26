
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using System.IdentityModel.Tokens.Jwt;
    using AutoMapper;
    using static Miilya2023.Services.Utils.DocumentsExternal;

    [ApiController]
    [Route("PrivateHistory/[Controller]")]
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

        [HttpGet]
        [Route("{startIndex:int?}")]
        public async Task<IActionResult> GetHistoryPostsBatchStartingFromIndex(int? startIndex)
        {
            var jwt = Request.Headers["Authorization"].ToString();
            var user = await _userService.GetUserWithLoginJwt(jwt);

            var historyPosts = await _historyPostService.GetFirstBatchLowerEqualThanIndex(startIndex, batchSize: 8);
            return Content(JsonConvert.SerializeObject(historyPosts.Select(historyPost =>
            {
                var historyPostExternal = _mapper.Map<HistoryPostDocumentExternal>(historyPost);
                historyPostExternal.MyPost = historyPost.UserId == user.Id;
                return historyPostExternal;
            })));
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitHistoryPost()
        {
            var jwt = Request.Headers["Authorization"].First();
            var user = await _userService.GetUserWithLoginJwt(jwt);

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
