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

        public RelationCriteria Get(string title)
        {
            var relationStrategy = relationCriterias.FirstOrDefault(
                s => s.KeywordsMatch == KeywordMatch.Every && 
                s.Keywords.All(k => title.ContainsCaseInsensitive(k)));
            if (relationStrategy != null)
                return relationStrategy;
            relationStrategy = relationCriterias.FirstOrDefault(
                s => s.KeywordsMatch == KeywordMatch.Any && 
                s.Keywords.Any(k => title.ContainsCaseInsensitive(k)));
            return relationStrategy;
        }

        public RelationCriteria Get(RelationType relationType)
        {
            var relationStrategy = relationCriterias.FirstOrDefault(s => s.Relations != null && s.Relations.Contains(relationType));
            return relationStrategy;
        }

        public RelationCriteria Get(string title, RelationType relationType)
        {
            var relationStrategy = Get(relationType);
            if (relationStrategy != null)
                return relationStrategy;
            relationStrategy = Get(title);
            if (relationStrategy != null)
                return relationStrategy;
            return relationCriterias.FirstOrDefault(s => s.Relations.Contains(RelationType.None));
        }
    }
}
