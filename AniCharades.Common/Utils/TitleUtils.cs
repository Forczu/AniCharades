using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniCharades.Common.Utils
{
    public static class TitleUtils
    {
        public static readonly string[] RedundantLastWords = { "OVA", "OAV", "Movie", "Special", "PV", "Season",
            "Gaiden", "Prologue", "Animation", "Recollections", "CM", "CMs" };
        public static readonly string[] RedundantFirstWords = { "Shin" };
        public static readonly string[] RedundantConnectives = { "the", "a", "an" };
        public static readonly string[] AnimeTypes = { "OVA", "Movie", "Special", "TV", "ONA" };

        public static ICollection<string> GetRedundantNumerals(int numbers)
        {
            var numerals = new string[numbers];
            for (int i = 1; i <= numbers; ++i)
            {
                string wrapUp;
                switch (i.ToString().Last())
                {
                    case '1':
                        wrapUp = "st";
                        break;
                    case '2':
                        wrapUp = "nd";
                        break;
                    case '3':
                        wrapUp = "rd";
                        break;
                    default:
                        wrapUp = "th";
                        break;
                }
                numerals[i - 1] = $"{i}{wrapUp}";
            }
            return numerals;
        }

        public static ICollection<string> GetRedundantNumbers(int numberCount)
        {
            var numbers = new string[numberCount];
            for (int i = 1; i <= numberCount; ++i)
            {
                numbers[i - 1] = i.ToString();
            }
            return numbers;
        }
    }
}
