using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class CallScheduleRepository : ICallScheduleRepository
    {
        private readonly SQLConnectionService _sqlConnectionService; // Assume this service exists

        public CallScheduleRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private const string StoredProcedureName = "SP_CRUD_CallShedule";

        // --- CREATE ---
        public async Task<CallSchedule> CreateAsync(CallSchedule model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Action", "INSERT");
            parameters.Add("@SfId", model.SfId);
            parameters.Add("@EngId", model.EngId);
            parameters.Add("@AppointmentDate", model.AppointmentDate);
            parameters.Add("@AppointmentTime", model.AppointmentTime);
            parameters.Add("@TalkToWhomeAllot", model.TalkToWhomeAllot);
            //parameters.Add("@Tdate", model.Tdate);
            parameters.Add("@TicketNo", model.TicketNo);
            parameters.Add("@TicketKey", model.TicketKey);
            parameters.Add("@Status", model.Status);
            parameters.Add("@ApptDateandTime", model.ApptDateandTime);

            // Stored procedure returns NewAppointmentID, StatusMsg, StatusCode
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                StoredProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Map the result
            model.ID = result?.NewAppointmentID ?? 0;
            model.StatusMsg = result?.StatusMsg;
            model.StatusCode = result?.StatusCode;

            return model;
        }

        // --- UPDATE ---
        public async Task<CallSchedule> UpdateAsync(CallSchedule model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Action", "UPDATE");
            parameters.Add("@ID", model.ID); // ID is mandatory for UPDATE
            parameters.Add("@SfId", model.SfId);
            parameters.Add("@EngId", model.EngId);
            parameters.Add("@AppointmentDate", model.AppointmentDate);
            parameters.Add("@AppointmentTime", model.AppointmentTime);
            parameters.Add("@TalkToWhomeAllot", model.TalkToWhomeAllot);
            parameters.Add("@Tdate", model.Tdate);
            parameters.Add("@TicketNo", model.TicketNo);
            parameters.Add("@TicketKey", model.TicketKey);
            parameters.Add("@Status", model.Status);
            parameters.Add("@ApptDateandTime", model.ApptDateandTime);

            // Stored procedure returns StatusMsg, StatusCode
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                StoredProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Map the result
            model.StatusMsg = result?.StatusMsg;
            model.StatusCode = result?.StatusCode;

            return model;
        }

        // --- DELETE ---
        public async Task<CallSchedule> DeleteAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Action", "DELETE");
            parameters.Add("@ID", id);

            // Stored procedure returns StatusMsg, StatusCode
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                StoredProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new CallSchedule
            {
                ID = id,
                StatusMsg = result?.StatusMsg,
                StatusCode = result?.StatusCode
            };
        }

        // --- GET ALL ---
        public async Task<IEnumerable<CallSchedule>> GetAllAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var result = await connection.QueryAsync<CallSchedule>(
                StoredProcedureName,
                new { Action = "SELECT" },
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }

        // --- GET BY ID ---
        public async Task<CallSchedule?> GetByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<CallSchedule>(
                StoredProcedureName,
                new { Action = "SELECT1", ID = id },
                commandType: CommandType.StoredProcedure
            );
            return result;
        }

        public async Task<CallSchedule?> GetByticketnoAsync(string ticketno)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<CallSchedule>(
                StoredProcedureName,
                new { Action = "SELECT", ticketno = ticketno },
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
    }
}
