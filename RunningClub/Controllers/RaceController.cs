﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunningClub.Models;
using RunningClub.Repository;
using RunningClub.Services;
using RunningClub.ViewModels;

namespace RunningClub.Controllers;

public class RaceController: Controller
{
    private readonly RaceRepository _raceRepo;
    private readonly PhotoService _photoService;
    public RaceController(RaceRepository raceRepository, PhotoService photoService)
    {
        _photoService = photoService;
        _raceRepo = raceRepository;
    }

    public async Task<IActionResult> Detail(int id)
    {
        Race? race = await _raceRepo.GetRaceByIdAsync(id, true);
        return View(race);
    }
    public async Task<IActionResult> Index()
    {
        List<Race> raceList = await _raceRepo.GetRacesAsync();
        return View(raceList);
    }
    public IActionResult Create()
    {
        return View();
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
    public async Task<IActionResult> CreateRace(CreateRaceViewModel createRaceModel)
    {
        if (!ModelState.IsValid) 
            return View("Create",createRaceModel);
        string? path = await ProcessImageAdd(createRaceModel.Image);
        if (path==null)
            return View("Create",createRaceModel);
        
        Race race = new Race
        {
            Title = createRaceModel.Title,
            Description = createRaceModel.Description,
            Image = path,
            Address = createRaceModel.Address,
            Category = createRaceModel.Category
        };
        _raceRepo.AddRace(race);
        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        Race? club = await _raceRepo.GetRaceByIdAsync(id, true);
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
    public async Task<IActionResult> EditRace(EditRaceViewModel editRaceModel)
    {
        if (!ModelState.IsValid)
            return View("Edit",editRaceModel);
        Race? race = await _raceRepo.GetRaceByIdAsyncNoTracking(editRaceModel.Id, true);
        if (race==null)
            return View("Error");
        if (editRaceModel.IsImageChanged)
        {
            if (editRaceModel.Image == null)
            {
                ModelState.AddModelError("Image","Please choose a image");
                return View("Edit", editRaceModel);
            }
            string? path = await ProcessImageAdd(editRaceModel.Image);
            if (path == null)
                return View("Edit", editRaceModel);
            _photoService.DeletePhoto(race.Image);
            race.Image = path;
        }
        race.Title = editRaceModel.Title;
        race.Description = editRaceModel.Description;
        race.Address = editRaceModel.Address;
        race.Category = editRaceModel.Category;
        race.AddressId = editRaceModel.Address.Id;
        _raceRepo.UpdateRace(race);
        return RedirectToAction("Detail",new {id=race.Id});
    }
}