using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AniCharades.Common.Titles
{
    internal static class TitlePatterns
    {
        public static readonly string OtherThanSemiColon = "[^:]";
        public static readonly string NonAscii = @"[^\u0000-\u007F]+";
        public static readonly string OtherThanSemicolonWithNumbers = @"([0-9]\.[0-9])?[^:]+";
        public static readonly string SubtitlePattern = CreateSubtitlePattern();
        public static readonly string LooseSubtitleSeparators = "[!?:]";
        public static readonly string AlphaNumeric = @"[A-Za-z0-9\s]";
        public static readonly string LooseSubtitlePattern = $@"(?<mainTitle>{AlphaNumeric}+){LooseSubtitleSeparators}(?<subTitle>{AlphaNumeric}+)";
        public static readonly string ExtraCharactersPattern = @"^(?<mainPart>[^!?*']+[!'?*])(?<extraPart>[!'?*]+$)";
        public static readonly string CustomCharactersPattern = @"(?<mainPart1>.+)(?<extraPart>x2)(?<mainPart2>.+)$";
        public static readonly string CollaborationPattern = @"(?<firstPart>.+)\sx\s(?<secondPart>.+)";
        public static readonly string NonAlphanumericAtEndPattern = @"(?<mainPart>[A-Za-z0-9\s]+)(?<endPart>[^A-Za-z0-9\s]+)$";
        public static readonly string YearInBracketsPattern = @"\([1-2][0-9][0-9][0-9]\)";

        private static string CreateSubtitlePattern()
        {
            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{envVariable}.json", optional: true)
                    .Build();
            var subtitleEndings = config.GetSection("Title:EndingsForExplicitSubtitle").Get<string[]>();

            var patternFirstPart = $@"(?<mainTitle>{OtherThanSemiColon}+):\s(?!(.+(";
            var patternSecondPart = $@"))$)(?<subTitle>{OtherThanSemicolonWithNumbers})";
            var patternBuilder = new StringBuilder();
            patternBuilder.Append(patternFirstPart);
            for (int i = 0; i < subtitleEndings.Length; ++i)
            {
                patternBuilder.Append(subtitleEndings[i]);
                if (i != subtitleEndings.Length - 1)
                    patternBuilder.Append('|');
            }
            patternBuilder.Append(patternSecondPart);
            return patternBuilder.ToString();
        }
    }

    public static class TitleRegularExpressions
    {
        public static readonly Regex SubtitleRegex = new Regex(TitlePatterns.SubtitlePattern);
        public static readonly Regex LooseSubtitleRegex = new Regex(TitlePatterns.LooseSubtitlePattern);
        public static readonly Regex ExtraCharactersRegex = new Regex(TitlePatterns.ExtraCharactersPattern);
        public static readonly Regex CustomCharactersRegex = new Regex(TitlePatterns.CustomCharactersPattern);
        public static readonly Regex CollaborationRegex = new Regex(TitlePatterns.CollaborationPattern);
        public static readonly Regex NonAlphanumericAtEndRegex = new Regex(TitlePatterns.NonAlphanumericAtEndPattern);
        public static readonly Regex YearInBracketsRegex = new Regex(TitlePatterns.YearInBracketsPattern);
    }
}
