using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SoundIdentifier.Services.AudioRepositoryServices;
using SoundIdentifier.Services.FlashFP;
using SoundIdentifier.Services.ReportService;

namespace SoundIdentifier.Controllers
{

    public class AudioController : Controller
    {
        private readonly AudioRepository _audioRepository;
        private readonly ReportRepository _reportRepository;
        private readonly FlashFpService _flash;

        public AudioController(AudioRepository audioRepo, ReportRepository reportRepo, FlashFpService flash)
        {
            _audioRepository = audioRepo;
            _reportRepository = reportRepo;
            _flash = flash;
        }

        public IActionResult Help()
        {
            return new JsonResult("Usage ");
        }

        public IActionResult Identify(string radio, string id)
        {
            var fileName = _audioRepository.GetFile(radio, id);
            var result = _flash.Identify(fileName);

            if (!result.Any())
            {
                _reportRepository.CreateReport(radio, id, result);
                return new JsonResult($"Audio {fileName} identified");
            }
            return new JsonResult("No audio identified");
        }

        public async Task<IActionResult> Upload(string radio, string id)
        {
            await _audioRepository.UploadAsync(radio, id);
            return new JsonResult($"Audio {id} stored");
        }


        public async Task<IActionResult> Download(string radio, string id)
        {
            await _audioRepository.DownloadAsync(radio, id);
            return new JsonResult($"Audio {id} pushed");
        }
    }

    
}