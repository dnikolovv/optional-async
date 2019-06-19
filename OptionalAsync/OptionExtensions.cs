using System;

namespace Optional.Extensions
{
    public static class OptionExtensions
    {
        public static Option<T, TException> Filter<T, TException>(this Option<T, TException> option, Func<T, bool> predicate, TException exception) =>
            option.Match(
                x => predicate(x) ?
                        option :
                        Option.None<T, TException>(exception),
                _ => option);

        public static Option<T, TException> SomeWhen<T, TException>(
            this T value,
            Func<T, bool> predicate,
            Func<T, TException> exceptionFactory)
        {
            var result = predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exceptionFactory(value));
        }
    }
}
