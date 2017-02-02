using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using RadioListener.Radio;
using RadioListener.Radio.Commands;

namespace RadioListener
{
    public class RadioListenerConfig
        {
            public string Stream { get; set; }
            public string Volume { get; set; }
            public string RadioName { get; set; }
            public string IdentificationServer { get; set; }
            public string QueryPath { get; set; }
            public int Duration { get; set; }
        }
    public class Program
    {
        public static RadioListenerConfig Config { get; set; }

        public static void Main (string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Config = builder.Build().GetSection("RadioListenerConfig").Get<RadioListenerConfig>();

            if (!Directory.Exists(Config.Volume))
            {
                Console.WriteLine("Volume not mounted");
                return;
            }

            Prepare();

            var listener = BuildListener();
            listener.Listen();
        }

        public static void Prepare()
        {
            var recordingDir = Path.Combine(Config.Volume, Config.RadioName);
            Directory.CreateDirectory(recordingDir);
        }

        public static Listener BuildListener()
        {
            var recordingDir = Path.Combine(Config.Volume, Config.RadioName);

            var recorder = new Recorder(Config.Stream);
            var converter = new Mp3Converter();
            var merger = new Merger();
            var notifier = new Notifier
            {
                BaseAddress = new Uri(Config.IdentificationServer),
                Path = Config.QueryPath
            };

            return new Listener(recorder, converter, merger, notifier, Config.Duration, recordingDir);
        }
    }
}
