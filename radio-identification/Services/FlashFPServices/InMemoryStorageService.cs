using FlashFP.Storage;
using Microsoft.Extensions.Options;

namespace SoundIdentifier.Services.FlashFP
{
    public class InMemoryStorageService : InMemoryStorage, IStorage
    {
        public InMemoryStorageService(IOptions<StorageOptions> option) : base(option.Value.FileName)
        {

        }
    }
}