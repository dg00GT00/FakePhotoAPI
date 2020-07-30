using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakePhoto.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FakePhotoController : ControllerBase
    {
        private readonly IFakePhotoService _fakePhotoService;
        private readonly IImageSourceService _imageSourceService;

        public FakePhotoController(IFakePhotoService fakePhotoService, IImageSourceService imageSourceService)
        {
            imageSourceService.ImageType = ImageType.Png;
            imageSourceService.CreateImageDirectory("FakeImagesDir");
            _fakePhotoService = fakePhotoService;
            _imageSourceService = imageSourceService;
        }

        [HttpGet("/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var result = await _fakePhotoService.GetBytePhotoByDimensions(new Tuple<int, int>(0, 300));
            await _imageSourceService.WriteImageFileAsync("other", result);
            return Ok();
        }
    }
}