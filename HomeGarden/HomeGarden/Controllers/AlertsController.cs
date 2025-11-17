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
    public class AlertsController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public AlertsController(HomeGardenDbContext db) => _db = db;

        // 🔹 GET /api/alerts?status=Active
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AlertDto>>>> GetAll([FromQuery] string? status)
        {
            var query = _db.Alerts
                .AsNoTracking()
                .Include(a => a.Plant).ThenInclude(p => p.Area)
                .Include(a => a.Status)
                .Where(a => a.IsDeleted == false || a.IsDeleted == null);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status.Code == status);

            // Nếu user không phải Admin thì chỉ xem alert thuộc cây của mình
            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.Plant.Area.UserId == CurrentUserId);

            var list = await query
                .OrderByDescending(a => a.AlertDate)
                .Select(a => new AlertDto
                {
                    AlertId = a.AlertId,
                    PlantId = a.PlantId,
                    AlertType = a.AlertType,
                    Message = a.Message,
                    Status = a.Status.Code,
                    AlertDate = a.AlertDate,
                    Resolved = a.Resolved ?? false,
                    ResolvedAt = a.ResolvedAt
                })
                .ToListAsync();

            return ApiResponse.Success(list);
        }

        // 🔹 POST /api/alerts
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] AlertCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse.Fail<object>("Dữ liệu không hợp lệ");

            var plant = await _db.Plants
                .Include(p => p.Area)
                .FirstOrDefaultAsync(p => p.PlantId == dto.PlantId && (p.IsDeleted == false || p.IsDeleted == null));

            if (plant == null)
                return ApiResponse.Fail<object>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<object>("Bạn không có quyền tạo cảnh báo cho cây này");

            var activeStatusId = await _db.StatusDefinitions
                .Where(s => s.Entity == "Alert" && s.Code == "Active")
                .Select(s => s.StatusId)
                .FirstAsync();

            var alert = new Alert
            {
                PlantId = dto.PlantId,
                AlertType = dto.AlertType.Trim(),
                Message = dto.Message.Trim(),
                StatusId = activeStatusId,
                AlertDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _db.Alerts.Add(alert);
            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { alert.AlertId }, "Tạo cảnh báo thành công");
        }

        // 🔹 POST /api/alerts/{id}/resolve
        [HttpPost("{id:long}/resolve")]
        public async Task<ActionResult<ApiResponse<string>>> Resolve(long id)
        {
            var alert = await _db.Alerts
                .Include(a => a.Plant).ThenInclude(p => p.Area)
                .FirstOrDefaultAsync(a => a.AlertId == id && (a.IsDeleted == false || a.IsDeleted == null));

            if (alert == null)
                return ApiResponse.Fail<string>("Cảnh báo không tồn tại");

            if (!User.IsInRole("Admin") && alert.Plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền xử lý cảnh báo này");

            if (alert.Resolved == true)
                return ApiResponse.Fail<string>("Cảnh báo này đã được xử lý");

            var resolvedStatusId = await _db.StatusDefinitions
                .Where(s => s.Entity == "Alert" && s.Code == "Resolved")
                .Select(s => s.StatusId)
                .FirstAsync();

            alert.Resolved = true;
            alert.ResolvedAt = DateTime.Now;
            alert.StatusId = resolvedStatusId;
            alert.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Đã xử lý cảnh báo thành công");
        }

        // 🔹 DELETE /api/alerts/{id}
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> SoftDelete(long id)
        {
            var alert = await _db.Alerts.FirstOrDefaultAsync(a => a.AlertId == id);
            if (alert == null)
                return ApiResponse.Fail<string>("Cảnh báo không tồn tại");

            alert.IsDeleted = true;
            alert.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Đã xóa mềm cảnh báo");
        }
    }
}
