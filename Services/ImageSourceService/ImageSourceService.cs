using System;
using System.IO;
using System.Threading.Tasks;
using FakePhoto.Extensions;
using FakePhoto.Services.ImageSourceService.Interfaces;

namespace FakePhoto.Services.ImageSourceService
{
    public class ImageSourceService : IImageSourceService
    {
        private IImageBuilderService ImageBuilder { get; }
        public ImageType ImageType { get; set; }
        public string DirPath { get; set; }

        public ImageSourceService(IImageBuilderService imageBuilder, string dirName, ImageType imageType)
        {
            ImageBuilder = imageBuilder;
            ImageType = imageType;
            CreateImageDirectory(dirName);
        }

        private void CreateImageDirectory(string dirName)
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
            await this.CheckAbsolutePathName(imageName, async s =>
            {
                using var image = File.Create(s);
                await image.WriteAsync(imageContent, 0, imageContent.Length);
                return Task.CompletedTask;
            });
        }

        public async Task WriteImageFileAsync(Tuple<int, int> dimensions, byte[] imageContent)
        {
            var parsedFileName = string.Format("{0}x{1}", dimensions.Item1, dimensions.Item2);
            await WriteImageFileAsync(parsedFileName, imageContent);
        }

        public async Task<string> ImageBuilderAsync(string imagePath)
        {
            return await this.CheckAbsolutePathName(imagePath, s =>
            {
                return Task.FromResult(ImageBuilder.BuildImageTag(s));
            });
        }
    }
}