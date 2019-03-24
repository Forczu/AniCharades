﻿using AniCharades.Adapters.Interfaces;
using AniCharades.Adapters.Interfaces.Extensions;
using AniCharades.Data.Models;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Adapters.Jikan
{
    public class JikanRelatedMangaAdapter : IRelatedInstance
    {
        private readonly RelatedManga relatedManga;

        public ICollection<MALSubItem> AlternativeVersions => relatedManga.AlternativeVersions;

        public ICollection<MALSubItem> Adaptations => relatedManga.Adaptations;

        public ICollection<MALSubItem> Characters => relatedManga.Characters;

        public ICollection<MALSubItem> Prequels => relatedManga.Prequels;

        public ICollection<MALSubItem> Others => relatedManga.Others;

        public ICollection<MALSubItem> Sequels => relatedManga.Sequels;

        public ICollection<MALSubItem> SideStories => relatedManga.SideStories;

        public ICollection<MALSubItem> SpinOffs => relatedManga.SpinOffs;

        public ICollection<MALSubItem> Summaries => relatedManga.Summaries;

        public ICollection<RelatedItem> AllRelatedPositions { get; set; }

        public JikanRelatedMangaAdapter(RelatedManga relatedManga)
        {
            this.relatedManga = relatedManga ?? new RelatedManga();
            this.CreateAllRelatedPositionsCollection();
        }
    }
}