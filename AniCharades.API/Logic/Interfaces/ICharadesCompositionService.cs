using AniCharades.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Logic.Interfaces
{
    public interface ICharadesCompositionService
    {
        ICollection<CharadesEntry> GetCompositedCharades(IEnumerable<string> usernames);
    }
}
