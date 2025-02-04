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

    public async Task<IActionResult> Detail(int id)
    {
        Race? race = await _raceRepo.GetRaceByIdAsyncRO(id);
        if (race == null)
            return RedirectToAction("Index");
        DetailRaceViewModel model = new DetailRaceViewModel(race);
        if (User.Identity.IsAuthenticated)
            model.IsJoined = await _raceRepo.IsUserMemberInRace(User.GetUserId(), id);
        else
            model.IsJoined = false;
        return View(model);
    }
    public async Task<IActionResult> Index()
    {
        HashSet<int> userClubs=await _clubRepo.GetUserClubsIdsAsyncRO(User.GetUserId());
        IndexRaceViewModel model = new IndexRaceViewModel()
        {
            Races = await _clubRepo.GetClubsRacesAsyncRO(userClubs)
        };
        if (User.Identity.IsAuthenticated)
            model.JoinedRaces = await _raceRepo.GetUserRacesIdsAsyncRO(User.GetUserId());
        return View(model);
    }
    public IActionResult Create()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        return View();
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
            MaxMembersNumber = 10,
            ClubId = createRaceModel.ClubId,
        };
        await _raceRepo.AddRace(race);
        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        Race? club = await _raceRepo.GetRaceByIdAsyncRO(id);
        if (club==null)
            return View("Error");
        EditRaceViewModel editClubModel = new EditRaceViewModel
        {
            Id = club.Id,
            Title = club.Title,
            Description = club.Description,
            Address = club.Address,
            Category = club.Category
        };
        return View(editClubModel);
    }

    [HttpPost]
    public async Task<IActionResult> Join(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        await _raceRepo.AddUserToRaceAsync(User.GetUserId(), id);
        return RedirectToAction("Detail", new { id = id });
    }
    [HttpPost]
    public async Task<IActionResult> Leave(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        await _raceRepo.RemoveUserFromRaceAsync(User.GetUserId(), id);
        return RedirectToAction("Detail", new { id = id });
    }
    [HttpPost]
    public async Task<IActionResult> Edit(EditRaceViewModel editRaceModel)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid)
            return View("Edit",editRaceModel);
        Race? race = await _raceRepo.GetRaceByIdAsync(editRaceModel.Id);
        if (race==null)
            return View("Error");
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
        await _raceRepo.Save();
        return RedirectToAction("Detail",new {id=race.Id});
    }
}