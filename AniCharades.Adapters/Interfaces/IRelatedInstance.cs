using AniCharades.Data.Models;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Adapters.Interfaces
{
    public interface IRelatedInstance
    {
        ICollection<MALSubItem> AlternativeVersions { get; }

        ICollection<MALSubItem> Adaptations { get; }
        
        ICollection<MALSubItem> Characters { get;  }
        
        ICollection<MALSubItem> Prequels { get; }

        ICollection<MALSubItem> Others { get; }

        ICollection<MALSubItem> Sequels { get; }

        ICollection<MALSubItem> SideStories { get; }

        ICollection<MALSubItem> SpinOffs { get; }

        ICollection<MALSubItem> Summaries { get; }

        ICollection<RelatedItem> AllRelatedPositions { get; set; }
    }
}
