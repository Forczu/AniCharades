using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.API.Logic.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Repositories.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.SeriesAssembler
{
    public class MangaSeriesAssembler : AbstractSeriesAssembler
    {
        private static readonly int RetryMaxNumber = 3;
        private static readonly int RetryWaitingTime = 60 * 1000;

        private readonly IJikan jikan;

        public MangaSeriesAssembler(IJikan jikan, IRelationService relationService) : base(relationService)
        {
            this.jikan = jikan;
        }

        protected override IEntryInstance GetEntry(long entryId)
        {
            var manga = jikan.GetMangaWithRetries(entryId, RetryMaxNumber, RetryWaitingTime);
            return new JikanMangaAdapter(manga);
        }
    }
}
