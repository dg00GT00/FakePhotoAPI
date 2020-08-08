using System;
using System.IO;
using System.Threading.Tasks;
using FakePhoto.Services.ImageSourceService.Interfaces;

namespace FakePhoto.Extensions
{
    public static class CheckPathExtensions
    {
        public static Task<T> CheckAbsolutePathName<T>(this IImageSourceService image, string imageName,
            Func<string, Task<T>> imageAction)
        {
            var imageFullName = ImageNameWithExtension(image, imageName);
            var imagePath = Directory.Exists(image.DirPath)
                ? Path.Combine(image.DirPath, imageFullName)
                : string.Empty;
            if (string.IsNullOrEmpty(imagePath)) throw new FileNotFoundException(imageFullName);
            return imageAction(imagePath);
        }

        public static string ImageNameWithExtension(this IImageSourceService image, string imageName)
        {
            return string.Join('.', imageName, image.ImageType.ToString().ToLower());
        }
    }
}