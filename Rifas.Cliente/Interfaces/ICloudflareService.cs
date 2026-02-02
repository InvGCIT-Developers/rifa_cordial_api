using Rifas.Client.Models.DTOs;

namespace Rifas.Client.Interfaces
{
    public interface ICloudflareService
    {
        Task<DownLoadImagegResponse> DownloadImageAsync(string imageId);
        Task<List<DownLoadImagegResponse>> DownloadMultipleImagesAsync(List<string> imageIds);
        Task<DownLoadImagegResponse?> UploadImageAsync(byte[] fileBytes, string fileName, long raffleId);
    }
}