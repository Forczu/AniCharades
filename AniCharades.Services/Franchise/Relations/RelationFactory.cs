using AniCharades.Services.Franchise.Relations.Common;
using AniCharades.Services.Franchise.Relations.Custom;

namespace AniCharades.Services.Franchise.Relations
{
    public class RelationFactory
    {
        private static RelationFactory instance = null;

        public static RelationFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new RelationFactory();
                return instance;
            }
        }

        private RelationFactory() { }

        public IRelationStrategy Create(string type)
        {
            switch(type)
            {
                case "nyaruko":
                    return new NyarukoRelationStrategy();
                case "kamiNomi":
                    return new KamiNomiRelationStrategy();
                case "noword":
                    return new NoWordMatchesStrategy();
                default:
                    return new AnyWordMatchesStrategy();
            }
        }
    }
}
