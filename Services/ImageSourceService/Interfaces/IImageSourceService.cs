using System;
using System.Threading.Tasks;

namespace FakePhoto.Services.ImageSourceService.Interfaces
{
    public interface IImageSourceService
    {
        ImageType ImageType { get; set; }
        string DirPath { get; set; }
        Task WriteImageFileAsync(string imageName, byte[] imageContent);
        Task WriteImageFileAsync(Tuple<int, int> dimensions, byte[] imageContent);
        Task<string> ImageBuilderAsync(string imagePath);
    }
}