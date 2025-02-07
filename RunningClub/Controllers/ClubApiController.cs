using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Repository;
using RunningClub.ViewModels.ClubApi;

namespace RunningClub.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClubApiController:ControllerBase
{
    private readonly ClubRepository _clubRepo;
    private readonly RaceRepository _raceRepo;

    public ClubApiController(ClubRepository clubRepo, RaceRepository raceRepo)
    {
        _clubRepo = clubRepo;
        _raceRepo = raceRepo;
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> KickMember([FromBody] KickMemberViewModel model)
    {
        if (await _clubRepo.GetClubByIdAsync(model.ClubId)==null)
            return NotFound("Club not found");
        if (!await _clubRepo.IsUserMemberInClubAsync(model.UserId, model.ClubId))
            return NotFound("User not found in this club");
        await _clubRepo.RemoveUserFromClubAsync(model.UserId, model.ClubId);
        return Ok("User kicked");
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Join(string userId, int clubId)
    {
        return Ok();
    }
}