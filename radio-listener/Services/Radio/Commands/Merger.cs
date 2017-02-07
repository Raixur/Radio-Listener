using System.IO;
using System.Threading.Tasks;

namespace RadioListener.Services.Radio.Commands
{
    public class Merger : BashCommand
    {
        private const string FileList = "songs.txt";
        private const string Command = "ffmpeg";
        private const string SubCommand = "concat";
        private const string Codec = "copy";
        private const string SafeOption = "-safe 0";

        private static string BuildCommand(string fileList, string output)
        {
            return $"{Command} -f {SubCommand} -y {SafeOption} -i {fileList} -c {Codec} {output}";
        }

        public async Task MergeAsync(string fileList, string outputFile)
        {
            var command = BuildCommand(fileList, outputFile);
            await StartAsync(command);
        }

        public async Task MergeAllAsync(string path, string outputFile)
        {
            var fileListPath = Path.Combine(path, FileList);
            using (var sw = File.CreateText(fileListPath))
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    if (Path.GetExtension(file) == ".mp3")
                    {
                        sw.WriteLine($"file {file}");
                    }
                }
            }
            await MergeAsync(fileListPath, outputFile);
        }
    }
}