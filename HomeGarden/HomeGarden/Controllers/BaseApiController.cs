using HomeGarden.Dtos.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeGarden.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected long? CurrentUserId =>
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (long?)null;

        protected string? CurrentRole => User.FindFirstValue(ClaimTypes.Role);

        protected ActionResult<ApiResponse<T>> OkData<T>(T data, string? msg = null)
            => Ok(ApiResponse<T>.Success(data, msg));

        protected ActionResult<ApiResponse<T>> Fail<T>(string msg, int code = 400)
            => StatusCode(code, ApiResponse<T>.Fail(msg));
    }
}
