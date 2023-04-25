using System;
using System.Collections.Generic;
using System.Linq;

namespace Miilya2023.Shared
{
    public static class Validation
    {
        private static readonly HashSet<string> _supportedImageFileExtensions = new HashSet<string>
        {
            "jpg", "jpeg", "png", "tiff", "gif", "bmp", "webp"
        };

        public static string EnsureValidSupportedImageFileName(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException("Filename is empty");
            }

            var parts = filename.Split('.');
            if (parts.Length <= 1)
            {
                throw new ArgumentException("Invalid filename");
            }

            var fileExtension = parts.Last();
            if (!_supportedImageFileExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"Unsupported file type: {fileExtension}");
            }

            if (filename.Contains("..") || filename.Contains("/") || filename.Contains("\\"))
            {
                throw new ArgumentException("Get out of here");
            }

            return fileExtension;
        }
    }
}
