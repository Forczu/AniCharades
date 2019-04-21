using AniCharades.Adapters.Interfaces;

namespace AniCharades.Services.Providers
{
    public interface IEntryProvider
    {
        IEntryInstance Get(long id);

        bool IsIgnored(long id);
    }
}
