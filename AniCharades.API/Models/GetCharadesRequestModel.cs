using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace AniCharades.API.Models
{
    public class GetCharadesRequestModel
    {
        [BindRequired]
        public ICollection<string> Usernames { get; set; }

        public bool IncludeAnimeLists { get; set; }

        public bool IncludeMangaLists { get; set; }

        public bool IncludeKnownAdaptations { get; set; }
    }
}
