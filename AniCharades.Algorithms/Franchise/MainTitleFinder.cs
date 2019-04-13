using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Common.Titles;
using AniCharades.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AniCharades.Algorithms.Franchise
{
    public class MainTitleFinder
    {
        private static readonly int RedundantNumberalsNumber = 20;
        private static readonly ICollection<string> RedundantNumerals = TitleUtils.GetRedundantNumerals(RedundantNumberalsNumber);
        private static readonly ICollection<string> RedundantNumbers = TitleUtils.GetRedundantNumbers(RedundantNumberalsNumber);
        private static readonly ICollection<string> RedundantRomanNumbers = TitleUtils.GetRedundantRomanNumbers(RedundantNumberalsNumber);

        private static readonly Regex SplitIntoPhrasesRegex = new Regex(@"(\s\(.+\))|(\s)");

        private ICollection<string> originalTitles = null;

        public string GetMainTitle(ICollection<string> entries)
        {
            originalTitles = entries;
            var titles = entries.Select(e => Clean(e)).ToArray();
            if (titles.Count() == 1)
                return titles.First();
            var title = FindMostFrequentPhrase(titles);
            return title;
        }

        public string GetMainTitle(ICollection<IEntryInstance> entries)
        {
            return GetMainTitle(entries.Select(e => e.Title).ToArray());
        }

        public string GetMainTitle(ICollection<string> entries, string mainTitle)
        {
            originalTitles = entries;
            var title = GetMainTitle(entries);
            mainTitle = Clean(mainTitle);
            if (!mainTitle.Equals(title) && mainTitle.Contains(title) &&
                mainTitle.Split(' ').Count() == title.Split(' ').Count() + 1)
            {
                return mainTitle;
            }
            return title;
        }

        public string GetMainTitle(ICollection<IEntryInstance> entries, IEntryInstance mainTitle)
        {
            return GetMainTitle(entries.Select(e => e.Title).ToArray(), mainTitle.Title);
        }

        private string Clean(string title)
        {
            title = GetMainTitle(title);
            title = RemoveCustomCharacters(title);
            title = RemoveExtraCharacters(title);
            title = RemoveLastRedundantWords(title);
            title = RemoveFirstRedundantWords(title);
            title = RemoveNonAsciiCharacters(title);
            return title;
        }

        private string GetMainTitle(string title)
        {
            var match = TitleRegularExpressions.SubtitleRegex.Match(title);
            if (match.Success)
            {
                title = match.Groups["mainTitle"].Value;
            }
            return title;
        }

        private string RemoveExtraCharacters(string title)
        {
            var match = TitleRegularExpressions.ExtraCharactersRegex.Match(title);
            if (match.Success)
            {
                title = match.Groups["mainPart"].Value;
            }
            return title;
        }

        private string RemoveCustomCharacters(string title)
        {
            var match = TitleRegularExpressions.CustomCharactersRegex.Match(title);
            if (match.Success)
            {
                title = match.Groups["mainPart1"].Value + match.Groups["mainPart2"].Value;
            }
            return title;
        }

        private string RemoveFirstRedundantWords(string title)
        {
            var words = title.Split(' ');
            if (words.Count() <= 1)
                return title;
            var firstWord = words.First();
            if (TitleUtils.RedundantFirstWords.Any(w => w.EqualsCaseInsensitive(firstWord)))
            {
                var nextWordIndex = title.IndexOf(' ') + 1;
                var titleWithoutPrefix = title.Substring(nextWordIndex, title.Length - nextWordIndex);
                var containsTitleWithoutPrefix = originalTitles
                    .Any(t => t.Contains(titleWithoutPrefix) && !t.Contains(firstWord));
                if (containsTitleWithoutPrefix)
                {
                    words = words.SubArray(1, words.Length - 1);
                }
            }
            return string.Join(' ', words);
        }

        private string RemoveLastRedundantWords(string title)
        {
            var words = title.Split(' ');
            if (words.Count() <= 1)
                return title;
            var lastWord = words.Last();
            if (IsWordRedundant(lastWord))
            {
                var nextLastWord = words[words.Count() - 2];
                int wordsToRemoveNumber = 1;
                while (words.Count() > 1 && (IsConnective(nextLastWord) ||
                     IsWordRedundant(nextLastWord)) || IsNumeral(nextLastWord))
                {
                    wordsToRemoveNumber++;
                    nextLastWord = words[words.Count() - wordsToRemoveNumber - 1];
                }
                words = words.SubArray(0, words.Length - wordsToRemoveNumber);
            }
            else if (IsWordNumeral(lastWord))
            {
                var nextLastWord = words[words.Count() - 2];
                int wordsToRemoveNumber = 1;
                while (words.Count() > 1 && IsWordRedundant(nextLastWord))
                {
                    wordsToRemoveNumber++;
                    nextLastWord = words[words.Count() - wordsToRemoveNumber - 1];
                }
                words = words.SubArray(0, words.Length - wordsToRemoveNumber);
                title = string.Join(' ', words);
                
            }
            else if (IsWordYear(lastWord))
            {
                words = words.SubArray(0, words.Length - 1);
            }
            return string.Join(' ', words);
        }

        private string RemoveNonAsciiCharacters(string title)
        {
            var words = title.Split(' ');
            if (words.Count() == 0)
                return title;
            var lastWord = words.Last();
            if (WordEndsWithNonAlphanumericCharacters(lastWord))
            {
                var match = TitleRegularExpressions.NonAlphanumericAtEndRegex.Match(title);
                var matchMainTitle = match.Groups["mainPart"].Value;
                var containsTitleWithoutSuffix = originalTitles
                    .Any(t => t.Equals(matchMainTitle));
                if (containsTitleWithoutSuffix)
                {
                    return matchMainTitle;
                }
            }
            return string.Join(' ', words);
        }

        private bool IsWordRedundant(string word)
        {
            return TitleUtils.RedundantLastWords.Any(rlw => rlw.EqualsCaseInsensitive(word)) 
                || TitleUtils.AnimeTypes.Any(at => at.EqualsWithBrackets(word));
        }

        private bool IsConnective(string word)
        {
            return TitleUtils.RedundantConnectives.Any(rc => rc.EqualsCaseInsensitive(word));
        }

        private bool IsWordNumeral(string word)
        {
            var isNumeral = RedundantNumerals.Any(n => n.Equals(word));
            if (isNumeral)
                return true;
            var isNumber = RedundantNumbers.Any(n => n.Equals(word));
            if (isNumber)
                return true;
            var isRomanNumber = RedundantRomanNumbers.Any(n => n.Equals(word));
            return isRomanNumber;
        }

        private bool IsNumeral(string word)
        {
            var isNumeral = RedundantNumerals.Any(n => n.Equals(word));
            return isNumeral;
        }

        private bool EntriesContainTitleWithoutSuffix(string title, string suffix)
        {
            var titleWithoutSuffix = title.Substring(0, title.LastIndexOf(' '));
            var containsTitleWithoutSuffix = originalTitles
                .Any(t => t.Contains(titleWithoutSuffix) && !t.Contains(suffix));
            return containsTitleWithoutSuffix;
        }

        private bool WordEndsWithNonAlphanumericCharacters(string lastWord)
        {
            return TitleRegularExpressions.NonAlphanumericAtEndRegex.IsMatch(lastWord);
        }

        private bool IsWordYear(string word)
        {
            return TitleRegularExpressions.YearInBracketsRegex.IsMatch(word);
        }

        private string FindMostFrequentPhrase(ICollection<string> titles)
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

        private ICollection<string> GetPhrases(string title)
        {
            var words = SplitIntoPhrasesRegex.Split(title).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
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

        private long GetPhrasesArrayLength(int length)
        {
            long sum = 0;
            for (int i = 1; i <= length; ++i)
            {
                sum += i;
            }
            return sum;
        }

        private string GetPhraseWithMaxScore(Dictionary<string, int> statistics, int titleCount)
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
                        IsPhrasesLengthGreaterOrEqualScore(maxScore, nextMaxScore, nextLongestPhrases) ||
                        char.IsLower(mostImportantPhrase[0]))
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

        private bool IsScoreGreaterThanHalfOfTitles(int titleCount, int nextMaxScore)
        {
            return nextMaxScore > Math.Floor((float)titleCount / 2);
        }

        private bool IsPhrasesLengthGreaterOrEqualScore(int maxScore, int nextMaxScore, KeyValuePair<string, int>[] nextLongestPhrases)
        {
            return (nextMaxScore > 1 || nextMaxScore == maxScore - 1) && nextLongestPhrases.Count() > maxScore;
        }
    }
}
