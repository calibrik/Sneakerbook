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
    public async Task<List<Race>> GetRacesAsync()
    {
        return await _context.Races.ToListAsync();
    }
    public async Task<List<Race>> GetUserRacesAsyncRO(string id)
    {
        return await _context.Races.AsNoTracking().Include(r=>r.Club).Where(u=>u.AppUserId==id).ToListAsync();
    }
    public async Task<List<Race>> GetRacesAsyncRO()
    {
        return await _context.Races.AsNoTracking().ToListAsync();
    }

    public async Task<Race?> GetRaceByIdAsync(int id,bool withAddress)
    {
        if (!withAddress)
            return await _context.Races.FirstOrDefaultAsync(a=>a.Id == id);
        return await _context.Races.Include(c=>c.Address).FirstOrDefaultAsync(a=>a.Id == id);
    }
    public async Task<Race?> GetRaceByIdAsyncRO(int id,bool withAddress)
    {
        if (!withAddress)
            return await _context.Races.AsNoTracking().FirstOrDefaultAsync(a=>a.Id == id);
        return await _context.Races.Include(c=>c.Address).AsNoTracking().FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<List<Race>> GetRacesByCityAsyncRO(string city)
    {
        return await _context.Races.AsNoTracking().Where(c=>c.Address.City.Contains(city)).ToListAsync();
    }

    public async Task<bool> AddRace(Race race)
    {
        _context.Races.Add(race);
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