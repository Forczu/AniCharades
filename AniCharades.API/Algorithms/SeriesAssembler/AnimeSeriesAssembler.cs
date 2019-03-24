using AniCharades.Adapters.Jikan;
using AniCharades.Data.Enumerations;
using AniCharades.Repositories.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.SeriesAssembler
{
    public class AnimeSeriesAssembler : AbstractSeriesAssembler<JikanAnimeAdapter>
    {
        private static readonly int RetryMaxNumber = 3;
        private static readonly int RetryWaitingTime = 60 * 1000;

        private readonly IJikan jikan;

        public AnimeSeriesAssembler(IJikan jikan, IRelationCriteriaRepository criteriaRepo) : base(criteriaRepo)
        {
            this.jikan = jikan;
        }

        protected override JikanAnimeAdapter GetEntry(long entryId)
        {
            int retries = 0;
            Anime anime = null;
            do
            {
                anime = jikan.GetAnime(entryId).Result;
                if (anime == null)
                {
                    Task.Delay(RetryWaitingTime);
                    retries++;
                }
            } while (anime == null && retries < RetryMaxNumber);
            return new JikanAnimeAdapter(anime);
        }
    }
}
