using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FakePhoto.Models
{
    [BindProperties(SupportsGet = true)]
    public class DimensionsModel
    {
        [Required]
        [BindRequired]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public int Width { get; set; }

        [Required]
        [BindRequired]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public int Height { get; set; }
    }
}