﻿using AniCharades.Adapters.Interfaces;
using AniCharades.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.Franchise
{
    public interface IFranchiseCreator
    {
        SeriesEntry Create(ICollection<IEntryInstance> animes, ICollection<IEntryInstance> mangas);
    }
}
