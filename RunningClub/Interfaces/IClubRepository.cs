using RunningClub.Models;

namespace RunningClub.Interfaces;

public interface IClubRepository
{
    Task<List<Club>> GetClubsAsyncRO();
    Task<List<Club>> GetClubsAsync();
    Task<Club?> GetClubByIdAsync(int id,bool withAddress);
    Task<Club?> GetClubByIdAsyncRO(int id,bool withAddress);
    Task<List<Club>> GetClubsByCityAsync(string city);
    Task<bool> AddClub(Club club);
    // bool UpdateClub(Club club);
    Task<bool> DeleteClub(Club club);
    Task<bool> Save();
}