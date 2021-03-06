using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class EnumerableExtensions
    {
        public static R[] SelectAsArray<T, R>(
            this IEnumerable<T> @this,
            Func<T, R> transform
        ) =>
            @this.Select(transform).ToArray();

        public static IEnumerable<(T, T)> EnsureSameLengthAndZipWith<T>(
            this IReadOnlyList<T> listA,
            IReadOnlyList<T> listB
        ) =>
            EnsureSameLengthAndZip(listA, listB);

        public static IEnumerable<(T, T)> EnsureSameLengthAndZip<T>(
            IReadOnlyList<T> listA,
            IReadOnlyList<T> listB)
        {
            if (listA.Count != listB.Count)
                throw new InvalidOperationException(
                    $"Expected count of {nameof(listA)} to equal count of {nameof(listB)}"
                );

            return listA.Zip(listB);
        }
    }
}