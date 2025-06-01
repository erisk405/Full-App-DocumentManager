using HRM_API.Data;
using HRM_API.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Repository
{
    public class ImageRepository
    {
        private readonly AppDbContext db;

        public ImageRepository(AppDbContext dbContext)
        {
            this.db = dbContext;
        }
        // ดึงข้อมูลทั้งหมด
        public async Task<List<Images>> GetAllImage()
        {
            return await db.Images.ToListAsync();
        }
        // ดึงตาม ID
        public async Task<Images?> GetById(int id)
        {
            return await db.Images.FindAsync(id);
        }
        // เพิ่ม
        public async Task Add(Images img)
        {
            db.Images.Add(img);
            await db.SaveChangesAsync();
        }

        // แก้ไข
        public async Task Update(Images img)
        {
            db.Images.Update(img);
            await db.SaveChangesAsync();
        }

        // ลบ
        public async Task Delete(string id)
        {
            var img = await db.Images.FindAsync(id);
            if (img != null)
            {
                db.Images.Remove(img);
                await db.SaveChangesAsync();
            }

        }
    }
}
