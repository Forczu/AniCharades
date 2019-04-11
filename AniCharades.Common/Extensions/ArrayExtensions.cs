﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AniCharades.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] array, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, index, result, 0, length);
            return result;
        }
    }
}
