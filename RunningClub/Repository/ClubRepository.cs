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

    public async Task<List<AppUser>> GetUsersInClubAsyncRO(int clubId)
    {
        return await _context.MemberClubs.AsNoTracking().Include(mc=>mc.Member).Where(mc => mc.ClubId == clubId).Select(mc=>mc.Member).ToListAsync();
    }
    public async Task<List<Club>> GetUserClubsAsyncRO(string userId)
    {
        return await _context.MemberClubs.AsNoTracking().Include(mc=>mc.Club.Admin).Where(mc => mc.MemberId==userId&&mc.Club.AdminId!=userId).Select(mc => mc.Club).ToListAsync();
    }

    public async Task<List<Club>> GetUserAdminsClubsAsyncRO(string userId)
    {
        return await _context.Clubs.AsNoTracking().Include(c=>c.Admin).Where(c=>c.AdminId==userId).ToListAsync();
    }
    public async Task<HashSet<int>> GetUserClubsIdsAsyncRO(string userId)
    {
        return await _context.MemberClubs.AsNoTracking().Where(mc => mc.MemberId==userId).Select(mc => mc.ClubId).ToHashSetAsync();
    }

    public async Task<bool> RemoveUserFromClubAsync(string userId, int clubId)
    {
        MemberClub? mc=await _context.MemberClubs.Where(mc => mc.ClubId == clubId&&mc.MemberId==userId).FirstOrDefaultAsync();
        if (mc == null)
            return true;
        _context.MemberClubs.Remove(mc);
        return await RemoveUserFromClubUpcomingRacesAsync(userId, clubId);
    }
    public async Task<bool> RemoveUserFromClubUpcomingRacesAsync(string userId, int clubId)
    {
        List<MemberRace> mrs = await _context.MemberRaces.AsNoTracking().Include(r=>r.Race).Where(mr => mr.MemberId == userId && mr.Race.ClubId==clubId&&!mr.Race.IsCompleted).ToListAsync();
        _context.MemberRaces.RemoveRange(mrs);
        return await Save();
    }
    public async Task<List<Club>> GetClubsAsyncRO()
    {
        return await _context.Clubs.AsNoTracking().Include(c=>c.Admin).ToListAsync();
    }
    public async Task<Club?> GetClubByIdAsync(int id)
    {
        return await _context.Clubs.Include(c=>c.Admin).FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<Club?> GetClubByIdAsyncRO(int id)
    {
        return await _context.Clubs.AsNoTracking().Include(c=>c.Admin).FirstOrDefaultAsync(a=>a.Id == id);
    }

    public async Task<int> GetClubMemberCountAsyncRO(int clubId)
    {
        return await _context.MemberClubs.AsNoTracking().Where(mc => mc.ClubId == clubId).CountAsync();
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

    public async Task<bool> IsUserAdminInClubAsync(string userId, int clubId)
    {
        string? AdminId=await _context.Clubs.AsNoTracking().Where(c=>c.Id==clubId).Select(c=>c.AdminId).FirstOrDefaultAsync();
        return AdminId == userId;
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
    
    public async Task<bool> AddManyClubs(List<Club> clubs)
    {
        _context.Clubs.AddRange(clubs);
        return await Save();
    }
    // public bool UpdateClub(Club club)
    // {
    //     _context.Update(club);
    //     return Save();
    // }
    public async Task<bool> IsAnythingInTable()
    {
        return await _context.Clubs.FirstOrDefaultAsync()!=null;
    }
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