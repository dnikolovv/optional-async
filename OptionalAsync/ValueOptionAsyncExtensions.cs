using System;
using System.Threading.Tasks;
using Optional.Unsafe;

namespace Optional
{
    public static class ValueOptionAsyncExtensions
    {
        public static ValueTask<Option<T>> FilterAsync<T>(this Option<T> option, Func<T, ValueTask<bool>> predicate) =>
            option.MatchAsync(
                async x => await predicate(x) ?
                    option :
                    Option.None<T>(),
                () => option);

        public static ValueTask<Option<T, TException>> FilterAsync<T, TException>(this Option<T, TException> option, Func<T, ValueTask<bool>> predicate, TException exception) =>
            option.MatchAsync(
                async x => await predicate(x) ?
                    option :
                    Option.None<T, TException>(exception),
                _ => option);

        public static async ValueTask<Option<T>> FilterAsync<T>(this ValueTask<Option<T>> optionValueTask, Func<T, ValueTask<bool>> predicate) =>
            await (await optionValueTask).FilterAsync(predicate);

        public static async ValueTask<Option<T, TException>> FilterAsync<T, TException>(this ValueTask<Option<T, TException>> optionValueTask, Func<T, ValueTask<bool>> predicate, TException exception) =>
            await (await optionValueTask).FilterAsync(predicate, exception);

        public static async ValueTask<Option<T, TException>> FilterAsync<T, TException>(this ValueTask<Option<T>> optionValueTask, Func<T, ValueTask<bool>> predicate, TException exception) =>
            await (await optionValueTask)
                .FilterAsync(predicate)
                .WithExceptionAsync(exception);

