using System;
using System.Collections.Generic;
using System.Linq;

namespace WebTest.Util
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Truncate string if it is longer than the maxlength provided
        /// </summary>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Convert IEnumerable into batches of provided size
        /// </summary>
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source,int batchSize)
        {
            var batch = new List<TSource>();
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<TSource>();
                }
            }

            if (batch.Any()) yield return batch;
        }
    }
}
