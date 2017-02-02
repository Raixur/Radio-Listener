using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using RadioListener.Radio.Commands;
using RadioListener.Utils;

namespace RadioListener.Radio
{
    public class Listener
    {
        private readonly Recorder _recorder;
        private readonly Mp3Converter _converter;
        private readonly Merger _merger;
        private readonly Notifier _notifier;
        private readonly int _intervalLength;
        private readonly string _recordingDir;

        public Listener(Recorder recorder, Mp3Converter converter,
                        Merger merger, Notifier notifier,
                        int intervalLength, string recordingDir )
        {
            _recorder = recorder;
            _converter = converter;
            _merger = merger;
            _notifier = notifier;

            _intervalLength = intervalLength;
            _recordingDir = recordingDir;
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public void Listen()
        {
            var task = Task.CompletedTask;
            try
            {
                while (true)
                {
                    string intermediateDir;
                    string outputFile;

                    Prepare(out intermediateDir, out outputFile);
                    Record(intermediateDir);

                    task = task.ContinueWith(async previousTask => await MergeAndClean(intermediateDir, outputFile))
                               .ContinueWith(async previousTask => await Notify(outputFile)).Unwrap();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            task?.Wait();
        }

        public void ListenTest()
        {
            Task task = null;
            for (int i = 0; i < 3; i++)
            {
                var outputDir = Path.Combine(_recordingDir, $"test{i}");
                var outputFile = Path.Combine(_recordingDir, $"test{i}.mp3");

                Directory.CreateDirectory(outputDir);
                RecordTest(outputDir);

                if (Directory.Exists(outputDir))
                {
                    task = (task ?? Task.CompletedTask)
                        .ContinueWith(async previousTask => await MergeAndClean(outputDir, outputFile))
                        .Unwrap()
                        .ContinueWith(async previousTask => await Notify(outputFile))
                        .Unwrap();
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            task?.Wait();
        }

        public void Prepare(out string intermediateDir, out string outputFile)
        {
            var recordDate = DateTime.Now.GetDateString();

            intermediateDir = Path.Combine(_recordingDir, recordDate);
            Directory.CreateDirectory(intermediateDir);

            outputFile = Path.Combine(_recordingDir, recordDate + ".mp3");
        }

        public void Record(string outputDir)
        {
            var recordingEnd = DateTime.Today.AddDays(1);

            do
            {
                var fileName = DateTime.Now.GetTimeString();
                var minRecordMinutes = Math.Min(_intervalLength, (int)(recordingEnd - DateTime.Now).TotalMinutes);

                _recorder.Record(minRecordMinutes * 60, outputDir, fileName);
            } while (DateTime.Now < recordingEnd);
        }

        public void RecordTest(string outputDir)
        {
            var recordingEnd = DateTime.Now.AddMinutes(1);

            do
            {
                var fileName = DateTime.Now.GetTimeString();
                var minRecordSeconds = Math.Min(15, (int) (recordingEnd - DateTime.Now).TotalSeconds);

                _recorder.Record(minRecordSeconds, outputDir, fileName);
            } while (DateTime.Now < recordingEnd);
        }

        public async Task MergeAndClean(string outputDir, string outputFile)
        {
            await _converter.ConvertAllAsync(outputDir);
            await _merger.MergeAllAsync(outputDir, outputFile);
            Directory.Delete(outputDir, true);
            Console.WriteLine("Cleaned and merged");
        }

        public async Task Notify(string outputFile)
        {
            await _notifier.Notify(outputFile);
            Console.WriteLine("\nNotified");
        }

    }
}