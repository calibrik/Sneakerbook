using System.Security.Claims;

namespace RunningClub.Misc;

public static class ClaimsPrincipalExt
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}