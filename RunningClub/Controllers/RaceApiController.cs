using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.ViewModels;

namespace RunningClub.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class RaceApiController : ControllerBase
{
    private readonly RaceRepository _raceRepo;
    private readonly ClubRepository _clubRepo;

    public RaceApiController(RaceRepository raceRepo, ClubRepository clubRepo)
    {
        _clubRepo = clubRepo;
        _raceRepo = raceRepo;
    }

    [HttpPost("")]
    public async Task<IActionResult> Join([FromBody] int raceId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "You do not have permission for this action" });
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(raceId);
        if (race == null)
            return NotFound(new { message = "Race is not found" });
        if (!await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), race.ClubId))
            return Unauthorized(new { message = $"You need to be a member of club {race.Club.Title} to be able to join this race" });
        if (await _raceRepo.IsUserMemberInRaceAsync(User.GetUserId(), raceId))
            return BadRequest(new { message = "You are already in this race" });
        await _raceRepo.AddUserToRaceAsync(User.GetUserId(), raceId);
        return Ok(new { message = "You successfully joined the race" });
    }
    [HttpPost("")]
    public async Task<IActionResult> Leave([FromBody] int raceId)
    {
        if (!User.Identity.IsAuthenticated)
            return Unauthorized(new { message = "You do not have permission for this action" });
        if (await _raceRepo.GetRaceByIdAsync(raceId) == null)
            return NotFound(new { message = "Race is not found" });
        if (!await _raceRepo.IsUserMemberInRaceAsync(User.GetUserId(), raceId))
            return BadRequest(new { message = "You are not in this race" });
        await _raceRepo.RemoveUserFromRaceAsync(User.GetUserId(), raceId);
        return Ok(new { message = "You successfully left the race" });
    }
    [HttpPost("")]
    public async Task<IActionResult> KickMember([FromBody] KickRaceMemberViewModel model)
    {
        if (!User.Identity.IsAuthenticated || !await _raceRepo.IsUserAdminInRaceAsync(User.GetUserId(), model.RaceId) ||
            !User.IsInRole("Admin"))
            return Unauthorized(new { message = "You do not have permission for this action" });
        if (await _raceRepo.GetRaceByIdAsync(model.RaceId) == null)
            return NotFound(new { message = "Race is not found" });
        if (!await _raceRepo.IsUserMemberInRaceAsync(model.UserId, model.RaceId))
            return NotFound(new { message = "User is not found in this club" });
        await _raceRepo.RemoveUserFromRaceAsync(model.UserId, model.RaceId);
        return Ok(new { message = "User successfully kicked" });
    }
}