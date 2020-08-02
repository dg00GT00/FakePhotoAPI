using System;
using System.Threading.Tasks;
using FakePhoto.Models;
using FakePhoto.Services;
using FakePhoto.Services.ImageSourceService;
using FakePhoto.Services.ImageSourceService.Interfaces;
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
            _fakePhotoService = fakePhotoService;
            _imageSourceService = imageSourceService;
        }

        [HttpGet("{width}x{height}")]
        public async Task<IActionResult> Get([FromRoute] DimensionsModel dimensionsModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var dimensions = new Tuple<int, int>(dimensionsModel.Width, dimensionsModel.Height);
            var result = await _fakePhotoService.GetBytePhotoByDimensions(dimensions);
            await _imageSourceService.WriteImageFileAsync(dimensions, result);
            return Ok();
        }
    }
}
