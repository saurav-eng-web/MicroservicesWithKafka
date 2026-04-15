using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class DocumentUploadService : IDocumentUploadService
    {
        private readonly IDocumentUploadRepository _repo;

        public DocumentUploadService(IDocumentUploadRepository repo)
        {
            _repo = repo;
        }

        public Task<int> InsertUploadAsync(DocumentUpload doc) => _repo.InsertUploadAsync(doc);
        public Task<int> InsertPartAsync(DocumentUpload doc) => _repo.InsertPartAsync(doc);
        public Task UpdateUploadAsync(DocumentUpload doc) => _repo.UpdateUploadAsync(doc);
        public Task UpdatePartAsync(DocumentUpload doc) => _repo.UpdatePartAsync(doc);
        public Task DeleteUploadAsync(int id) => _repo.DeleteUploadAsync(id);
        public Task DeletePartAsync(int id) => _repo.DeletePartAsync(id);
        public Task<IEnumerable<DocumentUpload>> GetByTicketAsync(string ticketKey, string accessUser) => _repo.GetByTicketAsync(ticketKey, accessUser);
    }
}
