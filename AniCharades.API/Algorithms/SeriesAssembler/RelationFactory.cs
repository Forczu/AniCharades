using AniCharades.API.Algorithms.SeriesAssembler.Relations;
using AniCharades.API.Algorithms.SeriesAssembler.Relations.Custom;
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
            switch(type)
            {
                case "nyaruko":
                    return new NyarukoRelationStrategy();
                default:
                    return new AnyWordMatchesStrategy();
            }
        }
    }
}
