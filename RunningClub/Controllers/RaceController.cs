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
            JoinedRaces = await _raceRepo.GetUserRacesIdsAsyncRO(User.GetUserId()),
        };
        List<Race> races = await _clubRepo.GetClubsRacesAsyncRO(userClubs);
        foreach (Race race in races)
        {
            model.Races.Add(new IndexRaceViewModel.IndexRaceModel(race)
            {
                MemberCount = await _raceRepo.GetRaceMemberCountAsyncRO(race.Id)
            });
        }
        return View(model);
    }
    public async Task<IActionResult> Create(int clubId)
    {
        if (!await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), clubId)&&!User.IsInRole("Admin"))
            return RedirectToAction("Detail","Club", new { id = clubId });
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        CreateRaceViewModel model = new CreateRaceViewModel()
        {
            ClubId = clubId
        };
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
    [HttpPost]
    public async Task<IActionResult> Create(CreateRaceViewModel createRaceModel)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!await _clubRepo.IsUserAdminInClubAsync(User.GetUserId(), createRaceModel.ClubId)&&!User.IsInRole("Admin"))
            return RedirectToAction("Detail","Club", new { clubId = createRaceModel.ClubId });
        if (!ModelState.IsValid) 
            return View(createRaceModel);
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
        };
        await _raceRepo.AddRace(race);
        await _raceRepo.AddUserToRaceAsync(User.GetUserId(), race.Id);
        return RedirectToAction("Detail","Club", new { clubId = race.ClubId });
    }
    
    public async Task<IActionResult> Edit(int raceId)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!await _raceRepo.IsUserAdminInRaceAsync(User.GetUserId(), raceId)&&!User.IsInRole("Admin"))
            return RedirectToAction("Detail", new { id = raceId });
        Race? club = await _raceRepo.GetRaceByIdAsyncRO(raceId);
        if (club==null)
            return View("Error");
        EditRaceViewModel editClubModel = new EditRaceViewModel
        {
            Id = club.Id,
            Title = club.Title,
            Description = club.Description,
            Address = club.Address,
            Category = club.Category,
            MaxMembersNumber = club.MaxMembersNumber,
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
        await _raceRepo.Save();
        return RedirectToAction("Detail",new {raceId=race.Id});
    }
}