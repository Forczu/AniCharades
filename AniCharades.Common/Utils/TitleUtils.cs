using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniCharades.Common.Utils
{
    public static class TitleUtils
    {
        public static readonly string[] RedundantLastWords;
        public static readonly string[] RedundantFirstWords;
        public static readonly string[] RedundantConnectives;
        public static readonly string[] AnimeTypes;

        static TitleUtils()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            RedundantLastWords = config.GetSection("Title:RedundantLastWords").Get<string[]>();
            RedundantFirstWords = config.GetSection("Title:RedundantFirstWords").Get<string[]>();
            RedundantConnectives = config.GetSection("Title:RedundantConnectives").Get<string[]>();
            AnimeTypes = config.GetSection("Title:AnimeTypes").Get<string[]>();
        }

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

        public static ICollection<string> GetRedundantRomanNumbers(int numberCount)
        {
            var numbers = new string[numberCount];
            for (int i = 1; i <= numberCount; ++i)
            {
                numbers[i - 1] = new RomanNumerals.RomanNumeral(i).ToString();
            }
            return numbers;
        }
    }
}
