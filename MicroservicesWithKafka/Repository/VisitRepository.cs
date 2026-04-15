using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class VisitRepository : IVisitRepository
    {
        private readonly SQLConnectionService _sql;
        public VisitRepository(SQLConnectionService sql) => _sql = sql;

        public async Task<IEnumerable<dynamic>> CreateVisitAsync(Visit visit)
        {
            using var conn = _sql.GetConnection();
            var parameters = new DynamicParameters(visit);
            parameters.Add("@Type", "INSERT");
            return await conn.QueryAsync("SP_CRUD_VISIT", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> UpdateVisitAsync(Visit visit)
        {
            using var conn = _sql.GetConnection();
            var parameters = new DynamicParameters(visit);
            parameters.Add("@Type", "UPDATE");
            return await conn.QueryAsync("SP_CRUD_VISIT", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> UpdateVisitByTicketAsync(Visit visit)
        {
            using var conn = _sql.GetConnection();

            var parameters = new DynamicParameters(visit);

            parameters.Add("@Type", "UPDATE-By-Ticket"); // 🔴 IMPORTANT

            return await conn.QueryAsync(
                "SP_CRUD_VISIT",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }


        public async Task<IEnumerable<dynamic>> DeleteVisitAsync(long slNo)
        {
            using var conn = _sql.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "DELETE");
            parameters.Add("@SlNo", slNo);
            return await conn.QueryAsync("SP_CRUD_VISIT", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetVisitByIdAsync(long slNo)
        {
            using var conn = _sql.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GET_BY_ID");
            parameters.Add("@SlNo", slNo);
            return await conn.QueryAsync("SP_CRUD_VISIT", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetAllVisitsAsync()
        {
            using var conn = _sql.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GET_ALL");
            return await conn.QueryAsync("SP_CRUD_VISIT", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetVisitsByTicketNoAsync(string ticketNo)
        {
            using var conn = _sql.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Type", "GET_BY_TICKETNO"); // unchanged
            parameters.Add("@TicketNo", ticketNo);
            return await conn.QueryAsync("SP_CRUD_VISIT", parameters, commandType: CommandType.StoredProcedure);
        }
    }




}

