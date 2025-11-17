// Controllers/LookupsController.cs
using HomeGarden.Dtos;
using HomeGarden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeGarden.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class LookupsController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public LookupsController(HomeGardenDbContext db) { _db = db; }

        [HttpGet("status/{entity}")]
        public async Task<IActionResult> StatusByEntity(string entity)
        {
            var list = await _db.StatusDefinitions
                .Where(s => s.Entity == entity)
                .Select(s => new StatusDto
                {
                    StatusId = s.StatusId,
                    Entity = s.Entity,
                    Code = s.Code,
                    Description = s.Description!
                }).ToListAsync();

            return Ok(list);
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            var list = await _db.HealthDefinitions.Select(h => new HealthDto
            {
                HealthId = h.HealthId,
                Code = h.Code,
                Description = h.Description!
            }).ToListAsync();
            return Ok(list);
        }

        [HttpGet("resource-types")]
        public async Task<IActionResult> ResourceTypes()
        {
            var list = await _db.ResourceTypes.Select(t => new ResourceTypeDto
            {
                TypeId = t.TypeId,
                Code = t.Code,
                Description = t.Description!
            }).ToListAsync();
            return Ok(list);
        }
    }
}
