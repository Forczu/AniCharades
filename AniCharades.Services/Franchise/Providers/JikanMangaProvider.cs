using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Common.Extensions;
using AniCharades.Repositories.Interfaces;
using JikanDotNet;

namespace AniCharades.Services.Providers
{
    public class JikanMangaProvider : IEntryProvider
    {
        private static readonly int RetryMaxNumber = 3;
        private static readonly int RetryWaitingTime = 60 * 1000;

        private readonly IJikan jikan;
        private readonly IIgnoredEntriesRepository ignored;

        public JikanMangaProvider(IJikan jikan, IIgnoredEntriesRepository ignored)
        {
            this.jikan = jikan;
            this.ignored = ignored;
        }

        public IEntryInstance Get(long id)
        {
            var manga = jikan.GetMangaWithRetries(id, RetryMaxNumber, RetryWaitingTime);
            if (manga != null)
                return new JikanMangaAdapter(manga);
            return null;
        }

        public bool IsIgnored(long id)
        {
            return ignored.IsIgnored(id, Contracts.Enums.EntrySource.Manga).Result;
        }
    }
}
