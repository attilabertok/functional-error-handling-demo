﻿namespace FunctionalErrorHandling.Infrastructure;

using Unit = ValueTuple;

    using static F;

    public static partial class F
    {
        public static Validation<T> Valid<T>(T value)
           => new(value ?? throw new ArgumentNullException(nameof(value)));

        // create a Validation in the Invalid state
        public static Validation.Invalid Invalid(params Error[] errors) => new(errors);
        public static Validation<T> Invalid<T>(params Error[] errors) => new Validation.Invalid(errors);
        public static Validation.Invalid Invalid(IEnumerable<Error> errors) => new(errors);
        public static Validation<T> Invalid<T>(IEnumerable<Error> errors) => new Validation.Invalid(errors);
    }

    public readonly struct Validation<T>
    {
        internal IEnumerable<Error> Errors { get; }
        internal T? Value { get; }

        public bool IsValid { get; }

        public static Validation<T> Fail(IEnumerable<Error> errors)
           => new(errors);

        public static Validation<T> Fail(params Error[] errors)
           => new(errors.AsEnumerable());

        private Validation(IEnumerable<Error> errors)
           => (IsValid, Errors, Value) = (false, errors, default);

        internal Validation(T t)
           => (IsValid, Errors, Value) = (true, Enumerable.Empty<Error>(), t);

        public static implicit operator Validation<T>(Error error)
           => new(new[] { error });
        public static implicit operator Validation<T>(Validation.Invalid left)
           => new(left.Errors);
        public static implicit operator Validation<T>(T right) => Valid(right);

        public R Match<R>(Func<IEnumerable<Error>, R> Invalid, Func<T, R> Valid)
           => IsValid ? Valid(Value!) : Invalid(Errors);

        public Unit Match(Action<IEnumerable<Error>> Invalid, Action<T> Valid)
           => Match(Invalid.ToFunc(), Valid.ToFunc());

        public IEnumerator<T> AsEnumerable()
        {
            if (IsValid) yield return Value!;
        }

        public override string ToString()
           => IsValid
              ? $"Valid({Value})"
              : $"Invalid([{string.Join(", ", Errors)}])";

        public override bool Equals(object? obj)
           => obj is Validation<T> other
              && IsValid == other.IsValid
              && (IsValid && Value!.Equals(other.Value)
                 || ToString() == other.ToString());

        public override int GetHashCode() => Match
        (
           Invalid: errs => errs.GetHashCode(),
           Valid: t => t!.GetHashCode()
        );
    }

    public static class Validation
    {
        public struct Invalid
        {
            internal IEnumerable<Error> Errors;
            public Invalid(IEnumerable<Error> errors) { Errors = errors; }
        }

        public static T GetOrElse<T>(this Validation<T> opt, T defaultValue)
           => opt.Match(
              (errs) => defaultValue,
              (t) => t);

        public static T GetOrElse<T>(this Validation<T> opt, Func<T> fallback)
           => opt.Match(
              (errs) => fallback(),
              (t) => t);

        public static Validation<R> Apply<T, R>(this Validation<Func<T, R>> valF, Validation<T> valT)
           => valF.Match(
              Valid: (f) => valT.Match(
                 Valid: (t) => Valid(f(t)),
                 Invalid: (err) => Invalid(err)),
              Invalid: (errF) => valT.Match(
                 Valid: (_) => Invalid(errF),
                 Invalid: (errT) => Invalid(errF.Concat(errT))));


        public static Validation<Func<T2, R>> Apply<T1, T2, R>
           (this Validation<Func<T1, T2, R>> @this, Validation<T1> arg)
           => Apply(@this.Map(F.Curry), arg);

        public static Validation<Func<T2, T3, R>> Apply<T1, T2, T3, R>
           (this Validation<Func<T1, T2, T3, R>> @this, Validation<T1> arg)
           => Apply(@this.Map(F.CurryFirst), arg);

        public static Validation<R> Map<T, R>
           (this Validation<T> @this, Func<T, R> f)
           => @this.Match
           (
              Valid: t => Valid(f(t)),
              Invalid: errs => Invalid(errs)
           );

        public static Validation<Func<T2, R>> Map<T1, T2, R>(this Validation<T1> @this
           , Func<T1, T2, R> func)
            => @this.Map(func.Curry());

        public static Validation<Unit> ForEach<R>
           (this Validation<R> @this, Action<R> act)
           => Map(@this, act.ToFunc());

        public static Validation<T> Do<T>
           (this Validation<T> @this, Action<T> action)
        {
            @this.ForEach(action);
            return @this;
        }

        public static Validation<R> Bind<T, R>
           (this Validation<T> val, Func<T, Validation<R>> f)
            => val.Match(
               Invalid: (err) => Invalid(err),
               Valid: (r) => f(r));


        // LINQ

        public static Validation<R> Select<T, R>(this Validation<T> @this
           , Func<T, R> map) => @this.Map(map);

        public static Validation<RR> SelectMany<T, R, RR>(this Validation<T> @this
           , Func<T, Validation<R>> bind, Func<T, R, RR> project)
           => @this.Match(
              Invalid: (err) => Invalid(err),
              Valid: (t) => bind(t).Match(
                 Invalid: (err) => Invalid(err),
                 Valid: (r) => Valid(project(t, r))));
    }