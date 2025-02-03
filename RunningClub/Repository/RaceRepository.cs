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

    public async Task<bool> IsUserMemberInRace(string userId, int raceId)
    {
        MemberRace? mr=await _context.MemberRaces.AsNoTracking().Where(mr => mr.RaceId == raceId&&mr.MemberId==userId).FirstOrDefaultAsync();
        return mr != null;
    }
    public async Task<List<Race>> GetRacesAsync()
    {
        return await _context.Races.ToListAsync();
    }

    public async Task<bool> RemoveUserFromRace(string userId, int raceId)
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
    public async Task<List<Race>> GetUserRacesAsyncRO(string id)
    {
        return await _context.MemberRaces.AsNoTracking().Where(mr=>mr.MemberId==id).Select(mr=>mr.Race).ToListAsync();
    }
    public async Task<List<Race>> GetRacesAsyncRO()
    {
        return await _context.Races.AsNoTracking().ToListAsync();
    }

    public async Task<HashSet<int>> GetUserRacesIdsAsyncRO(string userId)
    {
        return await _context.MemberRaces.AsNoTracking().Where(mr => mr.MemberId == userId).Select(mr=>mr.RaceId).ToHashSetAsync();
    }
    public async Task<Race?> GetRaceByIdAsync(int id)
    {
        return await _context.Races.FirstOrDefaultAsync(a=>a.Id == id);
    }
    public async Task<Race?> GetRaceByIdAsyncRO(int id)
    {
        return await _context.Races.AsNoTracking().Include(r=>r.Admin).Include(r=>r.Club).FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<List<Race>> GetRacesByCityAsyncRO(string city)
    {
        return await _context.Races.AsNoTracking().Where(c=>c.Address.City.Contains(city)).ToListAsync();
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