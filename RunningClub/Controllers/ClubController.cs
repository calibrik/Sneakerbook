using Microsoft.AspNetCore.Mvc;
using RunningClub.Interfaces;
using RunningClub.Misc;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.Services;
using RunningClub.ViewModels;

namespace RunningClub.Controllers;

public class ClubController:Controller
{
    private readonly ClubRepository _clubRepo;
    private readonly PhotoService _photoService;

    public ClubController(ClubRepository clubRepository,PhotoService photoService)
    {
        _clubRepo = clubRepository;
        _photoService = photoService;
    }
    public async Task<IActionResult> Index()
    {
        IndexClubViewModel model = new IndexClubViewModel()
        {
            Clubs = await _clubRepo.GetClubsAsync(),
        };
        if (User.Identity.IsAuthenticated)
            model.JoinedClubs = await _clubRepo.GetUserClubsIdsAsyncRO(User.GetUserId());
        return View(model);
    }
    public IActionResult Create()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        return View();
    }
    public async Task<IActionResult> Detail(int id)
    {
        Club club = await _clubRepo.GetClubByIdAsyncRO(id);
        DetailClubViewModel model = new DetailClubViewModel(club);
        if (User.Identity.IsAuthenticated)
            model.IsJoined = await _clubRepo.IsUserMemberInClubAsync(User.GetUserId(), id);
        else
            model.IsJoined = false;
        return View(model);
    }

    async Task<string?> ProcessImageAdd(IFormFile file)
    {
        PhotoService.UploadResult res= await _photoService.AddPhotoAsync(file,PhotoService.ImageType.Club);
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
    public async Task<IActionResult> Create(CreateClubViewModel createClubModel)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid) 
            return View(createClubModel);
        string? path = await ProcessImageAdd(createClubModel.Image);
        if (path == null)
            return View(createClubModel);
        Club club = new Club
        {
            Title = createClubModel.Title,
            Description = createClubModel.Description,
            Image = path,
            Address = createClubModel.Address,
            Category = createClubModel.Category,
            AdminId = User.GetUserId()
        };
        await _clubRepo.AddClub(club);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Join(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        await _clubRepo.AddUserToClubAsync(User.GetUserId(),id);
        return RedirectToAction("Detail", new { id = id });
    }

    public async Task<IActionResult> Leave(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        await _clubRepo.RemoveUserFromClubAsync(User.GetUserId(), id);
        return RedirectToAction("Detail", new { id = id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!User.IsInRole(UserRoles.Admin))
            return RedirectToAction("Index", "Home");
        Club? club = await _clubRepo.GetClubByIdAsyncRO(id);
        if (club==null)
            return View("Error");
        EditClubViewModel editClubModel = new EditClubViewModel
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
    public async Task<IActionResult> Edit(EditClubViewModel editClubModel)
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid)
            return View(editClubModel);
        Club? club = await _clubRepo.GetClubByIdAsync(editClubModel.Id);
        if (club==null)
            return View("Error");
        if (editClubModel.IsImageChanged)
        {
            if (editClubModel.Image == null)
            {
                ModelState.AddModelError("Image","Please choose a image");
                return View(editClubModel);
            }
            string? path = await ProcessImageAdd(editClubModel.Image);
            if (path == null)
                return View(editClubModel);
            _photoService.DeletePhoto(club.Image);
            club.Image = path;
        }
        club.Title = editClubModel.Title;
        club.Description = editClubModel.Description;
        club.Address.City = editClubModel.Address.City;
        club.Address.Country = editClubModel.Address.Country;
        club.Address.Street = editClubModel.Address.Street;
        club.Category = editClubModel.Category;
        await _clubRepo.Save();
        return RedirectToAction("Detail",new {id=club.Id});
    }
}