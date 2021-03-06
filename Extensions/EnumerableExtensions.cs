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
    }
}