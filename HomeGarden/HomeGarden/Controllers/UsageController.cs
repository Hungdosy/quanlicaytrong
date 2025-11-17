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
    public class UsageController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public UsageController(HomeGardenDbContext db) => _db = db;

        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UsageDto>>>> List([FromQuery] long plantId)
        {
            var query = _db.PlantResourceUsages
                .AsNoTracking()
                .Include(u => u.Plant).ThenInclude(p => p.Area)
                .Include(u => u.Resource)
                .Where(u => (u.IsDeleted == false || u.IsDeleted == null) && u.PlantId == plantId);

            if (!User.IsInRole("Admin"))
                query = query.Where(u => u.Plant.Area.UserId == CurrentUserId);

            var list = await query
                .OrderByDescending(u => u.UsedAt)
                .Select(u => new UsageDto
                {
                    UsageId = u.UsageId,
                    PlantId = u.PlantId,
                    ResourceId = u.ResourceId,
                    ResourceName = u.Resource.Name,
                    QuantityUsed = u.QuantityUsed ?? 0,
                    Note = u.Note,
                    UsedAt = u.UsedAt ?? DateTime.Now
                })
                .ToListAsync();

            return ApiResponse.Success(list);
        }

       
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] UsageCreateDto dto)
        {
            if (dto.QuantityUsed <= 0)
                return ApiResponse.Fail<object>("Số lượng sử dụng phải > 0");

            // 🔸 Kiểm tra cây
            var plant = await _db.Plants
                .Include(p => p.Area)
                .FirstOrDefaultAsync(p => p.PlantId == dto.PlantId && (p.IsDeleted == false || p.IsDeleted == null));

            if (plant == null)
                return ApiResponse.Fail<object>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<object>("Bạn không có quyền sử dụng tài nguyên cho cây này", 403);

            // 🔸 Kiểm tra tài nguyên
            var res = await _db.Resources.FirstOrDefaultAsync(r =>
                r.ResourceId == dto.ResourceId && (r.IsDeleted == false || r.IsDeleted == null));

            if (res == null)
                return ApiResponse.Fail<object>("Tài nguyên không tồn tại");

            if (!User.IsInRole("Admin") && res.UserId != CurrentUserId)
                return ApiResponse.Fail<object>("Bạn không có quyền sử dụng tài nguyên này", 403);

            if ((res.Quantity ?? 0) < dto.QuantityUsed)
                return ApiResponse.Fail<object>("Số lượng tồn không đủ");

            // 🔸 Status mặc định (Active)
            var statusId = dto.StatusId;
            if (statusId == 0)
            {
                statusId = await _db.StatusDefinitions
                    .Where(s => s.Entity == "Usage" && s.Code == "Active")
                    .Select(s => s.StatusId)
                    .FirstOrDefaultAsync();
            }

            // 🔸 Tạo usage record
            var usage = new PlantResourceUsage
            {
                PlantId = dto.PlantId,
                ResourceId = dto.ResourceId,
                QuantityUsed = dto.QuantityUsed,
                Note = dto.Note?.Trim(),
                StatusId = statusId,
                UsedAt = DateTime.Now,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            _db.PlantResourceUsages.Add(usage);

            // 🔸 Cập nhật tồn kho
            res.Quantity = (res.Quantity ?? 0) - dto.QuantityUsed;
            res.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { usage.UsageId }, "Ghi nhận sử dụng tài nguyên thành công");
        }

        
        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> SoftDelete(long id)
        {
            var u = await _db.PlantResourceUsages
                .Include(x => x.Plant).ThenInclude(p => p.Area)
                .FirstOrDefaultAsync(x => x.UsageId == id && (x.IsDeleted == false || x.IsDeleted == null));

            if (u == null)
                return ApiResponse.Fail<string>("Lịch sử sử dụng không tồn tại");

            if (!User.IsInRole("Admin") && u.Plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền xóa bản ghi này", 403);

            u.IsDeleted = true;
            u.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Đã xóa mềm bản ghi sử dụng tài nguyên");
        }
    }
}
