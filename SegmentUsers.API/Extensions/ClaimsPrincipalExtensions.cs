using System.Security.Claims;

namespace SegmentUsers.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    => Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}