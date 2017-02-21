using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlashFP;
using FlashFP.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SoundIdentifier.Models;

namespace SoundIdentifier.Services.FlashFP
{
    public class FlashFpService
    {
        private readonly FlashFp _flash;

        public FlashFpService(IOptions<FlashConfig> config, IStorage storage)
        {
            _flash = new FlashFp(config.Value, storage);
        }

        public List<Record> Identify(string fileName)
        {
            var recordList = new List<Record>();
            if (File.Exists(fileName))
            {
                _flash.QueryList(fileName, results =>
                {
                    recordList.AddRange(results.Select(result => new Record
                    {
                        Title = result.Description
                    }));
                });
            }

            return recordList;
        }

        public async Task IdentifyAndReport(string fileName, string uri)
        {
            var recordList = Identify(fileName);
            using (var client = new HttpClient())
            {
                await client.PostAsync(uri,
                    new StringContent(JsonConvert.SerializeObject(
                        new
                        {
                            FileName = fileName,
                            Records = recordList
                        }
                    ))
                );
            }
        }
    }
}