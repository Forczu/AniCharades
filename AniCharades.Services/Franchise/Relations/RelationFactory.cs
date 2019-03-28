using AniCharades.Services.Franchise.Relations.Custom;

namespace AniCharades.Services.Franchise.Relations
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
