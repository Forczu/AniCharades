using AniCharades.Adapters.Interfaces;

namespace AniCharades.API.Algorithms.SeriesAssembler.Providers
{
    public interface IEntryProvider
    {
        IEntryInstance Get(long id);
    }
}
