using HRM_API.Data;
using HRM_API.DTOs;
using HRM_API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRM_API.Repository
{
    public class UserRepository
    {
        private readonly AppDbContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.db = dbContext;

            _httpContextAccessor = httpContextAccessor;
        }
        // ดึงข้อมูลทั้งหมดพร้อม pagination
        public async Task<(List<UserGetDTO> Users, int TotalCount)> GetAllUsers(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? orderBy = null,
            string? orderDirection = null)
        {
            var baseUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";

            // Start with base query
            var query = db.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permission)
                .AsQueryable();

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.First_name.Contains(search) ||
                    u.Last_name.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.Username.Contains(search)
                );
            }

            // Apply ordering
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var direction = orderDirection?.ToLower() == "desc" ? "descending" : "ascending";
                query = orderBy.ToLower() switch
                {
                    "firstname" => direction == "ascending"
                        ? query.OrderBy(u => u.First_name)
                        : query.OrderByDescending(u => u.First_name),
                    "lastname" => direction == "ascending"
                        ? query.OrderBy(u => u.Last_name)
                        : query.OrderByDescending(u => u.Last_name),
                    "email" => direction == "ascending"
                        ? query.OrderBy(u => u.Email)
                        : query.OrderByDescending(u => u.Email),
                    "role" => direction == "ascending"
                        ? query.OrderBy(u => u.Role!.RoleName)
                        : query.OrderByDescending(u => u.Role!.RoleName),
                    "   " => direction == "ascending"
                        ? query.OrderBy(u => u.CreateAt)
                        : query.OrderByDescending(u => u.CreateAt),
                    _ => query.OrderByDescending(u => u.CreateAt) // default sorting
                };
            }
            else
            {
                // Default sorting if no orderBy is specified
                query = query.OrderByDescending(u => u.CreateAt);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Execute query and map to DTO
            var users = await query.Select(u => new UserGetDTO
            {
                UserId = u.UserId,
                First_name = u.First_name,
                Last_name = u.Last_name,
                Email = u.Email,
                Username = u.Username,
                Phone = u.Phone,
                RoleId = u.RoleId,
                Role_name = u.Role != null ? u.Role.RoleName : string.Empty,
                Permission = u.Role != null && u.Role.Permission != null
                    ? new PermissionDto
                    {
                        PermissionId = u.Role.Permission.PermissionId,
                        IsReadable = u.Role.Permission.IsReadable,
                        IsWriteable = u.Role.Permission.IsWriteable,
                        IsDeleteable = u.Role.Permission.IsDeleteable
                    }
                    : null,
                CreateAt = u.CreateAt,
                ProfileImageUrl = u.Image != null && !string.IsNullOrEmpty(u.Image.ProfileImagePath)
                    ? $"{baseUrl}{u.Image.ProfileImagePath}"
                    : string.Empty
            }).ToListAsync();

            return (users, totalCount);
        }
        // ดึงข้อมูลทั้งหมด
        //public async Task<List<UserGetDTO>> GetAllUsers()
        //{
        //    var baseUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";

        //    return await db.Users
        //        .Include(u => u.Role)
        //        .ThenInclude(r => r.Permission)
        //        .Select(u => new UserGetDTO
        //        {
        //            UserId = u.UserId,
        //            First_name = u.First_name,
        //            Last_name = u.Last_name,
        //            Email = u.Email,
        //            Username = u.Username,
        //            Phone = u.Phone,
        //            RoleId = u.RoleId,
        //            Role_name = u.Role != null ? u.Role.RoleName : string.Empty,
        //            Permission = u.Role != null && u.Role.Permission != null
        //                ? new PermissionDto
        //                {
        //                    PermissionId = u.Role.Permission.PermissionId,
        //                    IsReadable = u.Role.Permission.IsReadable,
        //                    IsWriteable = u.Role.Permission.IsWriteable,
        //                    IsDeleteable = u.Role.Permission.IsDeleteable
        //                }
        //                : null,
        //            CreateAt = u.CreateAt,
        //            ProfileImageUrl = u.Image != null && !string.IsNullOrEmpty(u.Image.ProfileImagePath)
        //                ? $"{baseUrl}{u.Image.ProfileImagePath}"
        //                : string.Empty 
        //        })
        //        .ToListAsync();
        //}
        // ดึงตาม ID
        public async Task<UserGetDTO?> GetById(string id)
        {
            var baseUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";

            return await db.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permission)
                .Where(u => u.UserId == id)
                .Select(u => new UserGetDTO
                {
                    UserId = u.UserId,
                    First_name = u.First_name,
                    Last_name = u.Last_name,
                    Email = u.Email,
                    Username = u.Username,
                    Role_name = u.Role != null ? u.Role.RoleName : string.Empty,
                    Permission = u.Role != null && u.Role.Permission != null
                        ? new PermissionDto
                        {
                            PermissionId = u.Role.Permission.PermissionId,
                            IsReadable = u.Role.Permission.IsReadable,
                            IsWriteable = u.Role.Permission.IsWriteable,
                            IsDeleteable = u.Role.Permission.IsDeleteable
                        }
                        : null,
                    CreateAt = u.CreateAt,
                    ProfileImageUrl = u.Image != null && !string.IsNullOrEmpty(u.Image.ProfileImagePath)
                        ? $"{baseUrl}{u.Image.ProfileImagePath}"
                        : string.Empty
                })
                .FirstOrDefaultAsync();
        }
        // ดึงตาม Username
        public async Task<User?> GetUserByUsername(string username)
        {
            return await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task Add(User us)
        {
            var hasher = new PasswordHasher<User>();
            us.Password = hasher.HashPassword(us, us.Password);
            

            db.Users.Add(us);
            await db.SaveChangesAsync();
        }
        public async Task Update(User us)
        {
            db.Users.Update(us);
            await db.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var us = await db.Users.FindAsync(id);
            if (us != null)
            {
                db.Users.Remove(us);
                await db.SaveChangesAsync();
            }

        }

        public async Task<User> GetUserImage(string userId)
        {
            return await db.Users.Include(u => u.Image).FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<bool> IsDuplicateEmailOrUsername(string email, string username)
        {
            return await db.Users.AnyAsync(u => u.Email == email || u.Username == username);
        }

        public async Task<bool> ChangePassword(string userId, string currentPassword ,  string newPassword)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                // ส่ง false กลับ หรือจะโยน exception ก็ได้
                return false;
            }
            Console.WriteLine("user", user);
            var hasher = new PasswordHasher<User>();


            // ตรวจสอบรหัสผ่านปัจจุบัน
            var result = hasher.VerifyHashedPassword(user, user.Password, currentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                return false; // รหัสผ่านปัจจุบันไม่ถูกต้อง
            }

            // แฮชและบันทึกรหัสผ่านใหม่
            user.Password = hasher.HashPassword(user, newPassword);
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return true;

        }

    }
}
