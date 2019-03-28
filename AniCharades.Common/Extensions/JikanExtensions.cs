using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AniCharades.Common.Extensions
{
    public static class JikanExtensions
    {
        public static Anime GetAnimeWithRetries(this IJikan jikan, long malId, int retryNumber, int delay)
        {
            var anime = GetWithRetries(jikan.GetAnime, malId, retryNumber, delay);
            return anime;
        }

        public static Manga GetMangaWithRetries(this IJikan jikan, long malId, int retryNumber, int delay)
        {
            var manga = GetWithRetries(jikan.GetManga, malId, retryNumber, delay);
            return manga;
        }

        private static TOutput GetWithRetries<TParam, TOutput>(Func<TParam, Task<TOutput>> func, TParam param, int retryNumber, int delay)
        {
            int retries = 0;
            TOutput result = default(TOutput);
            do
            {
                result = func(param).Result;
                if (result == null)
                {
                    Task.Delay(delay);
                    retries++;
                }
            } while (result == null && retries < retryNumber);
            return result;
        }
    }
}
