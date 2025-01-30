using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using RunningClub.Misc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RunningClub.Services;
public class PhotoService
{
    public enum UploadResultCode
    {
        Success,
        WrongExt,
        Unknown
    }

    public static class ImageType
    {
        public const string Race = "race";
        public const string Club = "club";
        public const string Profile = "profile";
    }
    public struct UploadResult
    {
        public UploadResultCode Code;
        public string Path;
    }
    // private readonly Cloudinary _cloudinary;
    // public PhotoService(IOptions<CloudinarySettings> config)
    // {
    //     Account acc = new Account(
    //         config.Value.CloudName,
    //         config.Value.ApiKey,
    //         config.Value.ApiSecret
    //     );
    //     _cloudinary = new Cloudinary(acc);
    // }
    // public async Task<ImageUploadResult> AddPhotoToCloudinaryAsync(IFormFile file)
    // {
    //     if (file.Length == 0)
    //         return new ImageUploadResult();
    //     await using var stream = file.OpenReadStream();
    //     ImageUploadParams uploadParams = new ImageUploadParams
    //     {
    //         File = new FileDescription(file.FileName, stream),
    //         Transformation = new Transformation().Height(600).Height(600).Crop("fill").Gravity("face")
    //     };
    //     return await _cloudinary.UploadAsync(uploadParams);
    // }
    //
    // public async Task<DeletionResult> DeletePhotoFromCloudinaryAsync(string photoId)
    // {
    //     DeletionParams deletionParams = new DeletionParams(photoId);
    //     return await _cloudinary.DestroyAsync(deletionParams);
    // }

    public async Task<UploadResult> AddPhotoAsync(IFormFile file,string imageFolder)
    {
        List<string> allowedExt=new List<string> { ".jpg", ".png", ".gif", ".bmp" };
        string ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExt.Contains(ext))
            return new UploadResult
            {
                Code = UploadResultCode.WrongExt
            };
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads",imageFolder);
        string fileName = Guid.NewGuid() + ext;
        string filePath = Path.Combine(uploadsFolder, fileName);
        
        using (Image image = await Image.LoadAsync(file.OpenReadStream()))
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(800, 0),
                Mode = ResizeMode.Max
            }));
            await image.SaveAsync(filePath);
        }
        return new UploadResult
        {
            Code = UploadResultCode.Success,
            Path=Path.Combine("/uploads",imageFolder,fileName)
        };
    }

    public bool DeletePhoto(string photoPath)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoPath);
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"File {fullPath} does not exist");
            return false;
        }
        File.Delete(fullPath);
        return true;
    }
}