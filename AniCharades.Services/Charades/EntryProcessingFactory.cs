using AniCharades.Contracts.Enums;
using AniCharades.Services.Charades.EntryProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniCharades.Services.Charades
{
    public class EntryProcessingFactory
    {
        private static EntryProcessingFactory instance = null;

        private readonly IEnumerable<IEntryProcessingStrategy> strategies;

        private EntryProcessingFactory(IEnumerable<IEntryProcessingStrategy> strategies)
        {
            this.strategies = strategies;
        }

        public static EntryProcessingFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EntryProcessingFactory(new List<IEntryProcessingStrategy>()
                    {
                        new AnimeProcessingStrategy(),
                        new MangaProcessingStrategy()
                    });
                }
                return instance;
            }
        }

        public IEntryProcessingStrategy Get(EntrySource source)
        {
            return strategies.First(s => s.GetType().Name.StartsWith(source.ToString()));
        }
    }
}
