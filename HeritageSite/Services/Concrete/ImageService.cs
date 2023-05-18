

namespace HeritageSite.Services.Abstract
{
    using HeritageSite.Constants;
    using HeritageSite.Shared;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;

    public class ImageService : IImageService
    {
        private const int _maxImageWidth = 720;
        private static readonly ConcurrentDictionary<string, byte[]> _imageCache = new ();
        private static readonly ConcurrentDictionary<string, byte[]> _lowResImageCache = new();

        public async Task<byte[]> GetImage(string imageName)
        {
            Validation.EnsureValidSupportedImageFileName(imageName);
            return await GetImageArray(imageName, lowResolution: false);
        }

        public async Task<byte[]> GetImageLowResolution(string imageName)
        {
            Validation.EnsureValidSupportedImageFileName(imageName);
            return await GetImageArray(imageName, lowResolution: true);
        }

        private static async Task<byte[]> GetImageArray(string imageName, bool lowResolution)
        {
            var imageCache = lowResolution switch
            {
                true => _lowResImageCache,
                false => _imageCache
            };

            if (imageCache.TryGetValue(imageName, out byte[] cachedImageAsArray))
            {
                return cachedImageAsArray;
            }

            var imagePath = Path.Combine(PrivateHistoryConstants.RootPath, "Media", "Images", imageName);

            using var image = await Image.LoadAsync(imagePath);

            if (lowResolution)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(Math.Min(image.Width, _maxImageWidth), 0),
                    Mode = ResizeMode.Max
                }));
            }

            var jpegEncoder = new JpegEncoder();

            // Return the final image
            using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, jpegEncoder);
            outputStream.Seek(0, SeekOrigin.Begin);
            byte[] imageAsArray =  outputStream.ToArray();

            imageCache.TryAdd(imageName, imageAsArray);
            return imageAsArray;
        }
    }
}
