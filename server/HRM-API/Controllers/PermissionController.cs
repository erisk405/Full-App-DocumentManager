using HRM_API.Model;
using HRM_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {

        private readonly PermissionRepository perm;

        public PermissionController(PermissionRepository permissionRepository)
        {
            this.perm = permissionRepository;
        }

        [HttpGet]
        public async Task<ActionResult> PermissionList()
        {
            var allDocument = await perm.GetAllPermissions();
            return Ok(allDocument);
        }

        [HttpPost]
        public async Task<ActionResult> AddPermission([FromBody] Permissions pr)
        {
            if (pr == null)
            {
                return BadRequest();
            }
            try
            {
                await perm.Add(pr);
                return Ok(pr);
            }
            catch (Exception ex)
            {
                // ถ้ามีข้อผิดพลาดเกิดขึ้น
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error : {ex.Message}");
            }
        }
    }
}
