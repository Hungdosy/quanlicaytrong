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
    [Authorize]
    public class SchedulesController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        private readonly EmailService _emailService;
        public SchedulesController(HomeGardenDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ScheduleListDto>>>> List([FromQuery] long? plantId)
        {
            var query = _db.Schedules.AsNoTracking()
                .Include(s => s.Plant).ThenInclude(p => p.Area)
                .Include(s => s.Status)
                .Where(s => s.IsDeleted == false || s.IsDeleted == null);

            if (plantId.HasValue)
                query = query.Where(s => s.PlantId == plantId.Value);

            if (!User.IsInRole("Admin"))
                query = query.Where(s => s.Plant.Area.UserId == CurrentUserId);

            var list = await query
                .OrderBy(s => s.NextDue)
                .Select(s => new ScheduleListDto
                {
                    ScheduleId = s.ScheduleId,
                    PlantId = s.PlantId,
                    TaskType = s.TaskType,
                    Frequency = s.Frequency,
                    NextDue = s.NextDue,
                    LastDone = s.LastDone,
                    Status = s.Status.Code
                })
                .ToListAsync();

            return ApiResponse.Success(list);
        }


        [HttpPost]
        [Authorize(Roles = "User,Technician,Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] ScheduleCreateDto dto)
        {

            var plant = await _db.Plants
                .Include(p => p.Area)
                .FirstOrDefaultAsync(p => p.PlantId == dto.PlantId && (p.IsDeleted == false || p.IsDeleted == null));

            if (plant == null)
                return ApiResponse.Fail<object>("Cây không tồn tại");

            if (!User.IsInRole("Admin") && plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<object>("Bạn không có quyền tạo lịch cho cây này", 403);

            var pendingId = await _db.StatusDefinitions
                .Where(s => s.Entity == "Schedule" && s.Code == "Pending")
                .Select(s => s.StatusId)
                .FirstOrDefaultAsync();

            if (pendingId == 0)
                return ApiResponse.Fail<object>("Không tìm thấy trạng thái Pending cho Schedule");

            var count = dto.Count <= 0 ? 1 : dto.Count;
            if (count > 100)
                count = 100; 

            var schedules = new List<Schedule>();
            var next = dto.NextDue;

            for (int i = 0; i < count; i++)
            {
                schedules.Add(new Schedule
                {
                    PlantId = dto.PlantId,
                    TaskType = dto.TaskType.Trim(),
                    Frequency = dto.Frequency.Trim(),
                    NextDue = next,
                    StatusId = pendingId,
                    Reminder = true,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                });

                next = ComputeNextDue(next, dto.Frequency);
            }

            _db.Schedules.AddRange(schedules);
            await _db.SaveChangesAsync();

            return ApiResponse.Success(
                (object)new
                {
                    firstScheduleId = schedules.First().ScheduleId,
                    totalCreated = schedules.Count
                },
                $"Tạo {schedules.Count} lịch thành công"
            );
        }



        [HttpPost("{id:long}/done")]
        public async Task<ActionResult<ApiResponse<string>>> MarkDone(long id, [FromBody] ScheduleDoneDto dto)
        {
            var schedule = await _db.Schedules
                .Include(x => x.Plant).ThenInclude(p => p.Area)
                .FirstOrDefaultAsync(x => x.ScheduleId == id && (x.IsDeleted == false || x.IsDeleted == null));

            if (schedule == null)
                return ApiResponse.Fail<string>("Lịch không tồn tại");

            if (!User.IsInRole("Admin") && schedule.Plant.Area.UserId != CurrentUserId)
                return ApiResponse.Fail<string>("Bạn không có quyền thao tác lịch này", 403);

            // ✅ Cập nhật trạng thái Completed
            var completedId = await _db.StatusDefinitions
                .Where(x => x.Entity == "Schedule" && x.Code == "Completed")
                .Select(x => x.StatusId)
                .FirstOrDefaultAsync();

            schedule.LastDone = dto.DoneAt;
            schedule.StatusId = completedId;
            schedule.UpdatedAt = DateTime.Now;

            // ✅ Tạo lịch Pending kế tiếp
            var pendingId = await _db.StatusDefinitions
                .Where(x => x.Entity == "Schedule" && x.Code == "Pending")
                .Select(x => x.StatusId)
                .FirstOrDefaultAsync();

            var nextDue = ComputeNextDue(dto.DoneAt, schedule.Frequency);

            var nextSchedule = new Schedule
            {
                PlantId = schedule.PlantId,
                TaskType = schedule.TaskType,
                Frequency = schedule.Frequency,
                NextDue = nextDue,
                StatusId = pendingId,
                Reminder = schedule.Reminder,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _db.Schedules.Add(nextSchedule);
            await _db.SaveChangesAsync();

            return ApiResponse.Success("Đã đánh dấu hoàn thành và tạo lịch kế tiếp");
        }
        [HttpPost("check-reminders")]
        public async Task<ActionResult<ApiResponse<int>>> CheckReminders()
        {
            // user hiện tại (từ BaseApiController)
            var uid = CurrentUserId;
            if (uid == null)
                return ApiResponse.Fail<int>("Không xác định được user hiện tại", 401);

            var now = DateTime.Now;
            var upper = now.AddMinutes(1); // trong vòng 1 phút tới

            // chỉ lấy lịch của user hiện tại
            var schedules = await _db.Schedules
                .Include(s => s.Plant)
                    .ThenInclude(p => p.Area)
                .Where(s =>
                    (s.IsDeleted == false || s.IsDeleted == null) &&
                    s.Reminder == true &&
                    s.NextDue > now &&
                    s.NextDue <= upper &&
                    s.Plant.Area.UserId == uid       // 👈 chỉ lịch của user đang đăng nhập
                )
                .ToListAsync();

            if (!schedules.Any())
                return ApiResponse.Success(0, "Không có lịch nào cần nhắc.");

            // tất cả những user liên quan thực ra chỉ là 1 uid, nhưng viết chung cũng được
            var userIds = schedules
                .Select(s => s.Plant.Area.UserId)
                .Distinct()
                .ToList();

            var users = await _db.Users
                .Where(u => userIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId);

            var recentSince = now.AddMinutes(-2);
            var recentNoti = await _db.EmailNotifications
                .Where(n => n.SendTime >= recentSince && userIds.Contains(n.UserId))
                .Select(n => new { n.UserId, n.Subject, n.SendTime })
                .ToListAsync();

            var sentKeys = new HashSet<string>(
                recentNoti.Select(n => $"{n.UserId}::{n.Subject}")
            );

            int sentCount = 0;

            foreach (var s in schedules)
            {
                if (!users.TryGetValue(s.Plant.Area.UserId, out var user))
                    continue;

                if (string.IsNullOrWhiteSpace(user.Email))
                    continue;

                var subject = $"[HomeGarden] Nhắc lịch: {s.TaskType} cho cây {s.Plant.Name}";
                var key = $"{user.UserId}::{subject}";

                // tránh spam: 2 phút vừa rồi đã gửi cùng subject cho user này rồi thì bỏ
                if (sentKeys.Contains(key))
                    continue;

                var content = $@"
Xin chào {user.Fullname},

Trong khoảng 1 phút nữa (lúc {s.NextDue:HH:mm dd/MM/yyyy})
bạn có lịch ""{s.TaskType}"" cho cây ""{s.Plant.Name}"" ở khu vực ""{s.Plant.Area.Name}"". 

Vui lòng thực hiện đúng lịch để cây phát triển tốt hơn 🌿

— Hệ thống HomeGarden
";

                try
                {
                    await _emailService.SendAsync(user.Email, subject, content);

                    _db.EmailNotifications.Add(new EmailNotification
                    {
                        UserId = user.UserId,
                        Subject = subject,
                        Content = content,
                        Sent = true,
                        SendTime = now,
                        SentAt = now
                    });

                    sentKeys.Add(key);
                    sentCount++;
                }
                catch
                {
                    _db.EmailNotifications.Add(new EmailNotification
                    {
                        UserId = user.UserId,
                        Subject = subject,
                        Content = content,
                        Sent = false,
                        SendTime = now
                    });
                }
            }

            if (sentCount > 0)
                await _db.SaveChangesAsync();

            return ApiResponse.Success(sentCount, $"Đã gửi {sentCount} email nhắc lịch.");
        }

        private DateTime ComputeNextDue(DateTime from, string frequency)
        {
            frequency = frequency?.ToLower() ?? "daily";

            if (frequency == "daily") return from.AddDays(1);
            if (frequency == "weekly") return from.AddDays(7);
            if (frequency == "monthly") return from.AddMonths(1);

            if (frequency.StartsWith("every"))
            {
                var num = new string(frequency.Where(char.IsDigit).ToArray());
                if (int.TryParse(num, out var n) && n > 0)
                    return from.AddDays(n);
            }

            return from.AddDays(1);
        }
    }
}
