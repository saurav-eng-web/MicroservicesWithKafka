using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    internal static class LogisticSpRunner
    {
        private static readonly JsonSerializerOptions PayloadOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static async Task<List<IDictionary<string, object?>>> RunAsync(
            IDbConnection connection,
            string spName,
            object? parameters = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null)
        {
            var payload = new { Payload = JsonSerializer.Serialize(parameters ?? new { }, PayloadOptions) };

            using var multi = await connection.QueryMultipleAsync(
                spName, payload, transaction, commandTimeout, CommandType.StoredProcedure);

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

            return resultSets.Take(resultSets.Count - 1).SelectMany(x => x).ToList();
        }
    }
}
