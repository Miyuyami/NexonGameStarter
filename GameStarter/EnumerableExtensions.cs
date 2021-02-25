using System;
using System.Collections.Generic;

namespace GameStarter
{
    public static class EnumerableExtensions
    {
        public static Dictionary<TKey, TSource> ToDictionaryDupes<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => source.ToDictionaryDupes(keySelector, x => x, null);

        public static Dictionary<TKey, TSource> ToDictionaryDupes<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            => source.ToDictionaryDupes(keySelector, x => x, comparer);

        public static Dictionary<TKey, TElement> ToDictionaryDupes<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => source.ToDictionaryDupes(keySelector, elementSelector, null);

        public static Dictionary<TKey, TElement> ToDictionaryDupes<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException(nameof(elementSelector));
            }

            Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);

            foreach (TSource item in source)
            {
                dictionary[keySelector(item)] = elementSelector(item);
            }

            return dictionary;
        }
    }
}
