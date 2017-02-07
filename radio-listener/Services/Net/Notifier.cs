using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RadioListener.Services.Net
{
    public class Notifier
    {
        private readonly string _baseAddress;
        private readonly string _path;
        private readonly int _port;

        public Notifier(string baseAddress, string path, int port=80)
        {
            _baseAddress = baseAddress;
            _path = path;
            _port = port;
        }


        public async Task<bool> Notify(string fileName)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var builder = new UriBuilder(_baseAddress)
                    {
                        Port = _port,
                        Path = _path,
                        Query = $"fileName='{fileName}'"
                    };
                    var result = await client.GetAsync(builder.Uri);

                    using (var sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
                    {
                        Console.WriteLine(sr.ReadToEnd());
                    }

                    //TODO: Make status checks
                    //var response = await client.GetAsync(RelativeAddress);
                    //response.EnsureSuccessStatusCode();

                    return true;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                    return false;
                }
            }
        }
    }
}