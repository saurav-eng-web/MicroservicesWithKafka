using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;

public class PORequestRepository : IPORequestRepository
{
    private readonly SQLConnectionService _sql;

    public PORequestRepository(SQLConnectionService sql)
    {
        _sql = sql;
    }

    public async Task<(IEnumerable<dynamic>, int, string)>
        ExecuteAsync(object request)
    {
        using var conn = _sql.GetConnection();
        await conn.OpenAsync();

        using var multi = await conn.QueryMultipleAsync(
            "SP_CRUD_PORequest",
            request,
            commandType: CommandType.StoredProcedure
        );

        var sets = new List<List<dynamic>>();

        // Read ALL result sets safely
        while (!multi.IsConsumed)
        {
            var result = (await multi.ReadAsync<dynamic>()).AsList();
            sets.Add(result);
        }

        // DEFAULTS
        int code = 500;
        string msg = "Stored procedure did not return a status result set";

        // LAST result set is assumed to be STATUS
        var statusSet = sets.LastOrDefault();

        // Validate status set
        if (statusSet != null && statusSet.Count > 0)
        {
            if (statusSet[0] is IDictionary<string, object> row)
            {
                // Safe extraction
                if (row.ContainsKey("StatusCode") && row["StatusCode"] != null)
                    code = Convert.ToInt32(row["StatusCode"]);

                if (row.ContainsKey("StatusMsg") && row["StatusMsg"] != null)
                    msg = row["StatusMsg"].ToString();
                else if (row.ContainsKey("Message") && row["Message"] != null)
                    msg = row["Message"].ToString();
            }
        }
        else
        {
            // ❗ SP returned no status — throw to surface SQL issues
            throw new Exception("No StatusCode/StatusMsg returned from stored procedure");
        }

        var data = sets.Count > 1 ? sets.Take(sets.Count - 1).SelectMany(x => x): new List<dynamic>();

        return (data, code, msg);
    }
}
