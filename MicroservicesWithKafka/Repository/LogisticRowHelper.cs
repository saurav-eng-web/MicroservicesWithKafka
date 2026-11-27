using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MicroservicesWithKafka.Repository
{
    internal static class LogisticRowHelper
    {
        public static List<IDictionary<string, object?>> ToRows(IEnumerable<dynamic> rows)
            => rows.Select(r =>
            {
                var copy = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var pair in (IDictionary<string, object?>)r)
                    copy.TryAdd(pair.Key, pair.Value);
                return (IDictionary<string, object?>)copy;
            }).ToList();

        // Result set exactly as the procedure returns it (same column names, same order), values as display text.
        // Used by detail tables whose columns are decided in SQL, so a new column needs no API or UI change.
        public static List<Dictionary<string, string>> ToTable(IEnumerable<IDictionary<string, object?>> rows)
            => rows.Select(r =>
            {
                var line = new Dictionary<string, string>();
                foreach (var pair in r)
                    line[pair.Key] = Display(pair.Value);
                return line;
            }).ToList();

        private static string Display(object? value) => value switch
        {
            null or DBNull => string.Empty,
            DateTime d => d.ToString(d.TimeOfDay == TimeSpan.Zero ? "dd-MM-yyyy" : "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
            bool b => b ? "Yes" : "No",
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };

        public static string Str(IDictionary<string, object?> row, params string[] candidates)
        {
            foreach (var name in candidates)
            {
                if (row.TryGetValue(name, out var value))
                    return value == null || value == DBNull.Value ? string.Empty : value.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        public static int Int(IDictionary<string, object?> row, params string[] candidates)
            => int.TryParse(Str(row, candidates), out var value) ? value : 0;
    }
}
