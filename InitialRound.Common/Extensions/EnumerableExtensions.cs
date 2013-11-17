using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> ts, long randomizer)
        {
            return ts.Zip(Randoms(randomizer), (t, n) => new { t, n }).OrderBy(x => x.n).Select(x => x.t);
        }

        private static IEnumerable<int> Randoms(long randomizer)
        {
            Random random = new Random((int)randomizer);

            while (true)
            {
                yield return random.Next();
            }
        }

        public static IEnumerable<V> ValuesOrEmpty<K, V>(this ILookup<K, V> lookup, K key)
        {
            return lookup.Contains(key) ? lookup[key] : Enumerable.Empty<V>();
        }
    }
}
