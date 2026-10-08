using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FashionStore.Services
{
    public interface IStorageService
    {
        Task<string?> UploadFileAsync(IFormFile? file, string folderName);
        Task<bool> DeleteFileAsync(string imageUrl);
    }
}