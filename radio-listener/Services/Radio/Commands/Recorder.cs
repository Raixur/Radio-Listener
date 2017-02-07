namespace RadioListener.Services.Radio.Commands
{
    public class Recorder : BashCommand
    {
        private const string Command = "streamripper";
        private const string WriteOver = "always";

        private readonly string _stream;

        public Recorder(string stream)
        {
            _stream = stream;
        }

        private string BuildCommand(int duration, string dir, string fileName)
        {
            var durationOption     = $"-l {duration}";
            var directoryOption    = $"-d {dir}";
            var noIntermidiateFile =  "-A";
            var fileNameOption     = $"-a {fileName}";
            var writeOverOption    = $"-o {WriteOver}";

            return $"{Command} {_stream} {durationOption} " +
                   $"{directoryOption} {noIntermidiateFile} " +
                   $"{fileNameOption} {writeOverOption}";
        }

        public void Record(int duration, string fileName, string subDir)
        {
            var command = BuildCommand(duration, fileName, subDir);
            Start(command);
        }
    }
}