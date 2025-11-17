using HomeGarden.Dtos;
using HomeGarden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeGarden.Controllers.Auth
{
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(HomeGardenDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(x => x.Email == dto.Email))
                return Conflict("Email đã tồn tại");

            var user = new User
            {
                Fullname = dto.Fullname,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId, // 1:Admin, 2:User, 3:Technician (tuỳ DB)
                StatusId = await _db.StatusDefinitions
                             .Where(s => s.Entity == "User" && s.Code == "Active")
                             .Select(s => s.StatusId).FirstAsync(),
                CreatedAt = DateTime.Now
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Đăng ký thành công" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return NotFound("Tài khoản không tồn tại trong hệ thống.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Sai mật khẩu. Vui lòng kiểm tra lại.");

            if (user.Status.Code != "Active")
                return Forbid("Tài khoản của bạn chưa được kích hoạt hoặc đã bị khóa.");

            var token = GenerateJwtToken(user);
            return Ok(new LoginResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Fullname = user.Fullname,
                Role = user.Role.RoleName
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ProfileDto>> Profile()
        {
            if (CurrentUserId is null) return Unauthorized();
            var u = await _db.Users
                .Include(x => x.Role)
                .Include(x => x.Status)
                .FirstOrDefaultAsync(x => x.UserId == CurrentUserId);
            if (u == null) return NotFound();

            return new ProfileDto
            {
                UserId = u.UserId,
                Fullname = u.Fullname,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role.RoleName,
                Status = u.Status.Code
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["ExpireMinutes"])),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
