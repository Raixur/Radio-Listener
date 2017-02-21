using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SoundIdentifier.Services.CloudStorage;


namespace SoundIdentifier.Services.AudioRepositoryServices
{
    public class AudioRepository
    {
        private readonly ICloudStorageService _cloudStorage;
        private readonly string _audioDir;

        public AudioRepository(ICloudStorageService cloudStorage, IOptions<AudioRepositoryOptions> options)
        {
            _cloudStorage = cloudStorage;
            _audioDir = options.Value.AudioDir;
        }

        public string GetFile(string radio, string file)
        {
            var audioFile = Path.Combine(_audioDir, radio, file);
            return File.Exists(audioFile) ? audioFile : "";
        }

        public async Task UploadAsync(string radio, string fileName)
        {
            var localPath = Path.Combine(_audioDir, radio, fileName);
            if (File.Exists(localPath))
            {
                await _cloudStorage.UploadAsync(radio, localPath, fileName);
            }
        }

        public async Task DownloadAsync(string radio, string fileName)
        {
            var localPath = Path.Combine(_audioDir, radio, fileName);
            if (!File.Exists(localPath))
            {
                await _cloudStorage.DownloadAsync(radio, fileName, localPath);
            }
        }

    }
}