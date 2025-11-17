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
    public class ReportsController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public ReportsController(HomeGardenDbContext db) => _db = db;

       
        [HttpGet("overview")]
        public async Task<ActionResult<ApiResponse<SystemReportDto>>> Overview()
        {
            var isAdmin = User.IsInRole("Admin");
            var uid = CurrentUserId ?? 0;

            // Base queries (chưa lọc)
            var plants = _db.Plants
                .Include(p => p.Status)
                .Include(p => p.Area)
                .Where(p => p.IsDeleted == false || p.IsDeleted == null);

            var schedules = _db.Schedules
                .Include(s => s.Status)
                .Include(s => s.Plant).ThenInclude(p => p.Area)
                .Where(s => s.IsDeleted == false || s.IsDeleted == null);

            var alerts = _db.Alerts
                .Include(a => a.Status)
                .Include(a => a.Plant).ThenInclude(p => p.Area)
                .Where(a => a.IsDeleted == false || a.IsDeleted == null);

            var resources = _db.Resources
                .Where(r => r.IsDeleted == false || r.IsDeleted == null);

            var users = _db.Users
                .Where(u => u.IsDeleted == false || u.IsDeleted == null);

            // Nếu không phải Admin → chỉ lấy dữ liệu của user hiện tại
            if (!isAdmin)
            {
                plants = plants.Where(p => p.Area.UserId == uid);
                schedules = schedules.Where(s => s.Plant.Area.UserId == uid);
                alerts = alerts.Where(a => a.Plant.Area.UserId == uid);
                resources = resources.Where(r => r.UserId == uid);
                users = users.Where(u => u.UserId == uid);
            }

            // ✅ Tính toán song song để tối ưu hiệu năng
            var totalUsersTask = users.CountAsync();
            var totalPlantsTask = plants.CountAsync();
            var deadPlantsTask = plants.CountAsync(p => p.Status.Code == "Dead");
            var harvestedPlantsTask = plants.CountAsync(p => p.Status.Code == "Harvested");
            var overdueSchedulesTask = schedules.CountAsync(s =>
                (s.Status.Code == "Overdue") ||
                (s.NextDue != null && s.NextDue < DateTime.Now && s.Status.Code == "Pending"));
            var activeAlertsTask = alerts.CountAsync(a => a.Status.Code == "Active");
            var lowResourcesTask = resources.CountAsync(r => r.Quantity <= 1);

            await Task.WhenAll(
                totalUsersTask,
                totalPlantsTask,
                deadPlantsTask,
                harvestedPlantsTask,
                overdueSchedulesTask,
                activeAlertsTask,
                lowResourcesTask
            );

            var dto = new SystemReportDto
            {
                TotalUsers = totalUsersTask.Result,
                TotalPlants = totalPlantsTask.Result,
                DeadPlants = deadPlantsTask.Result,
                HarvestedPlants = harvestedPlantsTask.Result,
                OverdueSchedules = overdueSchedulesTask.Result,
                ActiveAlerts = activeAlertsTask.Result,
                LowResources = lowResourcesTask.Result
            };

            return ApiResponse.Success(dto);
        }

        [HttpGet("plant/{id:long}")]
        public async Task<ActionResult<ApiResponse<PlantReportDto>>> PlantReport(long id)
        {
            var plant = await _db.Plants
                .Include(p => p.Area)
                .FirstOrDefaultAsync(p => p.PlantId == id && (p.IsDeleted == false || p.IsDeleted == null));

            if (plant == null)
                return ApiResponse.Fail<PlantReportDto>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<PlantReportDto>("Bạn không có quyền xem cây này", 403);

            var totalLogsTask = _db.PlantLogs.CountAsync(l => l.PlantId == id && (l.IsDeleted == false || l.IsDeleted == null));
            var totalAlertsTask = _db.Alerts.CountAsync(a => a.PlantId == id && (a.IsDeleted == false || a.IsDeleted == null));
            var totalSchedulesTask = _db.Schedules.CountAsync(s => s.PlantId == id && (s.IsDeleted == false || s.IsDeleted == null));

            // ✅ Tổng chi phí
            var totalCostTask = _db.PlantResourceUsages
                .Include(u => u.Resource)
                .Where(u => u.PlantId == id && (u.IsDeleted == false || u.IsDeleted == null))
                .SumAsync(u => (u.QuantityUsed ?? 0) * (u.Resource.Cost ?? 0));

            await Task.WhenAll(totalLogsTask, totalAlertsTask, totalSchedulesTask, totalCostTask);

            var dto = new PlantReportDto
            {
                PlantId = plant.PlantId,
                PlantName = plant.Name,
                TotalLogs = totalLogsTask.Result,
                TotalAlerts = totalAlertsTask.Result,
                TotalSchedules = totalSchedulesTask.Result,
                TotalCost = totalCostTask.Result
            };

            return ApiResponse.Success(dto);
        }
    }
}
