using Imagegram.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace Imagegram.Api.Controllers
{
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// The method returns image file in .jpg format.
        /// </summary>        
        /// <response code="200">Image file.</response>
        /// <response code="400">Query parameter has incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        /// <response code="404">Image doesn't exist.</response> 
        [HttpGet("images/{fileName}")]
        public ActionResult Get([FromRoute] string fileName)
        {
            if (!_imageService.Exists(fileName))
            {
                return NotFound("Image doesn't exist.");
            }

            var imageStream = _imageService.Get(fileName);            
            Response.Headers[HeaderNames.ContentDisposition] = new ContentDisposition
            {
                FileName = fileName,
                DispositionType = DispositionTypeNames.Attachment
            }.ToString();

            return File(imageStream, "image/jpg");
        }
    }
}
