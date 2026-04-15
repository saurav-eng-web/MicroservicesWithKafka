using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class MenuMasterRepository : IMenuMasterRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<MenuMasterRepository> _logger;

        public MenuMasterRepository(SQLConnectionService sql, ILogger<MenuMasterRepository> logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string Message)>
            ExecuteAsync(MenuMasterRequest model)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            // ✅ IF SELECT OR ROLE BASED FETCH → use QueryMultiple
            if (model.Action == "SELECT_ALL" || model.Action == "SELECT_BY_ROLE")
            {
                using var multi = await conn.QueryMultipleAsync(
                    "SP_CRUD_MenuMaster",
                    new
                    {
                        action = model.Action,
                        menu_role = model.Menu_Role
                    },
                    commandType: CommandType.StoredProcedure
                );

                var data = (await multi.ReadAsync<dynamic>()).AsList();

                return data.Any()
                    ? (data, 200, "Data fetched successfully")
                    : (data, 404, "No records found");
            }

            // ✅ INSERT / UPDATE / DELETE → single result only
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "SP_CRUD_MenuMaster",
                new
                {
                    action = model.Action,
                    menuid = model.MenuID,
                    menu_name = model.Menu_Name,
                    hierarchy = model.Hierarchy,
                    displayorder = model.DisplayOrder,
                    menu_role = model.Menu_Role,
                    navigation_url = model.Navigation_Url,
                    icon = model.Icon,
                    createdby = model.CreatedBy,
                    updatedby = model.UpdatedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return (
                new List<dynamic>(),
                result?.StatusCode ?? 500,
                result?.Message ?? "No response from procedure"
            );
        }
    }
}
