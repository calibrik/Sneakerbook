using RunningClub.Models;

namespace RunningClub.Interfaces;

public interface IClubRepository
{
    Task<List<Club>> GetClubsAsync();
    Task<Club?> GetClubByIdAsync(int id,bool withAddress);
    Task<Club?> GetClubByIdAsyncNoTracking(int id,bool withAddress);
    Task<List<Club>> GetClubsByCityAsync(string city);
    bool AddClub(Club club);
    bool UpdateClub(Club club);
    bool DeleteClub(Club club);
    bool Save();
}