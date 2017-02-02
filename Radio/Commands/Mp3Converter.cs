using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RadioListener.Radio.Commands
{
    public class Mp3Converter : BashCommand
    {
        private const string Command = "ffmpeg";
        private const string Codec = "libmp3lame";

        private static string BuildCommand(string inputFile, string outputFile)
        {
            var inputFileOption = $"-i {inputFile}";
            var codecOption = $"-acodec {Codec}";

            return $"{Command} -y {inputFileOption} {codecOption} {outputFile}";
        }

        public async Task ConvertAsync(string inputFile, string outputFile)
        {
            var command = BuildCommand(inputFile, outputFile);
            await StartAsync(command);
        }

        public async Task ConvertAllAsync(string dir)
        {
            var convertTasks = from file in Directory.GetFiles(dir)
                               where IsMp3(file)
                               select ConvertAsync(file, Path.Combine(dir, GetNewFileName(file)));
            await Task.WhenAll(convertTasks);
        }

        private static bool IsMp3(string file) =>
            Path.GetExtension(file) != ".mp3" && Path.GetExtension(file) != ".cue";

        private static string GetNewFileName(string file) =>
            Path.GetFileNameWithoutExtension(file) + ".mp3";
    }
}