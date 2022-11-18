namespace FunctionalErrorHandling.Infrastructure;

using Unit = ValueTuple;

   public static partial class F
    {
        public static Unit Unit() => default;

        // function manipulation 

        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> func)
            => t1 => t2 => func(t1, t2);

        public static Func<T1, Func<T2, T3, R>> CurryFirst<T1, T2, T3, R>
           (this Func<T1, T2, T3, R> @this) => t1 => (t2, t3) => @this(t1, t2, t3);

        public static Func<T, T> Tap<T>(Action<T> act)
           => x => { act(x); return x; };

        public static R Pipe<T, R>(this T @this, Func<T, R> func) => func(@this);

        /// <summary>
        /// Pipes the input value in the given Action, i.e. invokes the given Action on the given value.
        /// returning the input value. Not really a genuine implementation of pipe, since it combines pipe with Tap.
        /// </summary>
        public static T Pipe<T>(this T input, Action<T> func) => Tap(func)(input);

        // Using
        public static R Using<TDisp, R>(
            TDisp disposable, Func<TDisp, R> func)
            where TDisp : IDisposable
        {
            using var disp = disposable;
            return func(disp);
        }

        public static Unit Using<TDisp>(TDisp disposable
           , Action<TDisp> act) where TDisp : IDisposable
           => Using(disposable, act.ToFunc());

        public static R Using<TDisp, R>(Func<TDisp> createDisposable
           , Func<TDisp, R> func) where TDisp : IDisposable
        {
            using var disp = createDisposable();
            return func(disp);
        }

        public static Unit Using<TDisp>(Func<TDisp> createDisposable
           , Action<TDisp> action) where TDisp : IDisposable
           => Using(createDisposable, action.ToFunc());
    }
