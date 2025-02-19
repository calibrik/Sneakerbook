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
        Unknown,
        TooLong
    }

    public static class ImageType
    {
        #if DEBUG
        public const string Race = "racesLocalPics";
        public const string Club = "clubsLocalPics";
        public const string Profile = "profilesLocalPics";
        #else
        public const string Race = "racesAzurePics";
        public const string Club = "clubsAzurePics";
        public const string Profile = "profilesAzurePics";
        #endif
    }
    public struct UploadResult
    {
        public UploadResultCode Code;
        public string Path;
        public string PublicId;
    }
    private readonly Cloudinary _cloudinary;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public PhotoService(IOptions<CloudinarySettings> config,IWebHostEnvironment webHostEnvironment)
    {
        Account acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
        _webHostEnvironment = webHostEnvironment;
    }
    public async Task<UploadResult> AddPhotoToCloudinaryAsync(IFormFile file,string folder)
    {
        if (file.Length >10000000)
            return new UploadResult { Code = UploadResultCode.TooLong };
        List<string> allowedExt=new List<string> { ".jpg", ".png", ".gif", ".bmp" };
        string ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExt.Contains(ext))
            return new UploadResult
            {
                Code = UploadResultCode.WrongExt
            };
        string fileName = Guid.NewGuid()+ext;
        await using Stream stream = file.OpenReadStream();
        ImageUploadParams uploadParams = new ImageUploadParams
        {
            Folder = folder,
            File = new FileDescription(fileName, stream),
            Transformation = new Transformation().Height(470).Width(800).Crop("scale")
        };
        ImageUploadResult res= await _cloudinary.UploadAsync(uploadParams);
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
            return new UploadResult()
            {
                Code = UploadResultCode.Unknown,
            };
        return new UploadResult()
        {
            Code = UploadResultCode.Success,
            Path = res.Url.ToString(),
            PublicId = res.PublicId
        };
    }
    
    public async Task<bool> DeletePhotoFromCloudinaryAsync(string photoId)
    {
        if (photoId == "defaultImage")
            return true;
        DeletionParams deletionParams = new DeletionParams(photoId);
        DeletionResult res= await _cloudinary.DestroyAsync(deletionParams);
        return res.StatusCode == System.Net.HttpStatusCode.OK;
    }
    public async Task<UploadResult> AddPhotoToLocalAsync(IFormFile file,string imageFolder)
    {
        List<string> allowedExt=new List<string> { ".jpg", ".png", ".gif", ".bmp" };
        string ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExt.Contains(ext))
            return new UploadResult
            {
                Code = UploadResultCode.WrongExt
            };
        string uploadsFolder = Path.Combine("wwwroot", "uploads",imageFolder);
        string fileName = Guid.NewGuid().ToString();
        string filePath = Path.Combine(uploadsFolder, fileName+$".{ext}");
        
        using (Image image = await Image.LoadAsync(file.OpenReadStream()))
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(800, 600),
                Mode = ResizeMode.Stretch
            }));
            await image.SaveAsync(filePath);
        }
        return new UploadResult
        {
            Code = UploadResultCode.Success,
            Path=Path.Combine("/uploads",imageFolder,fileName),
            PublicId = fileName
        };
    }

    public bool DeletePhotoFromLocal(string photoPath)
    {
        photoPath = photoPath.Remove(0,1);
        string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, photoPath);
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"File {fullPath} does not exist");
            return false;
        }

        if (fullPath.Split('/').Last() == "defaultImage.jpg")
            return true;
        File.Delete(fullPath);
        return true;
    }
}