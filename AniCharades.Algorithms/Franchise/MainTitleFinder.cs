using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AniCharades.Algorithms.Franchise
{
    public static class MainTitleFinder
    {
        private static readonly string OtherThanSemiColon = "[^:]";
        private static readonly string NonAscii = @"[^\u0000-\u007F]+";
        private static readonly string AlphaNumeric = @"([0-9]\.[0-9])?[^:\.]+";
        private static readonly string SubtitlePattern = $@"(?<mainTitle>{OtherThanSemiColon}+):\s(?<subTitle>{AlphaNumeric})$";
        private static readonly Regex SubtitleRegex = new Regex(SubtitlePattern);
        private static readonly string ExtraCharactersPattern = @"(?<mainPart>[^!?]+[!?])(?<extraPart>[!?]+$)";
        private static readonly Regex ExtraCharactersRegex = new Regex(ExtraCharactersPattern);
        private static readonly string CustomCharactersPattern = @"(?<mainPart1>.+)(?<extraPart>x2)(?<mainPart2>.+)$";
        private static readonly Regex CustomCharactersRegex = new Regex(CustomCharactersPattern);
        private static readonly int RedundantNumberalsNumber = 20;

        public static string GetMainTitle(ICollection<string> entries)
        {
            var titles = entries.Select(e => Clean(e)).ToArray();
            if (titles.Count() == 1)
                return titles.First();
            var title = FindMostFrequentPhrase(titles);
            return title;
        }

        public static string GetMainTitle(ICollection<IEntryInstance> entries)
        {
            return GetMainTitle(entries.Select(e => e.Title).ToArray());
        }

        private static string Clean(string title)
        {
            title = GetMainTitle(title);
            title = RemoveCustomCharacters(title);
            title = RemoveExtraCharacters(title);
            title = RemoveRedundantWords(title);
            title = RemoveNonAsciiCharacters(title);
            return title;
        }

        private static string GetMainTitle(string title)
        {
            var match = SubtitleRegex.Match(title);
            if (match.Success)
            {
                title = match.Groups["mainTitle"].Value;
            }
            return title;
        }

        private static string RemoveExtraCharacters(string title)
        {
            var match = ExtraCharactersRegex.Match(title);
            if (match.Success)
            {
                title = match.Groups["mainPart"].Value;
            }
            return title;
        }

        private static string RemoveCustomCharacters(string title)
        {
            var match = CustomCharactersRegex.Match(title);
            if (match.Success)
            {
                title = match.Groups["mainPart1"].Value + match.Groups["mainPart2"].Value;
            }
            return title;
        }

        private static string RemoveRedundantWords(string title)
        {
            var words = title.Split(' ');
            var lastWord = words.Last();
            if (IsWordRedundant(lastWord))
            {
                if (words.Count() > 1)
                {
                    var secondLastWord = words[words.Count() - 2];
                    if (IsConnective(secondLastWord))
                    {
                        words = words.SubArray(0, words.Length - 2);
                    }
                    else if (IsWordNumeral(secondLastWord))
                    {
                        words = words.SubArray(0, words.Length - 2);
                    }
                    else
                    {
                        words = words.SubArray(0, words.Length - 1);
                    }
                }
            }
            else if (IsWordNumeral(lastWord) && words.Count() > 1)
            {
                var secondLastWord = words[words.Count() - 2];
                if (IsWordRedundant(secondLastWord))
                    words = words.SubArray(0, words.Length - 2);
            }
            return string.Join(' ', words);
        }

        private static string RemoveNonAsciiCharacters(string title)
        {
            title = Regex.Replace(title, NonAscii + "$", string.Empty);
            return Regex.Replace(title, NonAscii, " ");
        }

        private static bool IsWordRedundant(string word)
        {
            return TitleUtils.RedundantLastWords.Any(rlw => rlw.EqualsCaseInsensitive(word)) 
                || TitleUtils.AnimeTypes.Any(at => at.EqualsWithBrackets(word));
        }

        private static bool IsConnective(string word)
        {
            return TitleUtils.RedundantConnectives.Any(rc => rc.EqualsCaseInsensitive(word));
        }

        private static bool IsWordNumeral(string word)
        {
            var numerals = TitleUtils.GetRedundantNumerals(RedundantNumberalsNumber);
            var isNumeral = numerals.Any(n => n.Equals(word));
            if (isNumeral)
                return true;
            var numbers = TitleUtils.GetRedundantNumbers(RedundantNumberalsNumber);
            var isNumber = numbers.Any(n => n.Equals(word));
            return isNumber;
        }

        private static string FindMostFrequentPhrase(ICollection<string> titles)
        {
            var statistics = new Dictionary<string, int>();
            foreach (var title in titles)
            {
                var phrases = GetPhrases(title);
                phrases.ForEach(p => statistics.IncrementValue(p));
            }
            var phrase = GetPhraseWithMaxScore(statistics, titles.Count);
            return phrase;
        }

        private static ICollection<string> GetPhrases(string title)
        {
            var words = title.Split(' ');
            var length = GetPhrasesArrayLength(words.Length);
            var phrases = new string[length];
            for (int i = 0, k = 0; i < words.Length; ++i)
            {
                for (int j = 1; j <= words.Length - i; ++j, ++k)
                {
                    phrases[k] = string.Join(' ', words.SubArray(i, j));
                }
            }
            return phrases;
        }

        private static long GetPhrasesArrayLength(int length)
        {
            long sum = 0;
            for (int i = 1; i <= length; ++i)
            {
                sum += i;
            }
            return sum;
        }

        private static string GetPhraseWithMaxScore(Dictionary<string, int> statistics, int titleCount)
        {
            int maxScore = statistics.Values.Max();
            string mostImportantPhrase = statistics
                .Where(x => x.Value == maxScore)
                .Select(x => x.Key)
                .OrderByDescending(x => x.Length)
                .First();
            while (true)
            {
                var nextLongestPhrases = statistics
                    .Where(s => s.Key.Contains(mostImportantPhrase) && !s.Key.Equals(mostImportantPhrase))
                    .ToArray();
                if (nextLongestPhrases.Count() != 0)
                {
                    var nextMaxScore = nextLongestPhrases.Max(x => x.Value);
                    nextLongestPhrases = nextLongestPhrases
                        .Where(x => x.Value == nextMaxScore)
                        .ToArray();
                    if (IsScoreGreaterThanHalfOfTitles(titleCount, nextMaxScore) ||
                        IsPhrasesLengthGreaterOrEqualScore(maxScore, nextMaxScore, nextLongestPhrases))
                    {
                        mostImportantPhrase = nextLongestPhrases
                            .Select(x => x.Key)
                            .OrderByDescending(x => x.Length)
                            .First();
                        maxScore = nextMaxScore;
                        continue;
                    }
                }
                return mostImportantPhrase;
            }
        }

        private static bool IsScoreGreaterThanHalfOfTitles(int titleCount, int nextMaxScore)
        {
            return nextMaxScore > Math.Floor((float)titleCount / 2);
        }

        private static bool IsPhrasesLengthGreaterOrEqualScore(int maxScore, int nextMaxScore, KeyValuePair<string, int>[] nextLongestPhrases)
        {
            return (nextMaxScore > 1 || nextMaxScore == maxScore - 1) && nextLongestPhrases.Count() >= maxScore;
        }
    }
}
