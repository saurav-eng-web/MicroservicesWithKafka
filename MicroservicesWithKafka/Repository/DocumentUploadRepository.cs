using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MicroservicesWithKafka.Repository
{
    public class DocumentUploadRepository : IDocumentUploadRepository
    {
        private readonly SQLConnectionService _sql;

        public DocumentUploadRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<int> InsertUploadAsync(DocumentUpload doc)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SP_CRUD_DocumentUpload", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "INSERT_UPLOAD");
            cmd.Parameters.AddWithValue("@Ticket_No", doc.Ticket_no);
            cmd.Parameters.AddWithValue("@DocumentURL", doc.DocumentURL);
            cmd.Parameters.AddWithValue("@DocumentType", doc.DocumentType);
            cmd.Parameters.AddWithValue("@TDATE", doc.TDATE ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", doc.Status ?? "A");

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<int> InsertPartAsync(DocumentUpload doc)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SP_CRUD_DocumentUpload", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "INSERT_PART");
            cmd.Parameters.AddWithValue("@Ticket_No", doc.Ticket_no);
            cmd.Parameters.AddWithValue("@Path1", doc.Path1);
            cmd.Parameters.AddWithValue("@PartCode", doc.PartCode);
            cmd.Parameters.AddWithValue("@CreatedDate", doc.CreatedDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", doc.Status ?? "A");

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task UpdateUploadAsync(DocumentUpload doc)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SP_CRUD_DocumentUpload", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "UPDATE_UPLOAD");
            cmd.Parameters.AddWithValue("@DocumentID", doc.DocumentID);
            cmd.Parameters.AddWithValue("@DocumentURL", doc.DocumentURL);
            cmd.Parameters.AddWithValue("@DocumentType", doc.DocumentType);
            cmd.Parameters.AddWithValue("@TDATE", doc.TDATE ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", doc.Status ?? "A");
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdatePartAsync(DocumentUpload doc)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SP_CRUD_DocumentUpload", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "UPDATE_PART");
            cmd.Parameters.AddWithValue("@DocumentID", doc.DocumentID);
            cmd.Parameters.AddWithValue("@Path1", doc.Path1);
            cmd.Parameters.AddWithValue("@PartCode", doc.PartCode);
            cmd.Parameters.AddWithValue("@CreatedDate", doc.CreatedDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", doc.Status ?? "A");
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteUploadAsync(int id)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SP_CRUD_DocumentUpload", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "DELETE_UPLOAD");
            cmd.Parameters.AddWithValue("@DocumentID", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeletePartAsync(int id)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SP_CRUD_DocumentUpload", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "DELETE_PART");
            cmd.Parameters.AddWithValue("@DocumentID", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<DocumentUpload>> GetByTicketAsync(string ticketKey, string accessUser)
        {
            var list = new List<DocumentUpload>();
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SP_CRUD_DocumentUpload", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@type", "GET_BY_TICKET");
            cmd.Parameters.AddWithValue("@Ticket_No", ticketKey);
            cmd.Parameters.AddWithValue("@AccessUser",accessUser);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DocumentUpload
                {
                    DocumentID = reader["RowNum"] != DBNull.Value ? Convert.ToInt32(reader["RowNum"]) : 0,
                    DocumentURL = reader["DocumentURL"].ToString(),
                    DocumentType = reader["DocumentType"].ToString(),
                    EnteredDate = reader["EnteredDate"].ToString(),
                    DocumentURL1 = reader["DocumentURL1"].ToString()
                });
            }
            return list;
        }
    }
}
