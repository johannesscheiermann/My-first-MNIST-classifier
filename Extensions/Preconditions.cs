using System;

namespace Extensions
{
    public static class Preconditions
    {
        public static T Require<T>(
            this T @this,
            Func<T, bool> predicate,
            Func<T, string> buildErrorMessage
        ) =>
            predicate(@this) ? @this : throw new ArgumentException(buildErrorMessage(@this));

        public static R Let<T, R>(
            this T @this,
            Func<T, R> transform
        ) =>
            transform(@this);
    }
}