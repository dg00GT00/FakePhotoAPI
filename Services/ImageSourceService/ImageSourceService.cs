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
            DirPath = Path.Combine(Environment.CurrentDirectory, dirName);
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(dirName);
            }
        }

        public async Task<string> GetImageFullPath(string imageName)
        {
            return await this.CheckAbsolutePathName(imageName,
                s => { return Task.FromResult(Path.Combine(DirPath, s)); });
        }

        public async Task<string> WriteImageFileAsync(string imageName, byte[] imageContent)
        {
            return await this.CheckAbsolutePathName(imageName, async s =>
            {
                using var image = File.Create(s);
                await image.WriteAsync(imageContent, 0, imageContent.Length);
                return await GetImageFullPath(s);
            });
        }

        public async Task<string> WriteImageFileAsync(Tuple<int, int> dimensions, byte[] imageContent)
        {
            var parsedFileName = string.Format("{0}x{1}", dimensions.Item1, dimensions.Item2);
            return await WriteImageFileAsync(parsedFileName, imageContent);
        }

        public string GenerateImageTag(string imagePath)
        {
            return ImageBuilder.BuildImageTag(imagePath);
        }
    }
}