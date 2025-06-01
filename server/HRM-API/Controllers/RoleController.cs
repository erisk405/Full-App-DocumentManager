using System.Security;
using HRM_API.DTOs;
using HRM_API.Model;
using HRM_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly RoleRepository roleRepository;
        private readonly PermissionRepository permissionRepository;
        public RoleController(RoleRepository roleRepository,PermissionRepository permissionRepository) 
        {
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;
        }
        [HttpGet]
        public async Task<ActionResult> RoleList()
        {
            var allrole = await roleRepository.GetAllRole();
            return Ok(allrole);
        }

        [HttpGet("id")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            var role = await roleRepository.GetById(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpPut("batch")]
        public async Task<ActionResult> UpdateMultipleRoles([FromBody] List<Role> roles)
        {
            if (roles == null || !roles.Any())
            {
                return BadRequest();
            }

            try
            {
                foreach (var role in roles)
                {
                    await roleRepository.Update(role);
                }
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error : {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddRole([FromBody] RoleWithPermissionDto dto)
        {
            if (dto == null || dto.Permission == null)
                return BadRequest("Permission is required");

            try
            {
                var permission = new Permissions
                {
                    IsReadable = dto.Permission.IsReadable,
                    IsWriteable = dto.Permission.IsWriteable,
                    IsDeleteable = dto.Permission.IsDeleteable
                };
                await permissionRepository.Add(permission);

                var role = new Role
                {
                    RoleName = dto.RoleName,
                    PermissionId = permission.PermissionId
                };
                await roleRepository.Add(role);

                var response = new RoleResponseDto
                {
                    RoleName = role.RoleName,
                    PermissionId = permission.PermissionId,
                    IsReadable = permission.IsReadable,
                    IsWriteable = permission.IsWriteable,
                    IsDeleteable = permission.IsDeleteable
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

    }
}
