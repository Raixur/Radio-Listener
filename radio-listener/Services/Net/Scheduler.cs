using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;

namespace RadioListener.Services.Net
{
    public class Scheduler
    {
        private const int MaxAttempts = 5;
        private readonly Uri _uri;
        private MonitoringTask _task;

        public Scheduler(string baseAddress, string path, int port = 5000)
        {
            var builder = new UriBuilder(baseAddress)
            {
                Port = port,
                Path = Path.Combine(path, Dns.GetHostName())
            };

            _uri = builder.Uri;
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
                var attempts = 0;
                Console.WriteLine(_uri);
                while(true)
                {
                    try
                    {
                        var response = client.GetAsync(_uri).Result;

                        var stringResponse = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(stringResponse);

                        _task = JsonConvert.DeserializeObject<MonitoringTask>(stringResponse);
                        return _task;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Thread.Sleep(2000);
                        attempts++;

                        if (attempts >= MaxAttempts)
                        {
                            throw;
                        }
                    }
                }
            }
        }
    }

}