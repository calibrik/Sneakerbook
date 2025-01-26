using Microsoft.EntityFrameworkCore;
using RunningClub.Interfaces;
using RunningClub.Models;

namespace RunningClub.Repository;

public class ClubRepository
{
    private readonly AppDbContext _context;
    public ClubRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Club>> GetClubsAsync()
    {
        return await _context.Clubs.ToListAsync();
    }
    public async Task<List<Club>> GetUserClubsAsyncRO(string id)
    {
        return await _context.Clubs.AsNoTracking().Where(u=>u.AppUserId==id).ToListAsync();
    }
    public async Task<List<Club>> GetClubsAsyncRO()
    {
        return await _context.Clubs.AsNoTracking().ToListAsync();
    }

    public async Task<Club?> GetClubByIdAsync(int id,bool withAddress)
    {
        if (!withAddress)
            return await _context.Clubs.FirstOrDefaultAsync(a=>a.Id == id);
        return await _context.Clubs.Include(a=>a.Address).FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<Club?> GetClubByIdAsyncRO(int id, bool withAddress)
    {
        if (!withAddress)
            return await _context.Clubs.AsNoTracking().FirstOrDefaultAsync(a=>a.Id == id);
        return await _context.Clubs.AsNoTracking().Include(a=>a.Address).AsNoTracking().FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<List<Club>> GetClubsByCityAsync(string city)
    {
        return await _context.Clubs.Where(c=>
            c.Address!=null&&c.Address.City.Contains(city)
        ).ToListAsync();
    }

    public async Task<bool> AddClub(Club club)
    {
        _context.Clubs.Add(club);
        return await Save();
    }

    // public bool UpdateClub(Club club)
    // {
    //     _context.Update(club);
    //     return Save();
    // }

    public async Task<bool> DeleteClub(Club club)
    {
        _context.Clubs.Remove(club);
        return await Save();
    }

    public async Task<bool> Save()
    {
        int saved=await _context.SaveChangesAsync();
        return saved>0;
    }
}