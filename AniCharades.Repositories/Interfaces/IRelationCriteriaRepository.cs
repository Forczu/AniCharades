using AniCharades.Data.Enumerations;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AniCharades.Repositories.Interfaces
{
    public interface IRelationCriteriaRepository
    {
        Task<RelationCriteria> Get(string title, RelationType relationType);
    }
}
