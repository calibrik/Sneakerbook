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

    public async Task<Race?> GetRaceByIdAsync(int id,bool withAddress)
    {
        if (!withAddress)
            return await _context.Races.FirstOrDefaultAsync(a=>a.Id == id);
        return await _context.Races.Include(c=>c.Address).FirstOrDefaultAsync(a=>a.Id == id);
    }
    public async Task<Race?> GetRaceByIdAsyncNoTracking(int id,bool withAddress)
    {
        if (!withAddress)
            return await _context.Races.AsNoTracking().FirstOrDefaultAsync(a=>a.Id == id);
        return await _context.Races.Include(c=>c.Address).AsNoTracking().FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<List<Race>> GetRacesByCityAsync(string city)
    {
        return await _context.Races.Where(c=>c.Address.City.Contains(city)).ToListAsync();
    }

    public bool AddRace(Race race)
    {
        _context.Races.Add(race);
        return Save();
    }

    public bool UpdateRace(Race race)
    {
        _context.Update(race);
        return Save();
    }

    public bool DeleteRace(Race race)
    {
        _context.Races.Remove(race);
        return Save();
    }

    public bool Save()
    {
        int saved= _context.SaveChanges();
        return saved>0;
    }
}