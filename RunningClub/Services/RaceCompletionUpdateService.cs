using RunningClub.Models;
using RunningClub.Repository;

namespace RunningClub.Services;

public class RaceCompletionUpdateService:BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RaceCompletionUpdateService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            RaceRepository _raceRepository = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<RaceRepository>();
            List<Race> races = await _raceRepository.GetToBeCompletedRacesAsync();
            foreach (Race race in races)
            {
                List<AppUser> members = await _raceRepository.GetUsersInRaceAsync(race.Id);
                foreach (AppUser user in members)
                {
                    user.Mileage = Math.Round(user.Mileage+race.Length,1);
                }
            
                race.IsCompleted = true;
            }
            await _raceRepository.Save();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}