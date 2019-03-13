using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniCharades.API.Logic;
using AniCharades.API.Models; 
using AniCharades.API.RequestParameters;
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
        public IActionResult GetUsersCharades([FromQuery] GetUsersCharadesParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var charades = charadesCompositionService.GetCompositedCharades(parameters.UserIds);
            return Ok(charades);
        }
    }
}
