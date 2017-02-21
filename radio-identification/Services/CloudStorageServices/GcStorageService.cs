using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;

namespace SoundIdentifier.Services.CloudStorage
{
    public class GcStorageOptions
    {
        public string ProjectId { get; set; }
    }

    public class GcStorageService : ICloudStorageService
    {
        private readonly StorageClient _client;
        
        public GcStorageService()
        {
            _client = StorageClient.Create();
        }

        public bool Exists(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task UploadAsync(string bucket, string source, string destination)
        {
            var contentType = GetContentType(source);
            if (contentType == "")
            {
                throw new ArgumentException("Invalid content type");
            }

            using(var stream = new FileStream(source, FileMode.Open))
            {
                await _client.UploadObjectAsync(bucket, destination, contentType, stream);
            }
        }

        public async Task DownloadAsync(string bucket, string source, string destination)
        {
            using(var stream = new FileStream(destination, FileMode.Create))
            {
                await _client.DownloadObjectAsync(bucket, source, stream);
            }
        }

        private static string GetContentType(string fileName)
        {
            string contentType;
            var extension = Path.GetExtension(fileName);

            return ContentMappings.TryGetValue(extension, out contentType) ? contentType : "";
        }

        private static readonly IDictionary<string, string> ContentMappings = new Dictionary<string, string> {
            #region Big freaking list of mime types
            {".aa",   "audio/audible"},
            {".AAC",  "audio/aac"},
            {".aax",  "audio/vnd.audible.aax"},
            {".ac3",  "audio/ac3"},
            {".ADT",  "audio/vnd.dlna.adts"},
            {".ADTS", "audio/aac"},
            {".aif",  "audio/x-aiff"},
            {".aifc", "audio/aiff"},
            {".aiff", "audio/aiff"},
            {".au",   "audio/basic"},
            {".caf",  "audio/x-caf"},
            {".cdda", "audio/aiff"},
            {".gsm",  "audio/x-gsm"},
            {".m3u",  "audio/x-mpegurl"},
            {".m3u8", "audio/x-mpegurl"},
            {".m4a",  "audio/m4a"},
            {".m4b",  "audio/m4b"},
            {".m4p",  "audio/m4p"},
            {".m4r",  "audio/x-m4r"},
            {".mid",  "audio/mid"},
            {".midi", "audio/mid"},
            {".mp3",  "audio/mpeg"},
            {".pls",  "audio/scpls"},
            {".ra",   "audio/x-pn-realaudio"},
            {".ram",  "audio/x-pn-realaudio"},
            {".rmi",  "audio/mid"},
            {".rpm",  "audio/x-pn-realaudio-plugin"},
            {".sd2",  "audio/x-sd2"},
            {".smd",  "audio/x-smd"},
            {".smx",  "audio/x-smd"},
            {".smz",  "audio/x-smd"},
            {".snd",  "audio/basic"},
            {".wav",  "audio/wav"},
            {".wave", "audio/wav"},
            {".wax",  "audio/x-ms-wax"},
            {".wma",  "audio/x-ms-wma"}
            #endregion
        };

    }
}