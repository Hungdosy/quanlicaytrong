using HomeGarden.Dtos;
using HomeGarden.Dtos.Common;
using HomeGarden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeGarden.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ResourcesController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public ResourcesController(HomeGardenDbContext db) => _db = db;

        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ResourceDto>>>> GetMyResources(
            [FromQuery] string? type = null, [FromQuery] string? status = null)
        {
            var isAdmin = User.IsInRole("Admin");
            var query = _db.Resources
                .AsNoTracking()
                .Include(r => r.Type)
                .Include(r => r.Status)
                .Include(r => r.User)
                .Where(r => r.IsDeleted == false || r.IsDeleted == null);

            if (!isAdmin)
                query = query.Where(r => r.UserId == CurrentUserId);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(r => r.Type.Code == type);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status.Code == status);

            var list = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ResourceDto
                {
                    ResourceId = r.ResourceId,
                    Name = r.Name,
                    Type = r.Type.Code,
                    Quantity = r.Quantity,
                    Unit = r.Unit,
                    Cost = r.Cost,
                    Note = r.Note,
                    Status = r.Status.Code
                })
                .ToListAsync();

            return ApiResponse.Success(list);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] ResourceCreateDto dto)
        {
            if (CurrentUserId is null)
                return ApiResponse.Fail<object>("Không xác định được người dùng");

            // Nếu không có status → mặc định Active
            var statusId = dto.StatusId;
            if (statusId == 0)
            {
                statusId = await _db.StatusDefinitions
                    .Where(s => s.Entity == "Resource" && s.Code == "Active")
                    .Select(s => s.StatusId)
                    .FirstOrDefaultAsync();
            }

            var resource = new Resource
            {
                UserId = CurrentUserId.Value,
                Name = dto.Name.Trim(),
                TypeId = dto.TypeId,
                Quantity = dto.Quantity ?? 1,
                Unit = string.IsNullOrWhiteSpace(dto.Unit) ? "unit" : dto.Unit.Trim(),
                Cost = dto.Cost ?? 0,
                Note = dto.Note?.Trim(),
                StatusId = statusId,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _db.Resources.Add(resource);
            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { resource.ResourceId }, "Tạo tài nguyên thành công");
        }

      
        [HttpPatch("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(long id, [FromBody] ResourceUpdateDto dto)
        {
            var r = await _db.Resources.FirstOrDefaultAsync(x => x.ResourceId == id && (x.IsDeleted == false || x.IsDeleted == null));
            if (r == null)
                return ApiResponse.Fail<string>("Tài nguyên không tồn tại");

            if (!User.IsInRole("Admin") && r.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền cập nhật tài nguyên này", 403);

            if (dto.Quantity.HasValue) r.Quantity = dto.Quantity.Value;
            if (!string.IsNullOrWhiteSpace(dto.Unit)) r.Unit = dto.Unit!.Trim();
            if (dto.Cost.HasValue) r.Cost = dto.Cost;
            if (!string.IsNullOrWhiteSpace(dto.Note)) r.Note = dto.Note!.Trim();
            if (dto.StatusId.HasValue) r.StatusId = dto.StatusId.Value;

            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Cập nhật tài nguyên thành công");
        }

       
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> SoftDelete(long id)
        {
            var r = await _db.Resources.FirstOrDefaultAsync(x => x.ResourceId == id && (x.IsDeleted == false || x.IsDeleted == null));
            if (r == null)
                return ApiResponse.Fail<string>("Tài nguyên không tồn tại");

            if (!User.IsInRole("Admin") && r.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền xóa tài nguyên này", 403);

            r.IsDeleted = true;
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Đã xóa mềm tài nguyên");
        }
    }
}
