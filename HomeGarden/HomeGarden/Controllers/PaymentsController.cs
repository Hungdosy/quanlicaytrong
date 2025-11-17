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
    public class PaymentsController : BaseApiController
    {
        private readonly HomeGardenDbContext _db;
        public PaymentsController(HomeGardenDbContext db) => _db = db;

        // 🔹 Lấy danh sách thanh toán của user
        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<List<SubscriptionPaymentDto>>>> MyPayments()
        {
            var uid = CurrentUserId ?? 0;
            var payments = await _db.SubscriptionPayments
                .Include(p => p.Plan)
                .Include(p => p.Provider)
                .Where(p => p.UserId == uid)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new SubscriptionPaymentDto
                {
                    PaymentId = p.PaymentId,
                    PlanName = p.Plan.Name,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PaymentStatus = p.PaymentStatus,
                    PaymentMethod = p.PaymentMethod,
                    ProviderName = p.Provider != null ? p.Provider.Name : null
                })
                .ToListAsync();

            return ApiResponse.Success(payments);
        }

        // 🔹 Admin xem toàn bộ giao dịch
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<SubscriptionPaymentDto>>>> AllPayments()
        {
            var list = await _db.SubscriptionPayments
                .Include(p => p.User)
                .Include(p => p.Plan)
                .Include(p => p.Provider)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new SubscriptionPaymentDto
                {
                    PaymentId = p.PaymentId,
                    PlanName = p.Plan.Name,
                    UserEmail = p.User.Email,
                    Amount = p.Amount,
                    PaymentStatus = p.PaymentStatus,
                    PaymentMethod = p.PaymentMethod,
                    ProviderName = p.Provider != null ? p.Provider.Name : null,
                    PaymentDate = p.PaymentDate
                })
                .ToListAsync();

            return ApiResponse.Success(list);
        }

        // 🔹 Tạo bản ghi thanh toán (khi user thực hiện thanh toán)
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] PaymentCreateDto dto)
        {
            var uid = CurrentUserId ?? 0;
            var subscription = await _db.UserSubscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.SubscriptionId == dto.SubscriptionId && s.UserId == uid);

            if (subscription == null)
                return ApiResponse.Fail<object>("Không tìm thấy gói đăng ký");

            var payment = new SubscriptionPayment
            {
                SubscriptionId = dto.SubscriptionId,
                UserId = uid,
                PlanId = subscription.PlanId,
                ProviderId = dto.ProviderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Pending",
                CreatedAt = DateTime.Now
            };
            _db.SubscriptionPayments.Add(payment);
            await _db.SaveChangesAsync();

            return ApiResponse.Success((object)new { payment.PaymentId }, "Tạo giao dịch thành công");
        }

        // 🔹 Admin cập nhật trạng thái thanh toán (callback hoặc kiểm duyệt)
        [HttpPut("{id:long}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateStatus(long id, [FromBody] PaymentStatusUpdateDto dto)
        {
            var payment = await _db.SubscriptionPayments.FirstOrDefaultAsync(p => p.PaymentId == id);
            if (payment == null) return ApiResponse.Fail<string>("Không tìm thấy thanh toán");

            payment.PaymentStatus = dto.Status;
            payment.ProviderRef = dto.ProviderRef;
            payment.UpdatedAt = DateTime.Now;

            // Nếu success → cập nhật gói user
            if (dto.Status == "Paid")
            {
                var sub = await _db.UserSubscriptions.FirstAsync(s => s.SubscriptionId == payment.SubscriptionId);
                sub.LastPaymentId = id;
                sub.NextBillingDate = DateTime.Now.AddMonths(1);
                sub.Status = "Active";
                _db.UserNotifications.Add(new UserNotification
                {
                    UserId = payment.UserId,
                    Channel = "WEB",
                    Type = "PAYMENT",
                    Title = "Thanh toán thành công",
                    Content = $"Bạn đã thanh toán thành công {payment.Amount:N0} VND cho gói {sub.PlanId}",
                    SentAt = DateTime.Now
                });
            }

            await _db.SaveChangesAsync();
            return ApiResponse.Success("Cập nhật trạng thái thanh toán thành công");
        }
    }
}
