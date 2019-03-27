using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Adapters.Interfaces
{
    public interface IEntryInstance
    {
        long Id { get; }

        string Title { get; }

        string Translation { get; }

        ICollection<string> Synonyms { get; }

        string ImageUrl { get; }

        IRelatedInstance Related { get; }

        TimePeriod TimePeriod { get; }

        string Type { get; }
    }
}
