using AniCharades.API.Adapters.Interfaces;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Adapters.Jikan
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

        public ICollection<MALSubItem> AllRelatedPositions { get; private set; }

        public JikanRelatedMangaAdapter(RelatedManga relatedManga)
        {
            this.relatedManga = relatedManga ?? new RelatedManga();
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
