using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IDocumentUploadRepository
    {
        Task<int> InsertUploadAsync(DocumentUpload doc);
        Task<int> InsertPartAsync(DocumentUpload doc);
        Task UpdateUploadAsync(DocumentUpload doc);
        Task UpdatePartAsync(DocumentUpload doc);
        Task DeleteUploadAsync(int id);
        Task DeletePartAsync(int id);
        Task<IEnumerable<DocumentUpload>> GetByTicketAsync(string ticketKey,string accessUser);
    }
}
