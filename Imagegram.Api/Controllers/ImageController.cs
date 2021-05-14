using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Imagegram.Api.Controllers
{
    [Authorize]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// The method returns image file in .jpg format.
        /// </summary>        
        /// <response code="200">Image file.</response>
        /// <response code="400">Query parameter has incorrect format.</response>         
        /// <response code="401">Account unauthorized or doesn't exist.</response> 
        /// <response code="404">Image doesn't exist.</response> 
        [HttpGet("images/{imageId}")]
        public async Task<ActionResult> Get([FromRoute] string imageId)
        {
            var stream = new MemoryStream();
            Response.Headers[HeaderNames.ContentDisposition] = new ContentDisposition
            {
                FileName = $"{imageId}.jpg",
                DispositionType = DispositionTypeNames.Attachment
            }.ToString();

            return File(stream, "image/jpg");
        }
    }
}
