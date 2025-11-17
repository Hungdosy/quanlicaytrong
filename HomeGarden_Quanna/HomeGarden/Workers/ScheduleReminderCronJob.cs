using HomeGarden.Models;
using HomeGarden.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeGarden.Workers
{
    public class ScheduleReminderCronJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduleReminderCronJob> _logger;

        public ScheduleReminderCronJob(
            IServiceScopeFactory scopeFactory,
            ILogger<ScheduleReminderCronJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduleReminderCronJob started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndSendReminders(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when processing schedule reminders");
                }

                // chạy lại sau 30 giây
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("ScheduleReminderCronJob stopped");
        }

        private async Task CheckAndSendReminders(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HomeGardenDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

            var now = DateTime.Now;
            var upper = now.AddMinutes(1);   // trong vòng 1 phút tới

            // lấy schedule sắp tới trong vòng 1 phút
            var query = from s in db.Schedules
                        join p in db.Plants on s.PlantId equals p.PlantId
                        join a in db.Areas on p.AreaId equals a.AreaId
                        join u in db.Users on a.UserId equals u.UserId
                        where (s.IsDeleted == false || s.IsDeleted == null)
                              && s.Reminder == true
                              && s.NextDue > now
                              && s.NextDue <= upper
                        select new
                        {
                            ScheduleId = s.ScheduleId,
                            s.TaskType,
                            s.NextDue,
                            PlantName = p.Name,
                            AreaName = a.Name,
                            UserId = u.UserId,
                            UserName = u.Fullname,
                            UserEmail = u.Email
                        };

            var schedules = await query.ToListAsync(ct);
            if (!schedules.Any()) return;

            // lấy log email gần đây để tránh spam (nếu muốn)
            var recentSince = now.AddMinutes(-2); // trong 2 phút gần đây
            var recentNoti = await db.EmailNotifications
                .Where(n => n.SendTime >= recentSince)
                .Select(n => new { n.UserId, n.Subject, n.SendTime })
                .ToListAsync(ct);

            var sentKeys = new HashSet<string>(
                recentNoti.Select(n => $"{n.UserId}::{n.Subject}")
            );

            foreach (var s in schedules)
            {
                if (string.IsNullOrWhiteSpace(s.UserEmail))
                    continue;

                var subject = $"[HomeGarden] Nhắc lịch: {s.TaskType} cho cây {s.PlantName}";
                var key = $"{s.UserId}::{subject}";

                // nếu trong 2 phút gần đây đã gửi email cùng subject cho user này rồi → bỏ qua
                if (sentKeys.Contains(key))
                    continue;

                var content = $@"
Xin chào {s.UserName},

Trong khoảng 1 phút nữa (lúc {s.NextDue:HH:mm dd/MM/yyyy})
bạn có lịch ""{s.TaskType}"" cho cây ""{s.PlantName}"" ở khu vực ""{s.AreaName}"".

Vui lòng thực hiện đúng lịch để cây phát triển tốt hơn 🌿

— Hệ thống HomeGarden
";

                try
                {
                    await emailService.SendAsync(s.UserEmail, subject, content);

                    db.EmailNotifications.Add(new EmailNotification
                    {
                        UserId = s.UserId,
                        Subject = subject,
                        Content = content,
                        Sent = true,
                        SendTime = now,
                        SentAt = now
                    });

                    sentKeys.Add(key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Send reminder email failed for schedule {ScheduleId}", s.ScheduleId);

                    db.EmailNotifications.Add(new EmailNotification
                    {
                        UserId = s.UserId,
                        Subject = subject,
                        Content = content,
                        Sent = false,
                        SendTime = now
                    });
                }
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
