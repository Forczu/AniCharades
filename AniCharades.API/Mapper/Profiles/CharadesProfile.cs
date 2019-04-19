using AniCharades.API.Models;
using AniCharades.Contracts.Charades;
using AniCharades.Contracts.Enums;
using AutoMapper;
using System.Collections.Generic;

namespace AniCharades.API.Mapper.Profiles
{
    public class CharadesProfile : Profile
    {
        public CharadesProfile()
        {
            CreateMap<GetCharadesRequestModel, GetCharadesCriteria>()
                .ForMember(dest => dest.Usernames, opt => opt.MapFrom(src => src.Usernames))
                .ForMember(dest => dest.Sources, opt => opt.MapFrom(src => BoolsToEntrySourcesConverter(src)))
                .ForMember(dest => dest.IncludeKnownAdaptations, opt => opt.MapFrom(src => src.IncludeKnownAdaptations));
        }

        private static ICollection<EntrySource> BoolsToEntrySourcesConverter(GetCharadesRequestModel model)
        {
            var sources = new List<EntrySource>();
            bool includeAnimes = model.IncludeAnimeLists;
            if (includeAnimes)
            {
                sources.Add(EntrySource.Anime);
            }
            bool includeMangas = model.IncludeMangaLists;
            if (includeMangas)
            {
                sources.Add(EntrySource.Manga);
            }
            return sources;
        }
    }
}
