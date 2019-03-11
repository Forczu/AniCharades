using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AniCharades.API.RequestParameters
{
    public class GetUsersCharadesParameters
    {
        [BindRequired]
        public int[] UserIds { get; set; }
    }
}
