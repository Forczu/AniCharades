using AniCharades.Adapters.Interfaces;
using AniCharades.Common.Extensions;
using AniCharades.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AniCharades.API.Algorithms.Franchise
{
    public static class MainTitleFinder
    {
        private static readonly string[] RedundantEnglishArticles = { "The", "the", "From" };
        private static readonly string[] RedundantWords = { "Animation", "Movie", "TV", "(TV)", "OVA", "OAV" };
        private static readonly char[] SpecialSymbols = { '.', ',', ':', ':', '!' };
        private static readonly string ValidWordsSeparators = @"\s+";

        private static Regex ValidWordsSeparatorRegex = new Regex(ValidWordsSeparators);

        public static string GetMainTitle(IEntryInstance mainEntry, ICollection<IEntryInstance> otherEntries)
        {
            if (CollectionUtils.IsCollectionNullOrEmpty(otherEntries))
            {
                var mainTitle = RemoveRedundantWords(mainEntry.Title);
                return mainTitle;
            }
            var otherTitles = otherEntries.Where(a => a != mainEntry).Select(a => a.Title).ToArray();
            var coreTitle = GetCoreTitle(mainEntry.Title, otherTitles);
            return coreTitle;
        }

        private static string RemoveRedundantWords(string title)
        {
            string newTitle = title;
            foreach (var word in RedundantWords)
            {
                string redundantWord = ' ' + word;
                foreach (var article in RedundantEnglishArticles)
                {
                    string redundantPhrase = ' ' + article + redundantWord;
                    newTitle = newTitle.Replace(redundantPhrase, string.Empty);
                }
                newTitle = newTitle.Replace(redundantWord, string.Empty);
            }
            return newTitle;
        }

        private static string GetCoreTitle(string mainTitle, ICollection<string> otherFamilyTitles)
        {
            var currentTitle = RemoveRedundantWords(mainTitle);
            var otherTitles = otherFamilyTitles.Select(t => RemoveRedundantWords(t)).ToArray();
            var coreWords = ExtractCoreWords(currentTitle, otherTitles);
            string newTitle = string.Join(' ', coreWords);
            return newTitle;
        }

        private static string[] ExtractCoreWords(string currentMainTitle, ICollection<string> otherFamilyTitles)
        {
            IList<string> mainTitle = new List<string>();
            var initialTitleAsWords = GetTitleAsWords(currentMainTitle).ToArray();
            var otherTitlesAsWords = otherFamilyTitles.Select(t => GetTitleAsWords(t).ToArray());
            
            for (int wordIndex = 0; wordIndex < initialTitleAsWords.Count(); wordIndex++)
            {
                string currentTitleWord = initialTitleAsWords[wordIndex];
                var wordStatistics = new Dictionary<string, int>();
                wordStatistics.IncrementValue(currentTitleWord);
                int additionalSum = 0;
                foreach (string[] titleAsWords in otherTitlesAsWords)
                {
                    if (IsIndexOutOfOtherTitleBounds(wordIndex, titleAsWords.Length))
                    {
                        additionalSum++;
                        continue;
                    }
                    string otherTitleWord = titleAsWords[wordIndex];
                    if (HasWordSpecialCharacter(currentTitleWord) && HasWordSpecialCharacter(otherTitleWord))
                    {
                        wordStatistics.IncrementValue(otherTitleWord);
                        continue;
                    }
                    if (!AreWordsEqual(currentTitleWord, otherTitleWord))
                    {
                        wordStatistics.IncrementValue(otherTitleWord);
                    }
                    else
                    {
                        wordStatistics.IncrementValue(currentTitleWord);
                    }
                }
                AddNewWordToTitle(mainTitle, wordStatistics);
            }
            RemoveLastCharacterIfNecessary(mainTitle);
            return mainTitle.ToArray();
        }

        private static IEnumerable<string> GetTitleAsWords(string title)
        {
            return ValidWordsSeparatorRegex.Split(title).Cast<string>();
        }

        private static bool HasWordSpecialCharacter(string titleWord)
        {
            return titleWord.Any(c => SpecialSymbols.Any(ss => ss.Equals(c)));
        }

        private static bool IsIndexOutOfOtherTitleBounds(int wordIndex, int otherTitleLength)
        {
            return wordIndex + 1 > otherTitleLength;
        }

        private static bool AreWordsEqual(string currentTitleWord, string otherTitleWord)
        {
            return currentTitleWord.RemoveSpecialCharacters().Equals(otherTitleWord.RemoveSpecialCharacters());
        }

        private static void RemoveLastCharacterIfNecessary(IList<string> title)
        {
            var lastCharacter = title.Last().Last();
            if (lastCharacter == ':' || lastCharacter == '-')
            {
                string lastWord = title.Last();
                lastWord = lastWord.Remove(lastWord.Count() - 1);
                title[title.Count() - 1] = lastWord;
            }
        }

        private static void AddNewWordToTitle(IList<string> title, Dictionary<string, int> wordStatistics)
        {
            int maxScore = wordStatistics.Values.Max();
            var valuesWithMax = wordStatistics.Where(x => x.Value == maxScore);
            var word = valuesWithMax.FirstOrDefault().Key;
            title.Add(word);
        }
    }
}
