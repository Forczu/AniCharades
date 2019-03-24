using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Enumerations;

namespace AniCharades.API.Algorithms.SeriesAssembler.DataStructures
{
    public class EntryInstanceRelated<T> where T : IEntryInstance
    {
        public T Entry { get; set; }

        public RelationType RelationType { get; set; }

        public EntryInstanceRelated(T entry, RelationType relationType)
        {
            Entry = entry;
            RelationType = relationType;
        }
    }
}
