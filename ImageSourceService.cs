using System;
using System.IO;
using System.Threading.Tasks;

namespace FakePhoto
{
    public enum ImageType
    {
        Jpg,
        Png
    }

    public interface IImageSourceService
    {
        void CreateImageDirectory(string dirName);
        Task WriteImageFileAsync(string imageName, byte[] imageContent);
        IImageReader ReadImageFile(string imageName);
        ImageType ImageType { get; set; }
    }

    public interface IImageReader
    {
    }

    public class ImageSourceService : IImageSourceService
    {
        private string DirPath { get; set; }

        public ImageType ImageType { get; set; }

        public void CreateImageDirectory(string dirName)
        {
            var baseFolder = Environment.CurrentDirectory;
            DirPath = Path.Combine(baseFolder!, dirName);
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(dirName);
            }
        }

        public async Task WriteImageFileAsync(string imageName, byte[] imageContent)
        {
            await CheckAbsolutePathName(imageName, async s =>
            {
                using var image = File.Create(s);
                await image.WriteAsync(imageContent, 0, imageContent.Length);
            });
        }

        public IImageReader ReadImageFile(string imageName)
        {
            throw new NotImplementedException();
        }

        private async Task CheckAbsolutePathName(string imageName, Func<string, Task> imageAction)
        {
            var imageFullName = string.Join('.', imageName, ImageType.ToString().ToLower());
            var imagePath = Directory.Exists(DirPath)
                ? Path.Combine(DirPath, imageFullName)
                : string.Empty;
            if (!string.IsNullOrEmpty(imagePath) && !File.Exists(imagePath))
            {
                await imageAction(imagePath);
            }
        }
    }
}