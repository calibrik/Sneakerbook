using Microsoft.EntityFrameworkCore;
using RunningClub.Interfaces;
using RunningClub.Models;

namespace RunningClub.Repository;

public class RaceRepository
{
    private readonly AppDbContext _context;
    public RaceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsUserMemberInRaceAsync(string userId, int raceId)
    {
        MemberRace? mr=await _context.MemberRaces.AsNoTracking().Where(mr => mr.RaceId == raceId&&mr.MemberId==userId).FirstOrDefaultAsync();
        return mr != null;
    }
    public async Task<int> GetRaceMemberCountAsyncRO(int raceId)
    {
        return await _context.MemberRaces.AsNoTracking().Where(mc => mc.RaceId == raceId).CountAsync();
    }
    public async Task<bool> RemoveUserFromRaceAsync(string userId, int raceId)
    {
        MemberRace? mr=await _context.MemberRaces.Where(mr=>mr.RaceId==raceId&&mr.MemberId==userId).FirstOrDefaultAsync();
        if (mr == null)
            return true;
        _context.MemberRaces.Remove(mr);
        return await Save();
    }
    public async Task<bool> AddUserToRaceAsync(string userId, int raceId)
    {
        MemberRace mr = new MemberRace()
        {
            MemberId = userId,
            RaceId = raceId
        };
        await _context.MemberRaces.AddAsync(mr);
        return await Save();
    }
    public async Task<List<AppUser>> GetUsersInRaceAsync(int raceId)
    {
        return await _context.MemberRaces.Include(mr=>mr.Member).Where(mr => mr.RaceId == raceId).Select(mr=>mr.Member).ToListAsync();
    }
    public async Task<List<AppUser>> GetUsersInRaceAsyncRO(int raceId)
    {
        return await _context.MemberRaces.AsNoTracking().Include(mr=>mr.Member).Where(mr => mr.RaceId == raceId).Select(mr=>mr.Member).ToListAsync();
    }
    public async Task<List<Race>> GetUserUpcomingRacesAsyncRO(string userId)
    {
        return await _context.MemberRaces.AsNoTracking().Where(mr=>mr.MemberId==userId&&!mr.Race.IsCompleted&&mr.Race.AdminId!=userId).Include(mr=>mr.Race.Club).OrderBy(mr=>mr.Race.StartDate).Select(mr=>mr.Race).ToListAsync();
    }

    public async Task<List<Race>> GetToBeCompletedRacesAsync()
    {
        return await _context.Races.Where(r => !r.IsCompleted&&r.StartDate<=DateTime.UtcNow).ToListAsync();
    }
    public async Task<List<Race>> GetUserAdminUpcomingRacesAsyncRO(string userId)
    {
        return await _context.Races.AsNoTracking().Where(r=>r.AdminId==userId&&!r.IsCompleted).Include(r=>r.Club).OrderBy(r=>r.StartDate).ToListAsync();
    }
    
    public async Task<List<Race>> GetClubsUpcomingRacesAsyncRO(HashSet<int> clubIds)
    {
        return await _context.Races.AsNoTracking().Include(r=>r.Club).Where(r => clubIds.Contains(r.ClubId)&&!r.IsCompleted).OrderBy(r=>r.StartDate).ToListAsync();
    }
    public async Task<List<Race>> GetClubUpcomingRacesAsyncRO(int clubId)
    {
        return await _context.Races.AsNoTracking().Where(r => r.ClubId == clubId&&!r.IsCompleted).OrderBy(r=>r.StartDate).ToListAsync();
    }
    public async Task<List<Race>> GetClubCompletedRacesAsyncRO(int clubId)
    {
        return await _context.Races.AsNoTracking().Where(r => r.ClubId == clubId&&r.IsCompleted).OrderBy(r=>r.StartDate).ToListAsync();
    }
    public async Task<HashSet<int>> GetUserUpcomingRacesIdsAsyncRO(string userId)
    {
        return await _context.MemberRaces.AsNoTracking().Where(mr => mr.MemberId == userId&&!mr.Race.IsCompleted).Select(mr=>mr.RaceId).ToHashSetAsync();
    }
    public async Task<HashSet<int>> GetUserRacesIdsInClubAsyncRO(string userId,int clubId)
    {
        return await _context.MemberRaces.AsNoTracking().Where(mr => mr.MemberId == userId&&mr.Race.ClubId==clubId).Select(mr=>mr.RaceId).ToHashSetAsync();
    }
    public async Task<Race?> GetRaceByIdAsync(int id)
    {
        return await _context.Races.FirstOrDefaultAsync(a=>a.Id == id);
    }
    public async Task<Race?> GetRaceByIdAsyncRO(int id)
    {
        return await _context.Races.AsNoTracking().Include(r=>r.Admin).Include(r=>r.Club).FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<List<Race>> GetUserCompletedRacesAsyncRO(string userId)
    {
        return await _context.MemberRaces.AsNoTracking().Where(mr=>mr.MemberId==userId&&mr.Race.IsCompleted).Include(mr=>mr.Race.Club).OrderBy(mr=>mr.Race.StartDate).Select(mr=>mr.Race).ToListAsync();
    }
    public async Task<bool> AddRace(Race race)
    {
        await _context.Races.AddAsync(race);
        return await Save();
    }
    public async Task<bool> IsAnythingInTable()
    {
        return await _context.Races.FirstOrDefaultAsync()!=null;
    }

    public async Task<bool> IsUserAdminInRaceAsync(string userId, int raceId)
    {
        return await _context.Races.AsNoTracking().Where(r=>r.Id==raceId).Select(r=>r.AdminId).FirstOrDefaultAsync()==userId;
    }
    public async Task<bool> AddManyRacesAsync(List<Race> races)
    {
        await _context.Races.AddRangeAsync(races);
        return await Save();
    }
    public async Task<bool> DeleteRace(Race race)
    {
        _context.Races.Remove(race);
        return await Save();
    }

    public async Task<bool> Save()
    {
        int saved= await _context.SaveChangesAsync();
        return saved>0;
    }
}