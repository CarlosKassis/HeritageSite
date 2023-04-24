﻿using Microsoft.AspNetCore.Mvc;
using Miilya2023.Constants;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Miilya2023.Services.Abstract
{
    public class ImageService : IImageService
    {
        private const int _maxImageWidth = 600;
        private static readonly ConcurrentDictionary<string, byte[]> _imageCache = new ConcurrentDictionary<string, byte[]>();
        private static readonly ConcurrentDictionary<string, byte[]> _lowResImageCache = new ConcurrentDictionary<string, byte[]>();

        public async Task<byte[]> GetImage(string imageName)
        {
            return await GetImageArray(imageName, lowResolution: false);
        }

        public async Task<byte[]> GetImageLowResolution(string imageName)
        {
            return await GetImageArray(imageName, lowResolution: true);
        }

        private async Task<byte[]> GetImageArray(string imageName, bool lowResolution)
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

            using var image = Image.Load(imagePath);

            if (lowResolution)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(Math.Min(image.Width, _maxImageWidth), 0),
                    Mode = ResizeMode.Max
                }));
            }

            var jpegEncoder = new JpegEncoder();

            // Return the resized image
            using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, jpegEncoder);
            outputStream.Seek(0, SeekOrigin.Begin);
            byte[] imageAsArray =  outputStream.ToArray();

            imageCache.TryAdd(imageName, imageAsArray);
            return imageAsArray;
        }
    }
}