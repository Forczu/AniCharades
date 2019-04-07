using AniCharades.Services.Franchise.Relations.Common;

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
                case "noword":
                    return new NoWordMatchesStrategy();
                case "anyword": 
                default:
                    return new AnyWordMatchesStrategy();
                case "everyword":
                    return new EveryWordMatchesStrategy();
                case "spinoff":
                    return new SpinOffRelationStrategy();
                case "other":
                    return new OtherRelationStrategy();
            }
        }
    }
}
