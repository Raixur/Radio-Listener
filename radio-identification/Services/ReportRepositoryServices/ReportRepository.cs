using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SoundIdentifier.Models;

namespace SoundIdentifier.Services.ReportService
{
    public class ReportRepository
    {
        private readonly string _reportDir;

        public ReportRepository(IOptions<ReportOptions> options)
        {
            _reportDir = options.Value.ReportDir;
        }

        public void CreateReport(string radio, string id, List<Record> records)
        {
            var json = JsonConvert.SerializeObject(records);
            File.WriteAllText(Path.Combine(_reportDir, radio, id + ".json"), json);
        }

        public string GetReport(string radio, string id)
        {
            var reportFile = Path.Combine(_reportDir, radio, id + ".json");
            return File.Exists(reportFile) ? File.ReadAllText(reportFile) : "";
        }

        public List<string> GetMissingReports(string radio, string audioDir)
        {
            var audioPath = Path.Combine(audioDir, radio);
            var reportPath = Path.Combine(_reportDir, radio);

            var audioList = Directory.GetFiles(audioPath).Select(Path.GetFileNameWithoutExtension);
            var reportList = Directory.GetFiles(reportPath).Select(Path.GetFileNameWithoutExtension);

            return audioList.Except(reportList).ToList();
        }
    }
}