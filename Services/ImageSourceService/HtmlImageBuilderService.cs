using System.IO;
using System.Text.Encodings.Web;
using FakePhoto.Services.ImageSourceService.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FakePhoto.Services.ImageSourceService
{
    public class HtmlImageBuilderService : IImageBuilderService
    {
        public string BuildImageTag(string imagePath)
        {
            var tag = new TagBuilder("img") {TagRenderMode = TagRenderMode.SelfClosing};
            tag.MergeAttribute("src", imagePath);
            tag.MergeAttribute("alt", "fake image placeholder");
            using var stringWriter = new StringWriter();
            tag.WriteTo(stringWriter, HtmlEncoder.Default);
            return stringWriter.ToString();
        }
    }
}