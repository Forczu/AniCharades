using AniCharades.Contracts.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AniCharades.Data.Models
{
    public class IgnoredEntry
    {
        public EntrySource Source { get; set; }
        
        public long Id { get; set; }
    }
}
