using AniCharades.API.Algorithms.SeriesAssembler.Relations;
using AniCharades.Data.Enumerations;
using AniCharades.Repositories.Interfaces;

namespace AniCharades.API.Algorithms.SeriesAssembler
{
    public class RelationFactory
    {
        private static readonly RelationFactory instance = null;

        public static RelationFactory Instance => instance ?? new RelationFactory();

        private RelationFactory() { }

        public IRelationStrategy Create(string type)
        {
            return null;
        }
    }
}
