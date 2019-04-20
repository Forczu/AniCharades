using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Common.Extensions;
using JikanDotNet;

namespace AniCharades.Services.Providers
{
    public class JikanMangaProvider : IEntryProvider
    {
        private static readonly int RetryMaxNumber = 3;
        private static readonly int RetryWaitingTime = 60 * 1000;

        private readonly IJikan jikan;

        public JikanMangaProvider(IJikan jikan)
        {
            this.jikan = jikan;
        }

        public IEntryInstance Get(long id)
        {
            var manga = jikan.GetMangaWithRetries(id, RetryMaxNumber, RetryWaitingTime);
            if (manga != null)
                return new JikanMangaAdapter(manga);
            return null;
        }
    }
}
