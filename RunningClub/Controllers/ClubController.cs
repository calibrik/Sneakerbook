using Microsoft.AspNetCore.Mvc;
using RunningClub.Interfaces;
using RunningClub.Models;
using RunningClub.Services;
using RunningClub.ViewModels;

namespace RunningClub.Controllers;

public class ClubController:Controller
{
    private readonly IClubRepository _clubRepo;
    private readonly PhotoService _photoService;
    public async Task<IActionResult> Index()
    {
        List<Club> clubs = await _clubRepo.GetClubsAsyncRO();
        return View(clubs);
    }

    public ClubController(IClubRepository clubRepository,PhotoService photoService)
    {
        _clubRepo = clubRepository;
        _photoService = photoService;
    }

    public IActionResult Create()
    {
        return View();
    }
    public async Task<IActionResult> Detail(int id)
    {
        Club? club = await _clubRepo.GetClubByIdAsyncRO(id, true);
        return View(club);
    }

    async Task<string?> ProcessImageAdd(IFormFile file)
    {
        PhotoService.UploadResult res= await _photoService.AddPhotoAsync(file);
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
            Category = createClubModel.Category
        };
        await _clubRepo.AddClub(club);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        Club? club = await _clubRepo.GetClubByIdAsyncRO(id, true);
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
        if (!ModelState.IsValid)
            return View(editClubModel);
        Club? club = await _clubRepo.GetClubByIdAsync(editClubModel.Id, true);
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
        club.AddressId = editClubModel.Address.Id;
        await _clubRepo.Save();
        return RedirectToAction("Detail",new {id=club.Id});
    }
}