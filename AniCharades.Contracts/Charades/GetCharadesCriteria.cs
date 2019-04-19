using AniCharades.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Contracts.Charades
{
    public class GetCharadesCriteria
    {
        public ICollection<string> Usernames { get; set; }

        public ICollection<EntrySources> Sources { get; set; }

        public bool IncludeKnownAdaptations { get; set; }
    }
}
