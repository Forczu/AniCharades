using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Logic.Interfaces;
using JikanDotNet;

namespace AniCharades.API.Logic.Implementation
{
    public class MyAnimeListService : IMyAnimeListService
    {
        private readonly IJikan jikan;

        public MyAnimeListService(IJikan jikan)
        {
            this.jikan = jikan;
        }

        public UserAnimeList GetAnimeList(string username)
        {
            return null;
        }

        public UserMangaList GetMangaList(string username)
        {
            return null;
        }
    }
}
