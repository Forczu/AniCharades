using AniCharades.Contracts.Enums;
using AniCharades.Services.Providers;
using JikanDotNet;
using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Services.Franchise.Providers
{
    public class EntryProviderFactory : IEntryProviderFactory
    {
        private readonly IEnumerable<IEntryProvider> providers;

        public EntryProviderFactory(IJikan jikan)
        {
            providers = new List<IEntryProvider>()
            {
                new JikanAnimeProvider(jikan),
                new JikanMangaProvider(jikan)
            };
        }

        public IEntryProvider Get(EntrySource source)
        {
            var provider = providers.FirstOrDefault(s => s.GetType().Name.Contains(source.ToString()));
            return provider;
        }
    }
}
