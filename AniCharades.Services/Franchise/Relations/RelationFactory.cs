using AniCharades.Services.Franchise.Relations.Common;
using AniCharades.Services.Franchise.Relations.Custom;
using System.Collections.Generic;
using System.Linq;

namespace AniCharades.Services.Franchise.Relations
{
    public class RelationFactory
    {
        private static RelationFactory instance = null;

        private readonly ICollection<IRelationStrategy> strategies;

        public static RelationFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RelationFactory(new List<IRelationStrategy>()
                    {
                        new NoWordMatchesStrategy(),
                        new AnyWordMatchesStrategy(),
                        new EveryWordMatchesStrategy(),
                        new SpinOffRelationStrategy(),
                        new OtherRelationStrategy(),
                        new HasParentStoryStrategy(),
                        new GundamRelationStrategy(),
                        new DevilmanRelationStrategy(),
                        new MameshibaRelationStrategy()
                    });
                }
                return instance;
            }
        }

        private RelationFactory(ICollection<IRelationStrategy> strategies)
        {
            this.strategies = strategies;
        }

        public IRelationStrategy Get(string type)
        {
            var strategy = strategies.FirstOrDefault(s => s.GetType().Name.StartsWith(type));
            if (strategy != null)
                return strategy;
            return new AnyWordMatchesStrategy();
        }
    }
}
