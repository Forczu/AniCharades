using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AniCharades.API.RequestParameters
{
    public class GetUsersCharadesParameters
    {
        [BindRequired]
        public string[] Usernames { get; set; }
    }
}
