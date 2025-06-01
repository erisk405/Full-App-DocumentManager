using System.Data;
using HRM_API.Data;
using HRM_API.DTOs;
using HRM_API.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Repository
{
    public class RoleRepository
    {
        private readonly AppDbContext db;

        public RoleRepository(AppDbContext dbContext) 
        {
            this.db = dbContext;
        }

        // ดึงข้อมูลทั้งหมด
        public async Task<List<RoleWithPermissionDto>> GetAllRole()
        {
            var roles = await db.Roles.Include(r => r.Permission).ToListAsync();
            return roles.Select(r => new RoleWithPermissionDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                Permission = new PermissionDto
                {
                    PermissionId = r.Permission.PermissionId,
                    IsReadable = r.Permission.IsReadable,
                    IsWriteable = r.Permission.IsWriteable,
                    IsDeleteable = r.Permission.IsDeleteable
                }
            }).ToList();
        }
        // ดึงตาม ID
        public async Task<Role?> GetById(int id)
        {
            return await db.Roles.FindAsync(id);
        }
        // เพิ่ม
        public async Task Add(Role rl)
        {
            db.Roles.Add(rl);
            await db.SaveChangesAsync();
        }

        // แก้ไข
        public async Task Update(Role rl)
        {
            db.Roles.Update(rl);
            await db.SaveChangesAsync();
        }

        // ลบ
        public async Task Delete(int id)
        {
            var rl = await db.Roles.FindAsync(id);
            if (rl != null)
            {
                db.Roles.Remove(rl);
                await db.SaveChangesAsync();
            }

        }
    }
}
