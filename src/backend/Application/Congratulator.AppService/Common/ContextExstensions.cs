using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Congratulator.AppService.Common;

public static class ContextExstensions
{
    public static string? GetRoleFromContext(this HttpContext httpContext)
        => httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? null;

    public static Guid GetUserIdFromContext(this HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out Guid userId) ? userId : Guid.Empty;
    }
}
