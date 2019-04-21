using AniCharades.Contracts.Enums;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Providers;
using JikanDotNet;
using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Services.Franchise.Providers
{
    public class EntryProviderFactory : IEntryProviderFactory
    {
        private readonly IEnumerable<IEntryProvider> providers;
        private readonly IIgnoredEntriesRepository ignored;

        public EntryProviderFactory(IJikan jikan, IIgnoredEntriesRepository ignored)
        {
            providers = new List<IEntryProvider>()
            {
                new JikanAnimeProvider(jikan, ignored),
                new JikanMangaProvider(jikan, ignored)
            };
            this.ignored = ignored;
        }

        public IEntryProvider Get(EntrySource source)
        {
            var provider = providers.FirstOrDefault(s => s.GetType().Name.Contains(source.ToString()));
            return provider;
        }
    }
}
