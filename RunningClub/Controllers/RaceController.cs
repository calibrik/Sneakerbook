using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.Services;
using RunningClub.ViewModels;

namespace RunningClub.Controllers;

public class RaceController: Controller
{
    private readonly RaceRepository _raceRepo;
    private readonly PhotoService _photoService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ClubRepository _clubRepo;
    public RaceController(RaceRepository raceRepository, PhotoService photoService, UserManager<AppUser> userManager, ClubRepository clubRepo)
    {
        _photoService = photoService;
        _raceRepo = raceRepository;
        _userManager = userManager;
        _clubRepo = clubRepo;
    }

    public async Task<IActionResult> Detail(int raceId)
    {
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(raceId);
        if (race == null)
            return RedirectToAction("Index","Home");
        DetailRaceViewModel model = new DetailRaceViewModel(race)
        {
            Members = await _raceRepo.GetUsersInRaceAsyncRO(raceId),
        };
        if (User.Identity.IsAuthenticated)
        {
            model.IsAdmin=await _raceRepo.IsUserAdminInRaceAsync(User.GetUserId(),raceId);
            if (model.IsAdmin)
                model.IsJoined = true;
            else
                model.IsJoined = await _raceRepo.IsUserMemberInRaceAsync(User.GetUserId(), raceId);
        }
        else
        {
            model.IsJoined = false;
            model.IsAdmin = false;
        }
        return View(model);
    }
    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        HashSet<int> userClubs=await _clubRepo.GetUserClubsIdsAsyncRO(User.GetUserId());
        IndexRaceViewModel model = new IndexRaceViewModel()
        {
            JoinedRaces = await _raceRepo.GetUserUpcomingRacesIdsAsyncRO(User.GetUserId()),
        };
        List<Race> races = await _raceRepo.GetClubsUpcomingRacesAsyncRO(userClubs);
        foreach (Race race in races)
        {
            model.Races.Add(new IndexRaceViewModel.IndexRaceModel(race)
            {
                MemberCount = await _raceRepo.GetRaceMemberCountAsyncRO(race.Id)
            });
        }
        return View(model);
    }
    async Task<string?> ProcessImageAdd(IFormFile file)
    {
        PhotoService.UploadResult res= await _photoService.AddPhotoAsync(file,PhotoService.ImageType.Race);
        switch (res.Code)
        {
            case PhotoService.UploadResultCode.Success:
            {
                return res.Path;
            }
            case PhotoService.UploadResultCode.WrongExt:
            {
                ModelState.AddModelError("Image","Only .jpg, .png, .gif, .bmp allowed");
                break;
            }
            case PhotoService.UploadResultCode.Unknown:
            {
                ModelState.AddModelError("Image","Unknown error occured.");
                break;
            }
        }

        return null;
    }
    public async Task<IActionResult> Create(int clubId)
    {
        if (!await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), clubId)&&!User.IsInRole("Admin"))
            return RedirectToAction("Detail","Club", new { id = clubId });
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        DateTime date = DateTime.Now.ToLocalTime();
        CreateRaceViewModel model = new CreateRaceViewModel()
        {
            ClubId = clubId,
            StartDate = date.Date,
            StartTime = new DateTime(2000,1,1,date.Hour,date.Minute,0),
        };
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateRaceViewModel createRaceModel)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), createRaceModel.ClubId)&&!User.IsInRole("Admin"))
            return RedirectToAction("Detail","Club", new { clubId = createRaceModel.ClubId });
        if (!ModelState.IsValid) 
            return View(createRaceModel);
        DateTime todayDate = DateTime.Now.ToLocalTime();
        DateTime startDate = (createRaceModel.StartDate+createRaceModel.StartTime.TimeOfDay);
        if (startDate.Date < todayDate.Date)
        {
            ModelState.AddModelError("StartDate", "Start date should be not sooner than today.");
            return View("Create",createRaceModel);
        }
        if (startDate.Date == todayDate.Date && startDate.TimeOfDay < todayDate.TimeOfDay)
        {
            ModelState.AddModelError("StartTime", "Start time should be not sooner than right now.");
            return View("Create",createRaceModel);
        }
        
        switch (createRaceModel.Category)
        {
            case RaceCategory.Sprint:
            {
                if (createRaceModel.Length < 0.1 || createRaceModel.Length > 5)
                {
                    ModelState.AddModelError("Length", "Length for sprint category must be between 0.1km and 5km.");
                    return View("Create",createRaceModel);
                }
                break;
            }
            case RaceCategory.Marathon:
            {
                if (createRaceModel.Length <= 21 || createRaceModel.Length > 42)
                {
                    ModelState.AddModelError("Length", "Length for marathon category must be between 21km and 42km.");
                    return View("Create",createRaceModel);
                }
                break;
            }
            case RaceCategory.HalfMarathon:
            {
                if (createRaceModel.Length <= 5 || createRaceModel.Length > 21)
                {
                    ModelState.AddModelError("Length", "Length for sprint category must be between 5km and 21km.");
                    return View("Create",createRaceModel);
                }
                break;
            }
            case RaceCategory.Ultramarathon:
            {
                if (createRaceModel.Length <= 42 || createRaceModel.Length > 80)
                {
                    ModelState.AddModelError("Length", "Length for sprint category must be between 42km and 80km.");
                    return View("Create",createRaceModel);
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        string? path = await ProcessImageAdd(createRaceModel.Image);
        if (path==null)
            return View(createRaceModel);
        
        Race race = new Race
        {
            AdminId = User.GetUserId(),
            Title = createRaceModel.Title,
            Description = createRaceModel.Description,
            Image = path,
            Address = createRaceModel.Address,
            Category = createRaceModel.Category,
            MaxMembersNumber = createRaceModel.MaxMembersNumber,
            ClubId = createRaceModel.ClubId,
            StartDate = startDate.ToUniversalTime(),
            Length = createRaceModel.Length,
        };
        await _raceRepo.AddRace(race);
        await _raceRepo.AddUserToRaceAsync(User.GetUserId(), race.Id);
        return RedirectToAction("Detail", new { raceId = race.Id });
    }
    
    public async Task<IActionResult> Edit(int raceId)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!await _raceRepo.IsUserAdminInRaceAsync(User.GetUserId(), raceId)&&!User.IsInRole("Admin"))
            return RedirectToAction("Detail", new { id = raceId });
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(raceId);
        if (race==null)
            return View("Error");
        DateTime date = race.StartDate.ToLocalTime();
        EditRaceViewModel editClubModel = new EditRaceViewModel
        {
            Id = race.Id,
            Title = race.Title,
            Description = race.Description,
            Address = race.Address,
            Category = race.Category,
            MaxMembersNumber = race.MaxMembersNumber,
            StartDate = date.Date,
            StartTime = new DateTime(2000,1,1,date.Hour,date.Minute,0),
            Length = race.Length,
        };
        return View(editClubModel);
    }

    
    
    [HttpPost]
    public async Task<IActionResult> Edit(EditRaceViewModel editRaceModel)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!await _raceRepo.IsUserAdminInRaceAsync(User.GetUserId(), editRaceModel.Id)&&!User.IsInRole("Admin"))
            return RedirectToAction("Detail", new { id = editRaceModel.Id });
        if (!ModelState.IsValid)
            return View("Edit",editRaceModel);
        DateTime todayDate = DateTime.Now.ToLocalTime();
        DateTime startDate = (editRaceModel.StartDate+editRaceModel.StartTime.TimeOfDay);
        if (startDate.Date < todayDate.Date)
        {
            ModelState.AddModelError("StartDate", "Start date should be not sooner than today.");
            return View("Edit",editRaceModel);
        }
        if (startDate.Date == todayDate.Date && startDate.TimeOfDay < todayDate.TimeOfDay)
        {
            ModelState.AddModelError("StartTime", "Start time should be not sooner than right now.");
            return View("Edit",editRaceModel);
        }
        switch (editRaceModel.Category)
        {
            case RaceCategory.Sprint:
            {
                if (editRaceModel.Length < 0.1 || editRaceModel.Length > 5)
                {
                    ModelState.AddModelError("Length", "Length for sprint category must be between 0.1km and 5km.");
                    return View("Edit",editRaceModel);
                }
                break;
            }
            case RaceCategory.Marathon:
            {
                if (editRaceModel.Length <= 21 || editRaceModel.Length > 42)
                {
                    ModelState.AddModelError("Length", "Length for marathon category must be between 21km and 42km.");
                    return View("Edit",editRaceModel);
                }
                break;
            }
            case RaceCategory.HalfMarathon:
            {
                if (editRaceModel.Length <= 5 || editRaceModel.Length > 21)
                {
                    ModelState.AddModelError("Length", "Length for sprint category must be between 5km and 21km.");
                    return View("Edit",editRaceModel);
                }
                break;
            }
            case RaceCategory.Ultramarathon:
            {
                if (editRaceModel.Length <= 42 || editRaceModel.Length > 80)
                {
                    ModelState.AddModelError("Length", "Length for sprint category must be between 42km and 80km.");
                    return View("Edit",editRaceModel);
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        Race? race = await _raceRepo.GetRaceByIdAsync(editRaceModel.Id);
        if (race==null)
            return View("Error");
        if (await _raceRepo.GetRaceMemberCountAsyncRO(race.Id) > editRaceModel.MaxMembersNumber)
        {
            ModelState.AddModelError("MaxMembersNumber","Max Member Number can't be less than current member number.");
            return View(editRaceModel);
        }
        if (editRaceModel.IsImageChanged)
        {
            if (editRaceModel.Image == null)
            {
                ModelState.AddModelError("Image","Please choose a image");
                return View(editRaceModel);
            }
            string? path = await ProcessImageAdd(editRaceModel.Image);
            if (path == null)
                return View(editRaceModel);
            _photoService.DeletePhoto(race.Image);
            race.Image = path;
        }
        race.Title = editRaceModel.Title;
        race.Description = editRaceModel.Description;
        race.Address.City = editRaceModel.Address.City;
        race.Address.Country = editRaceModel.Address.Country;
        race.Address.Street = editRaceModel.Address.Street;
        race.Category = editRaceModel.Category;
        race.MaxMembersNumber = editRaceModel.MaxMembersNumber;
        race.StartDate = startDate.ToUniversalTime();
        race.Length = editRaceModel.Length;
        await _raceRepo.Save();
        return RedirectToAction("Detail",new {raceId=race.Id});
    }
}