using System.Linq;
using MFX.Core.Quant;
using MFX.Core.Quant.Models;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> SelectNotNull<T>(this IEnumerable<T?> enumerable)
            where T : struct
        {
            return enumerable.Where(x => x.HasValue).Select(x => x.Value);
        }

        public static IEnumerable<T1> SelectNotNull<T1, T2>(this IEnumerable<T2> enumerable, Func<T2, T1?> expression)
            where T1 : struct
        {
            return enumerable.Select(expression).SelectNotNull();
        }

        public static Dictionary<T1, IEnumerable<T2>> OrderDictionary<T1, T2>(
            this IEnumerable<KeyValuePair<T1, IEnumerable<T2>>> data, bool isDescending,
            Func<KeyValuePair<T1, IEnumerable<T2>>, object> expression)
        {
            var result = isDescending
                ? data.OrderByDescending(expression)
                : data.OrderBy(expression);
            return result.ToDictionary(sp => sp.Key, sp => sp.Value);
        }

        public static IList<T> ToTypedList<T>(this IEnumerable enumerable)
        {
            return (from object item in enumerable select (T) item).ToList();
        }

        public static IEnumerable<IGrouping<BinaryObject<TKey1, TKey2>, T>> BinaryGroupBy<T, TKey1, TKey2>(
            this IEnumerable<T> enumerable, Func<T, TKey1> expression1, Func<T, TKey2> expression2)
            where TKey2 : class
            where TKey1 : class
        {
            return enumerable
                .GroupBy(item => new BinaryObject<TKey1, TKey2>(expression1(item), expression2(item)));
        }

        public static double? SumNullIfNone<T>(this IEnumerable<T> enumerable, Func<T, double?> expression)
        {
            if (!enumerable.Any()) return null;
            var data = enumerable.Select(expression).Where(d => d.HasValue);
            if (!data.Any()) return null;
            return data.Sum();
        }

        public static TResult Min<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> expression,
            TResult defaultValue)
        {
            if (enumerable == null) return defaultValue;
            if (!enumerable.Any()) return defaultValue;
            return enumerable.Min(expression);
        }

        public static TResult Max<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> expression,
            TResult defaultValue)
        {
            if (enumerable == null) return defaultValue;
            if (!enumerable.Any()) return defaultValue;
            return enumerable.Max(expression);
        }

        public static IEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> expression,
            bool ascending)
        {
            return ascending
                ? enumerable.OrderBy(expression)
                : enumerable.OrderByDescending(expression);
        }

        public static string JoinToString(this IEnumerable<string> enumerable, string separator)
        {
            if (enumerable == null) return null;
            return string.Join(separator, enumerable.ToArray());
        }

        public static string JoinToString<T>(this IEnumerable<T> enumerable, Func<T, string> expression,
            string separator)
        {
            return JoinToString(enumerable.Select(expression), separator);
        }

        public static int ItemCount<TSource, TDesc>(this IEnumerable<TSource> enumerable,
            Func<TSource, TDesc> expression)
        {
            return enumerable.Select(expression).Distinct().Count();
        }

        public static double PercentileDisc(this IEnumerable<double?> enumerable, double value)
        {
            return StatisticFunctions.GetPercentileDisc(enumerable, value);
        }

        public static double Median(this IEnumerable<double?> enumerable)
        {
            return StatisticFunctions.GetMedian(enumerable);
        }

        public static double PercentileDisc(this IEnumerable<double> enumerable, double value)
        {
            return StatisticFunctions.GetPercentileDisc(enumerable, value);
        }

        public static double StandardDeviation(this IEnumerable<double> values)
        {
            return StatisticFunctions.GetStandardDeviation(values);
        }

        public static double? StandardDeviationPerAnnum(this IDictionary<DateTime, double> values)
        {
            return StatisticFunctions.GetStandardDeviationPerAnnum(values);
        }

        public static IDictionary<DateTime, double> ConvertToPerformanceValues(
            this IDictionary<DateTime, double> values)
        {
            return StatisticFunctions.GetItemPerformanceValues(values);
        }

        public static double? Performance(this IEnumerable<double> values)
        {
            return StatisticFunctions.GetPerformance(values);
        }

        public static T TryFirst<T>(this IEnumerable<T> enumerable, params Func<T, bool>[] methods)
        {
            return TryFirstOrDefault(enumerable, default(T), methods);
        }

        public static T TryFirstOrDefault<T>(this IEnumerable<T> enumerable, T defaultValue,
            params Func<T, bool>[] methods)
        {
            foreach (var method in methods)
                if (enumerable.Any(method))
                    return enumerable.First(method);
            return defaultValue;
        }

        public static IEnumerable<T> Update<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action(item);

            return enumerable;
        }

        public static IEnumerable<T> Insert<T>(this IEnumerable<T> enumerable, int index, T item)
        {
            var list = enumerable.ToList();
            list.Insert(index, item);
            return list;
        }

        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate)
        {
            var toRemove = list.Where(predicate).ToArray();
            foreach (var item in toRemove) list.Remove(item);
        }

        public static void AddIfNotNull<T>(IList<T> list, T item) where T : class
        {
            if (item == null) return;
            list.Add(item);
        }

        public static T GetOrAdd<T>(this IList<T> list, Func<T, bool> getPredicate, Func<T> addFunction) where T : class
        {
            var item = list.SingleOrDefault(getPredicate);
            if (item == null)
            {
                item = addFunction();
                list.Add(item);
            }

            return item;
        }

        public static TResult SingleOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, bool> predicate,
            Func<T, TResult> take, TResult defaultValue)
        {
            return enumerable
                .Where(predicate)
                .Select(take)
                .DefaultIfEmpty(defaultValue)
                .SingleOrDefault();
        }


        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            return DistinctByImpl(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return source.GroupBy(keySelector, comparer).Select(g => g.First());
        }

        public static double GetDistance(this IEnumerable<double> enumerable)
        {
            return Math.Abs(enumerable.Max() - enumerable.Min());
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                action(item, index);
                index++;
            }
        }
    }
}