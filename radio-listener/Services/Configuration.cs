namespace RadioListener.Services
{
    public class NotificationConfig
    {
        public string BaseAddress { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
    }

    public class SchedulingConfig
    {
        public string BaseAddress { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
    }

    public class ListenerConfig
    {
        public string Volume { get; set; }
        public int Duration { get; set; }
    }

    public class RadioListenerConfig
    {
        public NotificationConfig NotificationConfig { get; set; }
        public SchedulingConfig SchedulingConfig { get; set; }
        public ListenerConfig ListenerConfig { get; set; }
    }
}