using System;
using System.Collections.Generic;
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
