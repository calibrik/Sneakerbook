using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RunningClub.Models;

namespace RunningClub.Repository;

public class DashboardRepository
{
    private readonly RaceRepository _raceRepository;
    private readonly ClubRepository _clubRepository;
    private readonly AppDbContext _dbContext;

    public DashboardRepository(RaceRepository raceRepository, ClubRepository clubRepository, AppDbContext dbContext)
    {
        _raceRepository = raceRepository;
        _clubRepository = clubRepository;
        _dbContext=dbContext;
    }

    public async Task<List<Race>> GetUserUpcomingRacesAsyncRO(string userId)
    {
        return await _raceRepository.GetUserUpcomingRacesAsyncRO(userId);
    }
    public async Task<List<Club>> GetUserClubsAsyncRO(string userId)
    {
        return await _clubRepository.GetUserClubsAsyncRO(userId);
    }

    public async Task<List<Club>> GetUserAdminClubsAsyncRO(string userId)
    {
        return await _clubRepository.GetUserAdminsClubsAsyncRO(userId);
    }

    public async Task<List<Race>> GetUserAdminRacesAsyncRO(string userId)
    {
        return await _raceRepository.GetUserAdminUpcomingRacesAsyncRO(userId);
    }

    public async Task<List<Race>> GetUserCompletedRacesAsyncRO(string userId)
    {
        return await _raceRepository.GetUserCompletedRacesAsyncRO(userId);
    }
}