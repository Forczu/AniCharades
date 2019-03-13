using AniCharades.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Logic
{
    public interface ICharadesCompositionService
    {
        IEnumerable<CharadesEntry> GetCompositedCharades(IEnumerable<int> usersMalIds);
    }
}
