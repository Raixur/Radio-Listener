using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RadioListener.Radio
{
    public class Notifier
    {
        //private const string RelativeAddress = "/audio/identify";

        public Uri BaseAddress { get; set; } = new Uri("https://postman-echo.com");
        public string Path { get; set; } = "/get";

        public async Task<bool> Notify(string fileName)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var builder = new UriBuilder(BaseAddress)
                    {
                        Path = Path,
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