using AniCharades.Adapters.Jikan;
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

        public MangaSeriesAssembler(IJikan jikan)
        {
            this.jikan = jikan;
        }

        protected override bool CheckIfRelationIsValidForSeries(JikanMangaAdapter sourceEntry, JikanMangaAdapter relatedEntry)
        {
            throw new NotImplementedException();
        }

        protected override JikanMangaAdapter GetEntry(long entryId)
        {
            int retries = 0;
            Manga manga = null;
            do
            {
                manga = jikan.GetManga(entryId).Result;
                if (manga == null)
                {
                    Task.Delay(RetryWaitingTime);
                    retries++;
                }
            } while (manga == null && retries < RetryMaxNumber);
            return new JikanMangaAdapter(manga);
        }
    }
}
