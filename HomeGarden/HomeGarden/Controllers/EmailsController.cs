using HomeGarden.Dtos;
using HomeGarden.Dtos.Common;
using HomeGarden.Models;
using HomeGarden.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeGarden.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EmailsController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        private readonly EmailService _emailService;

        public EmailsController(HomeGardenDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        // 🔹 GET /api/emails/templates
        [HttpGet("templates")]
        public async Task<ActionResult<ApiResponse<List<EmailTemplateDto>>>> GetTemplates()
        {
            var templates = await _db.EmailTemplates
                .OrderBy(t => t.Code)
                .Select(t => new EmailTemplateDto
                {
                    TemplateId = t.TemplateId,
                    Code = t.Code,
                    Subject = t.Subject,
                    Description = t.Description,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return ApiResponse.Success(templates);
        }

        // 🔹 POST /api/emails/send
        // Body:
        // {
        //   "userIds": [1,2,3],   // hoặc "userId": 5
        //   "subject": "Tiêu đề",
        //   "content": "Nội dung",
        // }
        [HttpPost("send")]
        public async Task<ActionResult<ApiResponse<string>>> SendEmail([FromBody] EmailSendDto dto)
        {
            // Gom tất cả ID lại (UserId + UserIds)
            var ids = new List<long>();
            if (dto.UserIds != null && dto.UserIds.Any())
                ids.AddRange(dto.UserIds);
            if (dto.UserId.HasValue)
                ids.Add(dto.UserId.Value);

            ids = ids.Distinct().ToList();

            if (!ids.Any())
                return ApiResponse.Fail<string>("Chưa chọn người nhận");

            if (string.IsNullOrWhiteSpace(dto.Subject) || string.IsNullOrWhiteSpace(dto.Content))
                return ApiResponse.Fail<string>("Thiếu tiêu đề hoặc nội dung email");

            var users = await _db.Users
                .Where(u => ids.Contains(u.UserId))
                .ToListAsync();

            if (!users.Any())
                return ApiResponse.Fail<string>("Không tìm thấy người dùng phù hợp");

            var notifications = new List<EmailNotification>();

            foreach (var u in users)
            {
                try
                {
                    await _emailService.SendAsync(u.Email, dto.Subject, dto.Content);

                    notifications.Add(new EmailNotification
                    {
                        UserId = u.UserId,
                        Subject = dto.Subject,
                        Content = dto.Content,
                        Sent = true,
                        SendTime = DateTime.Now,
                        SentAt = DateTime.Now,
                    });
                }
                catch
                {
                    // Lưu lại cả case fail
                    notifications.Add(new EmailNotification
                    {
                        UserId = u.UserId,
                        Subject = dto.Subject,
                        Content = dto.Content,
                        Sent = false,
                        SendTime = DateTime.Now,
                    });
                }
            }

            if (notifications.Any())
            {
                _db.EmailNotifications.AddRange(notifications);
                await _db.SaveChangesAsync();
            }

            return ApiResponse.Success($"Đã xử lý gửi email cho {users.Count} người dùng.");
        }
    }
}
