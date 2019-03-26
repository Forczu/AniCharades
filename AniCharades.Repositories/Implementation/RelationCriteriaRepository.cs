using AniCharades.Data.Context;
using AniCharades.Data.Enumerations;
using AniCharades.Data.Models;
using AniCharades.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniCharades.Repositories.Implementation
{
    public class RelationCriteriaRepository : IRelationCriteriaRepository
    {
        private readonly DataContext dataContext;

        public RelationCriteriaRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<RelationCriteria> Get(string title)
        {
            var relationStrategy = await dataContext.RelationCriterias.FirstOrDefaultAsync(s => s.KeywordsMatch == KeywordMatch.Every && s.Keywords.All(k => title.Contains(k)));
            if (relationStrategy != null)
                return relationStrategy;
            relationStrategy = await dataContext.RelationCriterias.FirstOrDefaultAsync(s => s.KeywordsMatch == KeywordMatch.Any && s.Keywords.Any(k => title.Contains(k)));
            return relationStrategy;
        }

        public async Task<RelationCriteria> Get(RelationType relationType)
        {
            var relationStrategy = await dataContext.RelationCriterias.FirstOrDefaultAsync(s => s.Relations.Contains(relationType));
            return relationStrategy;
        }

        public async Task<RelationCriteria> Get(string title, RelationType relationType)
        {
            var relationStrategy = await Get(relationType);
            if (relationStrategy != null)
                return relationStrategy;
            relationStrategy = await Get(title);
            if (relationStrategy != null)
                return relationStrategy;
            return await dataContext.RelationCriterias.FirstOrDefaultAsync(s => s.Relations.Contains(RelationType.None));
        }
    }
}
