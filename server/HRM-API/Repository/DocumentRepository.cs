using HRM_API.Data;
using HRM_API.Model;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Repository
{
    public class DocumentRepository
    {
        private readonly AppDbContext db;

        public DocumentRepository(AppDbContext dbContext) 
        { 
            this.db = dbContext;
        }

        // ดึงข้อมูลทั้งหมด
        public async Task<List<Document>> GetAllDocument()
        {
            return await db.Documents.ToListAsync();
        }
        // ดึงตาม ID
        public async Task<Document?> GetById(string id)
        {
            return await db.Documents.FindAsync(id);
        }
        // เพิ่มเอกสาร
        public async Task Add(Document doc)
        {
            db.Documents.Add(doc);  
            await db.SaveChangesAsync();
        }

        // แก้ไขเอกสาร
        public async Task Update(Document doc)
        {
            db.Documents.Update(doc);
            await db.SaveChangesAsync();
        }

        // ลบเอกสาร
        public async Task Delete(string id)
        {
            var doc = await db.Documents.FindAsync(id);
            if (doc != null)
            {
                db.Documents.Remove(doc);
                await db.SaveChangesAsync();
            }

        }

    }
}
