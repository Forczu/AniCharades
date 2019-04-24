using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AniCharades.Common.Utils
{
    public static class RelationUtils
    {
        private static readonly string[] NonImportantWords;
        private static readonly char[] Separators;

        static RelationUtils()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            NonImportantWords = config.GetSection("Title:NonImportantWords").Get<string[]>();
            Separators = config.GetSection("Title:Separators").Get<char[]>();
        }

        public static bool ContainsAnyKeyword(this string title, string[] keywords)
        {
            return keywords.Any(k => title.ToLower().Contains(k.ToLower()));
        }

        public static bool ContainsEveryKeyword(this string title, string[] keywords)
        {
            return keywords.All(k => title.ToLower().Contains(k.ToLower()));
        }

        public static bool IsSubtitleOf(this string firstTitle, string secondTitle)
        {
            return secondTitle.ToLower().Contains(firstTitle.ToLower());
        }

        public static bool ContainsAnySharedWord(this string firstTitle, string secondTitle)
        {
            var firstWords = firstTitle.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            var secondWords = secondTitle.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            bool hasSharedWord = firstWords.Any(
                fw => !NonImportantWords.Contains(fw.ToLower()) &&
                secondWords.Any(sw => WordsAreEqual(sw, fw)));
            return hasSharedWord;
        }

        public static bool ContainsEverySharedWord(this string firstTitle, string secondTitle)
        {
            if (firstTitle.Count() < secondTitle.Count())
                return ShorterTitleContainsEverySharedWord(firstTitle, secondTitle);
            return ShorterTitleContainsEverySharedWord(secondTitle, firstTitle);
        }

        private static bool ShorterTitleContainsEverySharedWord(string shorterTitle, string longerTitle)
        {
            var shorterTitleWords = shorterTitle.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            var longerTitleWords = longerTitle.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            bool hasAllSharedWord = shorterTitleWords.All(stw => longerTitleWords.Any(ltw => WordsAreEqual(ltw, stw)));
            return hasAllSharedWord;
        }

        private static bool WordsAreEqual(string firstWord, string secondWord)
        {
            return firstWord.RemoveSpecialCharacters().ToLower()
                .Equals(secondWord.RemoveSpecialCharacters().ToLower());
        }

        public static string RemoveSpecialCharacters(this string title)
        {
            title = Regex.Replace(title, @"[^\w\d\s]", string.Empty);
            title = Regex.Replace(title, @"x2", string.Empty);
            return title;
        }
    }
}
