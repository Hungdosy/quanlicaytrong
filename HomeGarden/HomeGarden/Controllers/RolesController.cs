// Controllers/RolesController.cs
using HomeGarden.Dtos;
using HomeGarden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeGarden.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public RolesController(HomeGardenDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _db.Roles.Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                Description = r.Description
            }).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(RoleCreateDto dto)
        {
            if (await _db.Roles.AnyAsync(r => r.RoleName == dto.RoleName))
                return Conflict("Role đã tồn tại");
            var r = new Role
            {
                RoleName = dto.RoleName,
                Description = dto.Description,
                StatusId = await _db.StatusDefinitions.Where(s => s.Entity == "User" && s.Code == "Active")
                              .Select(s => s.StatusId).FirstAsync(),
                CreatedAt = DateTime.Now
            };
            _db.Roles.Add(r);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = r.RoleId }, new { r.RoleId });
        }
    }
}
