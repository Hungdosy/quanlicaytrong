using HomeGarden.Dtos;
using HomeGarden.Dtos.Common;
using HomeGarden.Infrastructure;
using HomeGarden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeGarden.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public UsersController(HomeGardenDbContext db) => _db = db;

        // 🔹 GET /api/users?role=User&status=Active&page=1&size=20
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetAll(
            [FromQuery] string? role,
            [FromQuery] string? status,
            [FromQuery] PagedRequest req)
        {
            var query = _db.Users
                .Include(u => u.Role)
                .Include(u => u.Status)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.Role.RoleName == role);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(u => u.Status.Code == status);

            var total = await query.LongCountAsync();

            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((req.Page - 1) * req.Size)
                .Take(req.Size)
                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    Fullname = u.Fullname,
                    Email = u.Email,
                    Phone = u.Phone,
                    RoleName = u.Role.RoleName,
                    Status = u.Status.Code
                })
                .ToListAsync();

            var result = PagedResult<UserListDto>.Create(items, req.Page, req.Size, total);
            return ApiResponse.Success(result);
        }

        // 🔹 GET /api/users/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<UserListDto>>> Get(long id)
        {
            var u = await _db.Users
                .Include(x => x.Role)
                .Include(x => x.Status)
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (u == null)
                return ApiResponse.Fail<UserListDto>("User không tồn tại");

            var dto = new UserListDto
            {
                UserId = u.UserId,
                Fullname = u.Fullname,
                Email = u.Email,
                Phone = u.Phone,
                RoleName = u.Role.RoleName,
                Status = u.Status.Code
            };
            return ApiResponse.Success(dto);
        }

        // 🔹 POST /api/users
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return ApiResponse.Fail<object>("Email đã tồn tại");

            var activeStatus = await _db.StatusDefinitions
                .Where(s => s.Entity == "User" && s.Code == "Active")
                .Select(s => s.StatusId)
                .FirstAsync();

            var user = new User
            {
                Fullname = dto.Fullname,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                StatusId = activeStatus,
                CreatedAt = DateTime.Now,
                CreatedBy = CurrentUserId
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { user.UserId }, "Tạo user thành công");
        }

        // 🔹 PATCH /api/users/5
        [HttpPatch("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(long id, [FromBody] UserUpdateDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return ApiResponse.Fail<string>("User không tồn tại");

            if (!string.IsNullOrWhiteSpace(dto.Fullname))
                user.Fullname = dto.Fullname!;
            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone!;
            if (dto.StatusId.HasValue)
                user.StatusId = dto.StatusId.Value;

            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = CurrentUserId;

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Cập nhật thông tin thành công");
        }

        // 🔹 POST /api/users/5/reset-password
        [HttpPost("{id:long}/reset-password")]
        public async Task<ActionResult<ApiResponse<string>>> ResetPassword(long id, [FromBody] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return ApiResponse.Fail<string>("Mật khẩu phải >= 6 ký tự");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return ApiResponse.Fail<string>("User không tồn tại");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = CurrentUserId;

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Đặt lại mật khẩu thành công");
        }

        // 🔹 POST /api/users/5/role/2
        [HttpPost("{id:long}/role/{roleId:int}")]
        public async Task<ActionResult<ApiResponse<string>>> SetRole(long id, int roleId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return ApiResponse.Fail<string>("User không tồn tại");

            var role = await _db.Roles.FindAsync(roleId);
            if (role == null)
                return ApiResponse.Fail<string>("Role không tồn tại");

            user.RoleId = roleId;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = CurrentUserId;

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Cập nhật vai trò thành công");
        }

        // 🔹 DELETE /api/users/5
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> SoftDelete(long id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return ApiResponse.Fail<string>("User không tồn tại");

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = CurrentUserId;

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Đã xóa mềm user");
        }
    }
}
