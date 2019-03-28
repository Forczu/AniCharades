using AniCharades.Adapters.Jikan;
using AniCharades.Common.Extensions;
using AniCharades.Repositories.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.SeriesAssembler
{
    public class MangaSeriesAssembler : AbstractSeriesAssembler<JikanMangaAdapter>
    {
        private static readonly int RetryMaxNumber = 3;
        private static readonly int RetryWaitingTime = 60 * 1000;

        private readonly IJikan jikan;

        public MangaSeriesAssembler(IJikan jikan, IRelationCriteriaRepository criteriaRepo) : base(criteriaRepo)
        {
            this.jikan = jikan;
        }

        protected override JikanMangaAdapter GetEntry(long entryId)
        {
            var manga = jikan.GetMangaWithRetries(entryId, RetryMaxNumber, RetryWaitingTime);
            return new JikanMangaAdapter(manga);
        }
    }
}
