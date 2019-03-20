﻿using AniCharades.API.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Adapters.Jikan
{
    public class JikanRelatedAnimeAdapter : IRelatedInstance
    {
        private readonly RelatedAnime relatedAnime;

        public ICollection<MALSubItem> AlternativeVersions => relatedAnime.AlternativeVersions;

        public ICollection<MALSubItem> Adaptations => relatedAnime.Adaptations;

        public ICollection<MALSubItem> Characters => relatedAnime.Characters;

        public ICollection<MALSubItem> Prequels => relatedAnime.Prequels;

        public ICollection<MALSubItem> Others => relatedAnime.Others;

        public ICollection<MALSubItem> Sequels => relatedAnime.Sequels;

        public ICollection<MALSubItem> SideStories => relatedAnime.SideStories;

        public ICollection<MALSubItem> SpinOffs => relatedAnime.SpinOffs;

        public ICollection<MALSubItem> Summaries => relatedAnime.Summaries;

        public ICollection<MALSubItem> AllRelatedPositions { get; private set; }

        public JikanRelatedAnimeAdapter(RelatedAnime relatedAnime)
        {
            this.relatedAnime = relatedAnime;
            CreateAllRelatedPositionsCollection();
        }

        private void CreateAllRelatedPositionsCollection()
        {
            var allRelatedCollections = new ICollection<MALSubItem>[]
            {
                AlternativeVersions, Characters, Prequels, Others,
                Sequels, SideStories, SpinOffs, Summaries
            };
            AllRelatedPositions = Common.Utils.CollectionUtils.MergeCollectionsWithoutNullValues(allRelatedCollections);
        }
    }
}