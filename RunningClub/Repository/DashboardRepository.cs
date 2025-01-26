using System.Security.Claims;
using RunningClub.Models;

namespace RunningClub.Repository;

public class DashboardRepository
{
    private readonly RaceRepository _raceRepository;
    private readonly ClubRepository _clubRepository;

    public DashboardRepository(RaceRepository raceRepository, ClubRepository clubRepository)
    {
        _raceRepository = raceRepository;
        _clubRepository = clubRepository;
    }

    public async Task<List<Race>> GetAllUserRacesAsync(string userId)
    {
        return await _raceRepository.GetUserRacesAsyncRO(userId);
    }
    public async Task<List<Club>> GetAllUserClubsAsync(string userId)
    {
        return await _clubRepository.GetUserClubsAsyncRO(userId);
    }
}