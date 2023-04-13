namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Constants;
    using Miilya2023.Services.Abstract;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("PrivateHistory/[Controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<List<string>> GetImageBatchAfter(int index)
        {
            var results = await _imageService.GetFirstBatchGreaterThanIndex(index);
            return results.Select(image => $"{PrivateHistoryConstants.MediaUrlPrefix}/{image.Name}").ToList();
        }
    }
}
