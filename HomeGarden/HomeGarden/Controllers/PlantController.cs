using HomeGarden.Dtos;
using HomeGarden.Dtos.Common;
using HomeGarden.Infrastructure;
using HomeGarden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeGarden.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class PlantsController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public PlantsController(HomeGardenDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PlantListDto>>>> Get(
            [FromQuery] long? areaId,
            [FromQuery] string? status,
            [FromQuery] string? search,
            [FromQuery] PagedRequest req)
        {
            var query = _db.Plants
                .AsNoTracking()
                .Include(p => p.Area)
                .Include(p => p.Status)
                .Include(p => p.Health)
                .Where(p => p.IsDeleted == false || p.IsDeleted == null);

            if (areaId.HasValue)
                query = query.Where(p => p.AreaId == areaId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status.Code == status);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Species ?? "").Contains(search));

            if (!User.IsInRole("Admin"))
                query = query.Where(p => p.Area.UserId == CurrentUserId);

            var total = await query.CountAsync();
            var plants = await query
                .OrderByDescending(p => p.PlantId)
                .Skip((req.Page - 1) * req.Size)
                .Take(req.Size)
                .Select(p => new PlantListDto
                {
                    PlantId = p.PlantId,
                    Name = p.Name,
                    Species = p.Species,
                    Health = p.Health != null ? p.Health.Code : null,
                    Status = p.Status.Code,
                    PlantedDate = p.PlantedDate.HasValue
                        ? p.PlantedDate.Value.ToDateTime(TimeOnly.MinValue)
                        : (DateTime?)null
                })
                .ToListAsync();

            var result = PagedResult<PlantListDto>.Create(plants, req.Page, req.Size, total);
            return ApiResponse.Success(result);
        }

       
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<PlantDetailDto>>> Detail(long id)
        {
            var p = await _db.Plants
                .Include(x => x.Area)
                .Include(x => x.Status)
                .Include(x => x.Health)
                .FirstOrDefaultAsync(x => x.PlantId == id && (x.IsDeleted == false || x.IsDeleted == null));

            if (p == null)
                return ApiResponse.Fail<PlantDetailDto>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && p.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<PlantDetailDto>("Bạn không có quyền xem cây này", 403);

            var dto = new PlantDetailDto
            {
                PlantId = p.PlantId,
                AreaId = p.AreaId,
                Name = p.Name,
                Species = p.Species,
                PlantedDate = p.PlantedDate.HasValue
                    ? p.PlantedDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null,
                ImageUrl = p.ImageUrl,
                Notes = p.Notes,
                Health = p.Health?.Code,
                Status = p.Status.Code
            };

            return ApiResponse.Success(dto);
        }

        [HttpPost]
        [Authorize(Roles = "User,Technician,Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] PlantCreateDto dto)
        {
            var area = await _db.Areas
                .FirstOrDefaultAsync(a => a.AreaId == dto.AreaId && (a.IsDeleted == false || a.IsDeleted == null));

            if (area == null)
                return ApiResponse.Fail<object>("Khu vực không tồn tại");

            if (!User.IsInRole("Admin") && area.UserId != CurrentUserId)
                return ApiResponse.Fail<object>("Bạn không có quyền thêm cây vào khu vực này", 403);

            var plant = new Plant
            {
                AreaId = dto.AreaId,
                Name = dto.Name.Trim(),
                Species = dto.Species?.Trim(),
                PlantedDate = dto.PlantedDate?.ToDateOnly(),
                ImageUrl = dto.ImageUrl?.Trim(),
                HealthId = dto.HealthId,
                StatusId = dto.StatusId,
                Notes = dto.Notes?.Trim(),
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _db.Plants.Add(plant);
            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { plant.PlantId }, "Tạo cây thành công");
        }

        [HttpPatch("{id:long}")]
        [Authorize(Roles = "User,Technician,Admin")]
        public async Task<ActionResult<ApiResponse<string>>> Update(long id, [FromBody] PlantUpdateDto dto)
        {
            var p = await _db.Plants
                .Include(x => x.Area)
                .FirstOrDefaultAsync(x => x.PlantId == id && (x.IsDeleted == false || x.IsDeleted == null));

            if (p == null)
                return ApiResponse.Fail<string>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && p.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền cập nhật cây này", 403);

            if (!string.IsNullOrWhiteSpace(dto.Name)) p.Name = dto.Name.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Species)) p.Species = dto.Species.Trim();
            if (dto.PlantedDate.HasValue) p.PlantedDate = dto.PlantedDate.Value.ToDateOnly();
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl)) p.ImageUrl = dto.ImageUrl.Trim();
            if (dto.HealthId.HasValue) p.HealthId = dto.HealthId.Value;
            if (dto.StatusId.HasValue) p.StatusId = dto.StatusId.Value;
            if (!string.IsNullOrWhiteSpace(dto.Notes)) p.Notes = dto.Notes.Trim();

            p.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Cập nhật cây thành công");
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "User,Technician,Admin")]
        public async Task<ActionResult<ApiResponse<string>>> SoftDelete(long id)
        {
            var p = await _db.Plants
                .Include(x => x.Area)
                .FirstOrDefaultAsync(x => x.PlantId == id && (x.IsDeleted == false || x.IsDeleted == null));

            if (p == null)
                return ApiResponse.Fail<string>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && p.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền xóa cây này", 403);

            p.IsDeleted = true;
            p.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Đã xóa cây thành công");
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new
            {
                IsAuth = User.Identity?.IsAuthenticated,
                Name = User.Identity?.Name,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).ToList(),
                NameId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Sub = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                Uid = User.Claims.FirstOrDefault(c => c.Type == "uid" || c.Type == "user_id")?.Value,
                All = claims
            });
        }
        [HttpGet("debug")]
        public async Task<IActionResult> DebugDb()
        {
            var conn = _db.Database.GetDbConnection();
            var uid = CurrentUserId ?? -1;

            var totalForUser = await _db.Plants
                .Include(p => p.Area)
                .CountAsync(p => (p.IsDeleted == false || p.IsDeleted == null)
                                 && p.Area.UserId == uid);

            var totalAll = await _db.Plants.CountAsync();

            return Ok(new
            {
                uid,
                db = new { conn.Database, conn.DataSource },
                totals = new { totalAll, totalForUser }
            });
        }


    }
}
