using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace RadioListener.Services.Net
{
    public class Scheduler
    {
        private readonly string _baseAddress;
        private readonly string _path;
        private readonly int _port;

        private MonitoringTask _task;

        public Scheduler(string baseAddress, string path, int port = 5000)
        {
            _baseAddress = baseAddress;
            _path = path;
            _port = port;
            _task = null;
        }

        public MonitoringTask GetTask()
        {
            if (_task != null)
            {
                return _task;
            }

            using (var client = new HttpClient())
            {
                var builder = new UriBuilder(_baseAddress)
                {
                    Port = _port,
                    Path = Path.Combine(_path, Dns.GetHostName())
                };

                Console.WriteLine(builder.Uri);
                var response = client.GetAsync(builder.Uri).Result;
                response.EnsureSuccessStatusCode();

                var stringResponse = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(stringResponse);

                _task = JsonConvert.DeserializeObject<MonitoringTask>(stringResponse);
                return _task;
            }
        }
    }

}