using System.IO;

using RadioListener.Services.Net;
using RadioListener.Services.Radio;
using RadioListener.Services.Radio.Commands;

namespace RadioListener.Services
{
    public class ServiceFactory
    {
        private readonly RadioListenerConfig _config;

        private Scheduler _scheduler;
        private Notifier _notifier;
        private Recorder _recorder;
        private Mp3Converter _converter;
        private Merger _merger;
        private Listener _listener;

        public ServiceFactory(RadioListenerConfig config)
        {
            _config = config;
        }

        public Scheduler Scheduler
        {
            get
            {
                _scheduler = _scheduler ?? new Scheduler(_config.SchedulingConfig.BaseAddress,
                                 _config.SchedulingConfig.Path,
                                 _config.SchedulingConfig.Port);
                return _scheduler;
            }
        }

        public Notifier Notifier
        {
            get
            {
                _notifier = _notifier ?? new Notifier(_config.NotificationConfig.BaseAddress,
                                 _config.NotificationConfig.Path,
                                 _config.NotificationConfig.Port);
                return _notifier;
            }
        }

        public Recorder Recorder
        {
            get
            {
                _recorder = _recorder ?? new Recorder(Scheduler.GetTask().Stream);
                return _recorder;
            }
        }

        public Mp3Converter Converter
        {
            get
            {
                _converter = _converter ?? new Mp3Converter();
                return _converter;
            }
        }

        public Merger Merger
        {
            get
            {
                _merger = _merger ?? new Merger();
                return _merger;
            }
        }

        public Listener Listener
        {
            get
            {
                _listener = _listener ?? new Listener(Recorder, Converter,
                                                      Merger, Notifier,
                                                      _config.ListenerConfig.Duration,
                                                      GetOutDir());
                return _listener;
            }
        }

        private string GetOutDir()
        {
            return  Path.Combine(_config.ListenerConfig.Volume,
                                 Scheduler.GetTask().RadioName);
        }
    }
}