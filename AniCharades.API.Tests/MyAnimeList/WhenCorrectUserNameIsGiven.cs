using AniCharades.API.Logic.Implementation;
using AniCharades.API.Logic.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AniCharades.API.Tests.MyAnimeList
{
    public class WhenCorrectUserNameIsGiven
    {
        IMyAnimeListService myAnimeListService;

        public WhenCorrectUserNameIsGiven()
        {
            myAnimeListService = new MyAnimeListService(new Jikan(true));
        }

        [Fact]
        public void ItShouldGetEntrieAnimeList()
        {
            // given
            var username = "Ervelan";
            // when
            var animeList = myAnimeListService.GetAnimeList(username).Result;
            // then
            Assert.Contains(animeList, a => a.Title == "Yuuki Yuuna wa Yuusha de Aru");
        }
    }
}
