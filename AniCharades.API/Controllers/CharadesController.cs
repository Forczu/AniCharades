using System.Threading.Tasks;
using AniCharades.API.Models;
using AniCharades.Contracts.Charades;
using AniCharades.Repositories.Interfaces;
using AniCharades.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AniCharades.API.Controllers
{
    [Route("api/[controller]")]
    public class CharadesController : Controller
    {
        private readonly ICharadesCompositionService charadesCompositionService;
        private readonly IMapper mapper;

        public CharadesController(ICharadesCompositionService charadesCompositionService, IMapper mapper)
        {
            this.charadesCompositionService = charadesCompositionService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersCharades([FromQuery] GetCharadesRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var criteria = mapper.Map<GetCharadesCriteria>(model);
            var charades = await charadesCompositionService.GetCharades(criteria);
            return Ok(charades);
        }
    }
}
