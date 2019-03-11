using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Models; 
using AniCharades.API.RequestParameters;
using Microsoft.AspNetCore.Mvc;

namespace AniCharades.API.Controllers
{
    [Route("api/[controller]")]
    public class CharadesController : Controller
    {
        private static IList<CharadesEntry> SampleData = new List<CharadesEntry>()
        {
            new CharadesEntry()
            {
                Series = new SeriesEntry()
                {
                    Id = 1,
                    Title = "Bleach",
                    AnimePositions = new int[] { 1, 2, 3 },
                    MangaPositions = new int[] { 3, 4, 5 },
                    ImageUrl = "bleachImage.png"
                },
                KnownBy = new int[] { 111, 222 }
            },
            new CharadesEntry()
            {
                Series = new SeriesEntry()
                {
                    Id = 1,
                    Title = "Date A Live",
                    AnimePositions = new int[] { 5, 6, 7 },
                    MangaPositions = new int[] { 8, 9, 10 },
                    ImageUrl = "dalImage.png"
                },
                KnownBy = new int[] { 222, 333 }
            }
        };

        [HttpGet]
        public IActionResult GetUsersCharades([FromQuery] GetUsersCharadesParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            return Ok(SampleData);
        }
    }
}
