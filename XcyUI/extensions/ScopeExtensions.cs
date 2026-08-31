using System;
using System.Collections.Generic;
using System.Linq;

namespace XcyUI.expansions
{
    public static class ScopeExtensions
    {
        public static TResult Let<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
        {
            return func(source);
        }

        // 类似Kotlin的also
        public static TSource Also<TSource>(this TSource source, Action<TSource> action)
        {
            action(source);
            return source;
        }

        public static TSource TakeOf<TSource>(this TSource source, Func<TSource, bool> action)
        {
            return action(source)? source: default(TSource);
        }

        // 类似Kotlin的run
        public static TResult Run<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
        {
            return func(source);
        }

        // 类似Kotlin的apply
        public static TSource Apply<TSource>(this TSource source, Action<TSource> action)
        {
            action(source);
            return source;
        }

        public static void ReplaceAt<T>(this List<T> list, int index, T t)
        {
            var old = list.ElementAtOrDefault(index);
            if (old != null)
            {
                list.RemoveAt(index);
                list.Insert(index, t);
            }
            else
            {
                list.Add(t);
            }
        }
    }
}
