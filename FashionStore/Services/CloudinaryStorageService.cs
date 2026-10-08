using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FashionStore.Services
{
    public class CloudinaryStorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Bắt buộc dùng URL bảo mật https
        }

        public async Task<string?> UploadFileAsync(IFormFile? file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folderName // Ảnh sẽ tự chia thư mục gọn gàng trên Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Trả về link ảnh online trực tiếp (vd: https://res.cloudinary.com/...)
            return uploadResult.SecureUrl?.ToString();
        }
        public async Task<bool> DeleteFileAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl) || !imageUrl.Contains("cloudinary.com"))
                return false;

            try
            {
                var uri = new Uri(imageUrl);
                var path = uri.AbsolutePath;
                var segments = path.Split('/');
                int uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex == -1 || uploadIndex + 2 >= segments.Length) return false;

                int startIndex = uploadIndex + 1;
                if (segments[startIndex].StartsWith("v") && long.TryParse(segments[startIndex].Substring(1), out _))
                {
                    startIndex++;
                }

                var publicIdWithExt = string.Join("/", segments.Skip(startIndex));
                var publicId = Path.ChangeExtension(publicIdWithExt, null);

                var deleteParams = new CloudinaryDotNet.Actions.DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }
    }
}