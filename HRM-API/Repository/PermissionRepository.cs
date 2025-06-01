using HRM_API.Data;
using HRM_API.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Repository
{
    public class PermissionRepository
    {
        private readonly AppDbContext db;

        public PermissionRepository(AppDbContext dbContext)
        {
            this.db = dbContext;
        }
        // ดึงข้อมูลทั้งหมด
        public async Task<List<Permissions>> GetAllPermissions()
        {
            return await db.Permissions.ToListAsync();
        }
        // ดึงตาม ID
        public async Task<Permissions?> GetById(int id)
        {
            return await db.Permissions.FindAsync(id);
        }
        // เพิ่ม
        public async Task Add(Permissions perm)
        {
            db.Permissions.Add(perm);
            await db.SaveChangesAsync();
        }

        // แก้ไข
        public async Task Update(Permissions perm)
        {
            db.Permissions.Update(perm);
            await db.SaveChangesAsync();
        }

        // ลบ
        public async Task Delete(string id)
        {
            var perm = await db.Permissions.FindAsync(id);
            if (perm != null)
            {
                db.Permissions.Remove(perm);
                await db.SaveChangesAsync();
            }

        }
    }
}
