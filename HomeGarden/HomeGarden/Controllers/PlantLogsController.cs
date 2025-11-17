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
    public class PlantLogsController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public PlantLogsController(HomeGardenDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PlantLogDto>>>> List([FromQuery] long plantId)
        {
            var query = _db.PlantLogs
                .AsNoTracking()
                .Include(l => l.Plant).ThenInclude(p => p.Area)
                .Include(l => l.Health)
                .Where(l => l.PlantId == plantId && (l.IsDeleted == false || l.IsDeleted == null));

            if (!User.IsInRole("Admin"))
                query = query.Where(l => l.Plant.Area.UserId == CurrentUserId);

            var list = await query
                         .OrderByDescending(l => l.LogDate)
                         .Select(l => new PlantLogDto
                            {
                                LogId = l.LogId,
                                PlantId = l.PlantId,
                                Activity = l.Activity,
                                Description = l.Description,
                                ImageUrl = l.ImageUrl,
                                Health = l.Health != null ? l.Health.Code : null,
                                LogDate = l.LogDate ?? DateTime.Now
                            })
                         .ToListAsync();


            return ApiResponse.Success(list);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] PlantLogCreateDto dto)
        {
            var plant = await _db.Plants
                .Include(p => p.Area)
                .FirstOrDefaultAsync(p => p.PlantId == dto.PlantId && (p.IsDeleted == false || p.IsDeleted == null));

            if (plant == null)
                return ApiResponse.Fail<object>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<object>("Bạn không có quyền ghi log cho cây này", 403);

            // Lấy status "Growing" mặc định cho log
            var statusId = await _db.StatusDefinitions
                .Where(s => s.Entity == "Plant" && s.Code == "Growing")
                .Select(s => s.StatusId)
                .FirstAsync();

            var log = new PlantLog
            {
                PlantId = dto.PlantId,
                Activity = dto.Activity.Trim(),
                Description = dto.Description?.Trim(),
                ImageUrl = dto.ImageUrl?.Trim(),
                HealthId = dto.HealthId,
                StatusId = statusId,
                LogDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _db.PlantLogs.Add(log);
            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { log.LogId }, "Tạo log thành công");
        }


        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(long id)
        {
            var log = await _db.PlantLogs
                .Include(l => l.Plant).ThenInclude(p => p.Area)
                .FirstOrDefaultAsync(l => l.LogId == id && (l.IsDeleted == false || l.IsDeleted == null));

            if (log == null)
                return ApiResponse.Fail<string>("Log không tồn tại");

            if (!User.IsInRole("Admin") && log.Plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền xóa log này", 403);

            log.IsDeleted = true;
            log.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Đã xóa log thành công");
        }
    }
}
