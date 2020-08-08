using System;
using System.Threading.Tasks;

namespace FakePhoto.Services.ImageSourceService.Interfaces
{
    public interface IImageSourceService
    {
        ImageType ImageType { get; set; }
        string DirPath { get; set; }
        Task<string> GetImageFullPath(string imageName);
        Task<string> WriteImageFileAsync(string imageName, byte[] imageContent);
        Task<string> WriteImageFileAsync(Tuple<int, int> dimensions, byte[] imageContent);
        string GenerateImageTag(string imagePath);
    }
}