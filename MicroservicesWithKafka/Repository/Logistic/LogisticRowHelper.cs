using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>
    /// Helpers for reading Dapper dynamic rows in the Logistic repositories. Behaves like the old SqlDataReader helpers:
    /// column names are matched case-insensitively, a missing column or NULL gives an empty string, and values are
    /// converted with ToString() (so numbers / bit columns come out exactly as before).
    /// </summary>
    internal static class LogisticRowHelper
    {
        // An SP can return the same column name twice (sp_LogisticDashBoard returns tbl_SfFGStock twice). Like
        // SqlDataReader["name"], the FIRST column with that name wins, so a duplicate must not throw.
        public static List<IDictionary<string, object?>> ToRows(IEnumerable<dynamic> rows)
            => rows.Select(r =>
            {
                var copy = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var pair in (IDictionary<string, object?>)r)
                    copy.TryAdd(pair.Key, pair.Value);
                return (IDictionary<string, object?>)copy;
            }).ToList();

        /// <summary>First matching column as string, or empty when the column is absent / NULL.</summary>
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
