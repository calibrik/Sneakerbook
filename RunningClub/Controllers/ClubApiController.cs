using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.ViewModels.ClubApi;

namespace RunningClub.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ClubApiController : ControllerBase
{
    private readonly ClubRepository _clubRepo;
    private readonly RaceRepository _raceRepo;

    public ClubApiController(ClubRepository clubRepo, RaceRepository raceRepo)
    {
        _clubRepo = clubRepo;
        _raceRepo = raceRepo;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> Delete([FromBody] int clubId)
    {
        if (!User.Identity.IsAuthenticated || !await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), clubId) ||
            !User.IsInRole("Admin"))
            return Unauthorized(new { message = "You do not have permission for this action" });
        Club? club = await _clubRepo.GetClubByIdAsync(clubId);
        if (club == null)
            return NotFound(new { message = "Club is not found" });
        await _clubRepo.DeleteClub(club);
        return Ok(new { message = "Club successfully deleted" });
    }
    
    [HttpPost("")]
    public async Task<IActionResult> KickMember([FromBody] KickClubMemberViewModel model)
    {
        if (!User.Identity.IsAuthenticated || !await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), model.ClubId) ||
            !User.IsInRole("Admin"))
            return Unauthorized(new { message = "You do not have permission for this action" });
        if (await _clubRepo.GetClubByIdAsync(model.ClubId) == null)
            return NotFound(new { message = "Club is not found" });
        if (!await _clubRepo.IsUserMemberInClubAsync(model.UserId, model.ClubId))
            return NotFound(new { message = "User is not found in this club" });
        await _clubRepo.RemoveUserFromClubAsync(model.UserId, model.ClubId);
        return Ok(new { message = "User successfully kicked" });
    }

    [HttpPost("")]
    public async Task<IActionResult> Join([FromBody] int clubId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "You do not have permission for this action" });
        if (await _clubRepo.GetClubByIdAsync(clubId) == null)
            return NotFound(new { message = "Club is not found" });
        if (await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), clubId))
            return BadRequest(new { message = "You are already in this club" });
        await _clubRepo.AddUserToClubAsync(User.GetUserId(), clubId);
        return Ok(new { message = "You successfully joined" });
    }

    [HttpPost("")]
    public async Task<IActionResult> Leave([FromBody] int clubId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "You do not have permission for this action" });
        if (await _clubRepo.GetClubByIdAsync(clubId) == null)
            return NotFound(new { message = "Club is not found" });
        if (!await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), clubId))
            return BadRequest(new { message = "You aren't in this club" });
        await _clubRepo.RemoveUserFromClubAsync(User.GetUserId(), clubId);
        return Ok(new { message = "You successfully left" });
    }
}