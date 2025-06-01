using HRM_API.Model;
using HRM_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageRepository img;

        public ImageController(ImageRepository imageRepository)
        {
            this.img = imageRepository;
        }
        [HttpGet]
        public async Task<ActionResult> ImageList()
        {
            var allDocument = await img.GetAllImage();
            return Ok(allDocument);
        }

        [HttpPost]
        public async Task<ActionResult> AddImage([FromBody] Images im)
        {
            if (im == null)
            {
                return BadRequest();
            }
            try
            {
                await img.Add(im);
                return Ok(im);
            }
            catch (Exception ex)
            {
                // ถ้ามีข้อผิดพลาดเกิดขึ้น
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error : {ex.Message}");
            }
        }
    }
}
