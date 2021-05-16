using Imagegram.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Imagegram.Api.Controllers
{
    public class ImageController : ControllerBase
    {
        private readonly FileService _fileService;

        public ImageController(FileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// The method returns image file in .jpg format.
        /// </summary>        
        /// <response code="200">Image file.</response>
        /// <response code="400">Query parameter has incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        /// <response code="404">Image doesn't exist.</response> 
        [HttpGet("images/{fileName}")]
        public async Task<ActionResult> Get([FromRoute] string fileName)
        {
            if (!_fileService.Exists(fileName))
            {
                return NotFound("Image doesn't exist.");
            }

            var imageStream = await _fileService.Get(fileName);            
            Response.Headers[HeaderNames.ContentDisposition] = new ContentDisposition
            {
                FileName = fileName,
                DispositionType = DispositionTypeNames.Attachment
            }.ToString();

            return File(imageStream, "image/jpg");
        }
    }
}
