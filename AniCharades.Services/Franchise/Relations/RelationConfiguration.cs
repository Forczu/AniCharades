using AniCharades.Adapters.Jikan;
using AniCharades.Common.Extensions;
using AniCharades.Data.Enumerations;
using AniCharades.Data.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniCharades.Services.Franchise.Relations
{
    public class RelationConfiguration
    {
        private static RelationConfiguration instance;

        private readonly IConfigurationRoot config;
        private readonly RelationCriteria[] relationCriterias;

        public static RelationConfiguration Instance
        {
            get
            {
                if (instance == null)
                    instance = new RelationConfiguration();
                return instance;
            }
        }

        private RelationConfiguration()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            relationCriterias = config.GetSection("Relations").Get<RelationCriteria[]>();
        }

        public RelationCriteria GetFromTitle(RelationBetweenEntries relation)
        {
            var sourceTitle = relation.SourceEntry.Title;
            var targetTitle = relation.TargetEntry.Title;
            var relationStrategy = relationCriterias.FirstOrDefault(
                s => s.Keywords != null  && s.Keywords.Any(k => sourceTitle.ContainsCaseInsensitive(k)));
            if (relationStrategy != null)
                return relationStrategy;
            relationStrategy = relationCriterias.FirstOrDefault(
                s => s.Keywords != null && s.Keywords.Any(k => targetTitle.ContainsCaseInsensitive(k)));
            return relationStrategy;
        }

        public RelationCriteria GetFromRelation(RelationType relationType)
        {
            var relationStrategy = relationCriterias.FirstOrDefault(s => s.Relations != null && s.Relations.Contains(relationType));
            return relationStrategy;
        }

        public RelationCriteria Get(RelationBetweenEntries relation)
        {
            var relationStrategy = GetFromTitle(relation);
            if (relationStrategy != null)
                return relationStrategy;
            relationStrategy = GetFromType(relation.TargetEntry.Type);
            if (relationStrategy != null)
                return relationStrategy;
            var isTargetParentStory = relation.TargetForSourceType == RelationType.ParentStory;
            var relationType = !isTargetParentStory ? relation.TargetForSourceType : relation.SourceForTargetType;
            relationStrategy = GetFromRelation(relationType);
            if (relationStrategy != null)
                return relationStrategy;
            return relationCriterias.FirstOrDefault(s => s.Relations != null && s.Relations.Contains(RelationType.None));
        }

        private RelationCriteria GetFromType(string type)
        {
            var relationStrategy = relationCriterias.FirstOrDefault(s => s.Types != null && s.Types.Contains(type));
            return relationStrategy;
        }
    }
}
