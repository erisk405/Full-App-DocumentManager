using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace HRM_API.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        
        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }
        public class CloudinaryUploadResult
        {
            public string PublicId { get; set; }
            public string SecureUrl { get; set; }
        }
        public async Task<CloudinaryUploadResult> UploadFileUniqueAsync(IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();

            var extension = Path.GetExtension(file.FileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N");
            var filename = $"{timestamp}_{guid}{extension}";


            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(filename, stream),
                Folder = folder,
                PublicId = Path.GetFileNameWithoutExtension(filename),
                Overwrite = true,
                AccessMode = "public"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return new CloudinaryUploadResult
            {
                PublicId = uploadResult.PublicId,
                SecureUrl = uploadResult.SecureUrl.ToString()
            };
        }
        public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                Overwrite = true,
                Type = "upload"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return new CloudinaryUploadResult
            {
                PublicId = uploadResult.PublicId,
                SecureUrl = uploadResult.SecureUrl.ToString()
            };
        }
        public async Task DeleteImageAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            await _cloudinary.DestroyAsync(deletionParams);
        }

        public async Task DeleteFileAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw
            };

            await _cloudinary.DestroyAsync(deletionParams);
        }
    }
}
