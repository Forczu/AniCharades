﻿using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Jikan;
using AniCharades.Common.Extensions;
using JikanDotNet;

namespace AniCharades.API.Algorithms.SeriesAssembler.Providers
{
    public class JikanAnimeProvider : IEntryProvider
    {
        private static readonly int RetryMaxNumber = 3;
        private static readonly int RetryWaitingTime = 60 * 1000;

        private readonly IJikan jikan;

        public JikanAnimeProvider(IJikan jikan)
        {
            this.jikan = jikan;
        }

        public IEntryInstance Get(long id)
        {
            var anime = jikan.GetAnimeWithRetries(id, RetryMaxNumber, RetryWaitingTime);
            return new JikanAnimeAdapter(anime);
        }
    }
}