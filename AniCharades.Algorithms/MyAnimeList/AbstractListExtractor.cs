using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.Algorithms.MyAnimeList
{
    public abstract class AbstractListExtractor<T>
    {
        public async Task<IList<T>> GetList(string username)
        {
            var entries = new List<T>();
            var pageIndex = 1;
            while (true)
            {
                var nextListPage = await GetUserListPage(username, pageIndex);
                if (!ListPageExists(nextListPage))
                    break;
                entries.AddRange(nextListPage);
                pageIndex++;
            }
            return entries;
        }

        protected abstract Task<ICollection<T>> GetUserListPage(string username, int page);

        private bool ListPageExists(ICollection<T> page)
        {
            return page != null && page.Count > 0;
        }
    }
}
