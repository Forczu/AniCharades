using AniCharades.Contracts.Enums;
using AniCharades.Services.Providers;

namespace AniCharades.Services.Franchise.Providers
{
    public interface IEntryProviderFactory
    {
        IEntryProvider Get(EntrySource source);
    }
}
