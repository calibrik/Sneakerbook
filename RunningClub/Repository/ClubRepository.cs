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
        return await _context.MemberClubs.AsNoTracking().Where(mc => mc.MemberId==id).Select(mc => mc.Club).ToListAsync();
    }

    public async Task<HashSet<int>> GetUserClubsIdsAsyncRO(string id)
    {
        return await _context.MemberClubs.AsNoTracking().Where(mc => mc.MemberId==id).Select(mc => mc.ClubId).ToHashSetAsync();
    }

    public async Task<bool> RemoveUserFromClubAsync(string userId, int clubId)
    {
        MemberClub? mc=await _context.MemberClubs.Where(mc => mc.ClubId == clubId&&mc.MemberId==userId).FirstOrDefaultAsync();
        if (mc == null)
            return true;
        _context.MemberClubs.Remove(mc);
        return await Save();
    }
    public async Task<List<Club>> GetClubsAsyncRO()
    {
        return await _context.Clubs.AsNoTracking().ToListAsync();
    }

    public async Task<Club?> GetClubByIdAsync(int id)
    {
        return await _context.Clubs.FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<Club?> GetClubByIdAsyncRO(int id)
    {
        return await _context.Clubs.AsNoTracking().Include(c=>c.Admin).FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<bool> AddUserToClubAsync(string userId,int clubId)
    {
        MemberClub mc = new MemberClub()
        {
            ClubId = clubId,
            MemberId = userId
        };
        await _context.MemberClubs.AddAsync(mc);
        return await Save();
    }
    public async Task<bool> IsUserMemberInClubAsync(string userId, int clubId)
    {
        MemberClub? mc=await _context.MemberClubs.AsNoTracking().Where(mc => mc.ClubId == clubId&&mc.MemberId==userId).FirstOrDefaultAsync();
        return mc != null;
    }
    public async Task<List<Club>> GetClubsByCityAsync(string city)
    {
        return await _context.Clubs.Where(c=>
            c.Address.City.Contains(city)
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