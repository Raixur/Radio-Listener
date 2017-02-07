namespace RadioScheduler.Models
{
    public class MonitoringTask
    {
        public string Stream { get; set; }
        public string RadioName { get; set; }

        public static MonitoringTask DefaultTask => new MonitoringTask
        {
            Stream = "default",
            RadioName = "default"
        };

        public override bool Equals(object obj)
        {
            var other = obj as MonitoringTask;
            return other != null && Equals(other);
        }

        protected bool Equals(MonitoringTask other)
        {
            return string.Equals(Stream, other.Stream) && string.Equals(RadioName, other.RadioName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Stream?.GetHashCode() ?? 0) * 397) ^ (RadioName?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(MonitoringTask left, MonitoringTask right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MonitoringTask left, MonitoringTask right)
        {
            return !Equals(left, right);
        }
    }
}