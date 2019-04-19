using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.RequestParameters;
using AniCharades.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AniCharades.API.Controllers
{
    [Route("api/[controller]")]
    public class CharadesController : Controller
    {
        private readonly ICharadesCompositionService charadesCompositionService;

        public CharadesController(ICharadesCompositionService charadesCompositionService)
        {
            this.charadesCompositionService = charadesCompositionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersCharades([FromQuery] GetCharadesRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var charades = await charadesCompositionService.GetCharades(new Contracts.Charades.GetCharadesCriteria() { Usernames = model.Usernames });
            return Ok(charades);
        }
    }
}
