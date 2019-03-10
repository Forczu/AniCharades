using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AniCharades.API.Data
{
    public static class DataConverters
    {
        public static ValueConverter<int[], string> IntArrayToStringConverter = new ValueConverter<int[], string>(
           e => String.Join(",", e.Select(p => p.ToString())),
           e => e.Split(",", StringSplitOptions.None).Select(i => Convert.ToInt32(i)).ToArray());

        public static ValueConverter<string[], string> StringArrayToStringConverter = new ValueConverter<string[], string>(
            e => String.Join('\n', e),
            e => e.Split('\n', StringSplitOptions.None).ToArray());
    }
}
