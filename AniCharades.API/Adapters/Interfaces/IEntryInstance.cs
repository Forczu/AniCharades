﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Adapters.Interfaces
{
    public interface IEntryInstance
    {
        long Id { get; }

        string Title { get; }

        string Translation { get; }

        ICollection<string> Synonyms { get; }

        string ImageUrl { get; }
    }
}