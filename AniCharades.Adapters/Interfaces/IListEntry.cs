using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Adapters.Interfaces
{
    public interface IListEntry
    {
        long Id { get; }

        string Title { get; }

        ICollection<string> Users { get; }

        void AddUser(string username);
    }
}