        public static ValueTask<Option<TResult, TException>> FlatMapAsync<T, TException, TResult>(this ValueTask<Option<T, TException>> option, Func<T, ValueTask<Option<TResult, TException>>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)),
                none: err => new ValueTask<Option<TResult, TException>>(Option.None<TResult, TException>(err)));

        public static ValueTask<Option<TResult>> FlatMapAsync<T, TResult>(this ValueTask<Option<T>> option, Func<T, ValueTask<Option<TResult>>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)),
                none: () => new ValueTask<Option<TResult>>(Option.None<TResult>()));

        public static ValueTask<Option<TResult, TException>> FlatMapAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, ValueTask<Option<TResult, TException>>> mapping) =>
            option.MatchAsync(
                some: async val => await mapping(val),
                none: err => Option.None<TResult, TException>(err));

        public static ValueTask<Option<TResult, TException>> FlatMapAsync<T, TException, TResult>(this Option<T> option, Func<T, ValueTask<Option<TResult, TException>>> mapping, TException exception) =>
            option.MatchAsync(
                some: async val => await mapping(val),
                none: () => Option.None<TResult, TException>(exception));

        public static ValueTask<Option<TResult>> FlatMapAsync<T, TResult>(this Option<T> option, Func<T, ValueTask<Option<TResult>>> mapping) =>
            option.MatchAsync(
                some: async val => await mapping(val),
                none: () => Option.None<TResult>());

        public static ValueTask<Option<TResult>> MapAsync<T, TResult>(this ValueTask<Option<T>> option, Func<T, ValueTask<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult>(),
                none: () => new ValueTask<Option<TResult>>(Option.None<TResult>()));

        public static ValueTask<Option<TResult, TException>> MapAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, ValueTask<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult, TException>(),
                none: err => Option.None<TResult, TException>(err));

        public static ValueTask<Option<TResult, TException>> MapAsync<T, TException, TResult>(this ValueTask<Option<T, TException>> option, Func<T, ValueTask<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult, TException>(),
                none: err => new ValueTask<Option<TResult, TException>>(Option.None<TResult, TException>(err)));

        public static ValueTask<Option<TResult>> MapAsync<T, TResult>(this Option<T> option, Func<T, ValueTask<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult>(),
                none: () => Option.None<TResult>());

        public static ValueTask<Option<T, TExceptionResult>> MapExceptionAsync<T, TException, TExceptionResult>(this ValueTask<Option<T, TException>> optionValueTask, Func<TException, ValueTask<TExceptionResult>> mapping) =>
            optionValueTask.MatchAsync(
                some: val => new ValueTask<Option<T, TExceptionResult>>(Option.Some<T, TExceptionResult>(val)), 
                none: async err => Option.None<T, TExceptionResult>(await mapping(err)));

        public static ValueTask<Option<T, TExceptionResult>> MapExceptionAsync<T, TException, TExceptionResult>(this Option<T, TException> option, Func<TException, ValueTask<TExceptionResult>> mapping) =>
            option.MatchAsync(
                some: val => new ValueTask<Option<T, TExceptionResult>>(Option.Some<T, TExceptionResult>(val)), 
                none: async err => Option.None<T, TExceptionResult>(await mapping(err)));

        public static ValueTask<TResult> MatchAsync<T, TResult>(this Option<T> option, Func<T, ValueTask<TResult>> some, Func<ValueTask<TResult>> none) =>
            option.Match(some, none);

        public static ValueTask<TResult> MatchAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, ValueTask<TResult>> some, Func<TException, ValueTask<TResult>> none) =>
            option.Match(some, none);

        public static async ValueTask<TResult> MatchAsync<T, TException, TResult>(this ValueTask<Option<T, TException>> option, Func<T, ValueTask<TResult>> some, Func<TException, ValueTask<TResult>> none) =>
            await (await option).Match(some, none);

        public static async ValueTask<TResult> MatchAsync<T, TResult>(this ValueTask<Option<T>> option, Func<T, ValueTask<TResult>> some, Func<ValueTask<TResult>> none) =>
            await (await option).Match(some, none);

        public static ValueTask<TResult> MatchAsync<T, TResult>(this Option<T> option, Func<T, ValueTask<TResult>> some, Func<TResult> none) =>
            option.Match(
                some: x => some(x),
                none: () => new ValueTask<TResult>(none()));

        public static ValueTask<TResult> MatchAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, ValueTask<TResult>> some, Func<TException, TResult> none) =>
            option.Match(
                some: x => some(x),
                none: e => new ValueTask<TResult>(none(e)));

        public static async ValueTask MatchAsync<T>(this Option<T> option, Func<T, ValueTask> some, Func<ValueTask> none)
        {
            if (option.HasValue)
            {
                await some(option.ValueOrDefault());
            }
            else
            {
                await none();
            }
        }

        public static async ValueTask MatchAsync<T>(this Option<T> option, Func<T, ValueTask> some, Action none)
        {
            if (option.HasValue)
            {
                await some(option.ValueOrDefault());
            }
            else
            {
                none();
            }
        }

        public static async ValueTask MatchSomeAsync<T>(this Option<T> option, Func<T, ValueTask> some)
        {
            if (option.HasValue)
            {
                await some(option.ValueOrDefault());
            }
        }

        public static async ValueTask<Option<T, TException>> SomeNotNullAsync<T, TException>(this ValueTask<T> task, TException exception) =>
            (await task).SomeNotNull(exception);

        public static async ValueTask<Option<T>> SomeNotNullAsync<T>(this ValueTask<T> task) =>
            (await task).SomeNotNull();

        public static async ValueTask<Option<T, TException>> SomeWhenAsync<T, TException>(
            this ValueTask<T> valueValueTask,
            Func<T, bool> predicate,
            Func<T, TException> exceptionFactory)
        {
            var value = await valueValueTask;
            var result = predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exceptionFactory(value));
        }

        public static async ValueTask<Option<T, TException>> SomeWhenAsync<T, TException>(this ValueTask<T> task, Func<T, bool> predicate, TException exception) =>
            (await task).SomeWhen(predicate, exception);

        public static async ValueTask<Option<T, TException>> SomeWhenAsync<T, TException>(
            this T value,
            Func<T, ValueTask<bool>> predicate,
            TException exception)
        {
            var result = await predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exception);
        }

        public static async ValueTask<Option<T, TException>> SomeWhenAsync<T, TException>(
            this T value,
            Func<T, ValueTask<bool>> predicate,
            Func<T, TException> exceptionFactory)
        {
            var result = await predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exceptionFactory(value));
        }

        public static async ValueTask<Option<T, TException>> WithExceptionAsync<T, TException>(this ValueTask<Option<T>> option, TException exception) => new Option<T, TException>();
    }
}