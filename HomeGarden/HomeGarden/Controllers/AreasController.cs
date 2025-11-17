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
    public class AreasController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public AreasController(HomeGardenDbContext db) => _db = db;

        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AreaDto>>>> GetMyAreas()
        {
            var query = _db.Areas
                .AsNoTracking()
                .Include(a => a.Status)
                .Where(a => a.IsDeleted == false || a.IsDeleted == null);

            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.UserId == CurrentUserId);

            var result = await query
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AreaDto
                {
                    AreaId = a.AreaId,
                    Name = a.Name,
                    Location = a.Location,
                    Description = a.Description,
                    Status = a.Status.Code
                })
                .ToListAsync();

            return ApiResponse.Success(result);
        }

        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] AreaCreateDto dto)
        {
            if (CurrentUserId is null)
                return ApiResponse.Fail<object>("Không xác định được người dùng");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponse.Fail<object>("Tên khu vực không được để trống");

            var activeStatusId = await _db.StatusDefinitions
                .Where(s => s.Entity == "Area" && s.Code == "Active")
                .Select(s => s.StatusId)
                .FirstAsync();

            var area = new Area
            {
                UserId = CurrentUserId.Value,
                Name = dto.Name.Trim(),
                Location = dto.Location?.Trim(),
                Description = dto.Description?.Trim(),
                StatusId = activeStatusId,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _db.Areas.Add(area);
            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { area.AreaId }, "Tạo khu vực thành công");
        }

       
        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<AreaDto>>> GetById(long id)
        {
            var area = await _db.Areas
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AreaId == id && (a.IsDeleted == false || a.IsDeleted == null));

            if (area == null)
                return ApiResponse.Fail<AreaDto>("Khu vực không tồn tại");

            if (!User.IsInRole("Admin") && area.UserId != CurrentUserId)
                return ApiResponse.Fail<AreaDto>("Bạn không có quyền xem khu vực này", 403);

            var dto = new AreaDto
            {
                AreaId = area.AreaId,
                Name = area.Name,
                Location = area.Location,
                Description = area.Description,
                Status = area.Status.Code
            };

            return ApiResponse.Success(dto);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(long id, [FromBody] AreaUpdateDto dto)
        {
            var area = await _db.Areas.FirstOrDefaultAsync(a => a.AreaId == id && (a.IsDeleted == false || a.IsDeleted == null));
            if (area == null)
                return ApiResponse.Fail<string>("Khu vực không tồn tại");

            if (!User.IsInRole("Admin") && area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền cập nhật khu vực này", 403);

            bool hasChange = false;

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != area.Name)
            {
                area.Name = dto.Name.Trim();
                hasChange = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.Location) && dto.Location != area.Location)
            {
                area.Location = dto.Location.Trim();
                hasChange = true;
            }

            if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description != area.Description)
            {
                area.Description = dto.Description.Trim();
                hasChange = true;
            }

            if (dto.StatusId.HasValue && dto.StatusId != area.StatusId)
            {
                area.StatusId = dto.StatusId.Value;
                hasChange = true;
            }

            if (!hasChange)
                return ApiResponse.Success("Không có thay đổi nào để cập nhật");

            area.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Cập nhật khu vực thành công");
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult<ApiResponse<string>>> SoftDelete(long id)
        {
            var area = await _db.Areas.FirstOrDefaultAsync(a => a.AreaId == id && (a.IsDeleted == false || a.IsDeleted == null));
            if (area == null)
                return ApiResponse.Fail<string>("Khu vực không tồn tại");

            if (!User.IsInRole("Admin") && area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền xóa khu vực này", 403);

            area.IsDeleted = true;
            area.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Đã xóa mềm khu vực thành công");
        }

        [HttpGet("{id:long}/plants")]
        public async Task<ActionResult<ApiResponse<List<AreaPlantDto>>>> GetPlants(long id)
        {
            var area = await _db.Areas.FirstOrDefaultAsync(a => a.AreaId == id && (a.IsDeleted == false || a.IsDeleted == null));
            if (area == null)
                return ApiResponse.Fail<List<AreaPlantDto>>("Khu vực không tồn tại");

            if (!User.IsInRole("Admin") && area.UserId != CurrentUserId)
                return ApiResponse.Fail<List<AreaPlantDto>>("Bạn không có quyền truy cập cây trong khu vực này", 403);

            var plants = await _db.Plants
                .AsNoTracking()
                .Include(p => p.Status)
                .Include(p => p.Health)
                .Where(p => p.AreaId == id && (p.IsDeleted == false || p.IsDeleted == null))
               .Select(p => new AreaPlantDto
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

            return ApiResponse.Success(plants);
        }
    }
}
