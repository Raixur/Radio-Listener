using System.Threading.Tasks;

namespace SoundIdentifier.Services.CloudStorage
{
    public interface ICloudStorageService
    {
        bool Exists(string fileName);
        Task UploadAsync(string bucket, string source, string destination);
        Task DownloadAsync(string bucket, string source, string destination);
    }
}