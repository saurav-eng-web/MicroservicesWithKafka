using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>
    /// Runs the SP_CRUD_Logistic* stored procedures the same way the rest of the system does (see ChallanRepository):
    /// the data result set(s) come first and the LAST result set is the status row
    /// (200 + statusMsg on success, 500 + Message on error). A non-200 status is raised as an exception, so callers
    /// that run inside a SqlTransaction still roll back exactly as before.
    /// </summary>
    internal static class LogisticSpRunner
    {
        public static async Task<List<IDictionary<string, object?>>> RunAsync(
            IDbConnection connection,
            string spName,
            object? parameters = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null)
        {
            using var multi = await connection.QueryMultipleAsync(
                spName, parameters, transaction, commandTimeout, CommandType.StoredProcedure);

            var resultSets = new List<List<IDictionary<string, object?>>>();
            while (!multi.IsConsumed)
                resultSets.Add(LogisticRowHelper.ToRows(await multi.ReadAsync()));

            var statusRow = resultSets.LastOrDefault()?.FirstOrDefault();
            if (statusRow == null)
                throw new InvalidOperationException($"{spName} did not return a status row.");

            var statusCode = LogisticRowHelper.Int(statusRow, "StatusCode");
            if (statusCode != 200)
            {
                var message = LogisticRowHelper.Str(statusRow, "statusMsg", "Message");
                throw new InvalidOperationException(
                    $"{spName} failed ({statusCode}): {(message.Length > 0 ? message : "Unknown error")}");
            }

            // Every result set before the status set is data
            return resultSets.Take(resultSets.Count - 1).SelectMany(x => x).ToList();
        }
    }
}
