
namespace Miilya2023.Controllers.PrivateHistory
{
    using Microsoft.AspNetCore.Mvc;
    using Miilya2023.Services.Abstract;
    using Miilya2023.Shared;
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
        [Route("LowRes/{imageName}")]
        public async Task<IActionResult> GetImageLowResolution(string imageName)
        {
            Validation.EnsureValidSupportedImageFileName(imageName);

            var image = await _imageService.GetImageLowResolution(imageName);
            return File(image, "image/jpeg");
        }

        [HttpGet]
        [Route("{imageName}")]
        public async Task<IActionResult> GetImage(string imageName)
        {
            Validation.EnsureValidSupportedImageFileName(imageName);

            var image = await _imageService.GetImage(imageName);
            return File(image, "image/jpeg");
        }
    }
}
