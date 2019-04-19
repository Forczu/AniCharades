using AniCharades.API.Mapper.Profiles;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Mapper
{
    internal class AutoMapperConfiguration : MapperConfiguration
    {
        public AutoMapperConfiguration() : base(GetProfiles())
        {
        }

        private static Action<IMapperConfigurationExpression> GetProfiles()
        {
            return cfg =>
            {
                cfg.AddProfile(new CharadesProfile());
            };
        }
    }
}
