using MicroservicesWithKafka.Kafka;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Services.Kafka;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;


namespace MicroservicesWithKafka.Services
{
    public class ComplaintHistoryService : IComplaintHistoryService
    {

        private readonly KafkaProducerService _producer;
        private readonly IComplaintHistoryRepository _repository;

        public ComplaintHistoryService(IComplaintHistoryRepository repository, KafkaProducerService producer)
        {
            _repository = repository;
            _producer = producer;
        }

        
        public async Task<long> AddComplaintAsync(ComplaintHistory model)
        {
            // 1️⃣ Save complaint to DB
            //var ticketNo = await _repository.InsertAsync(model);

            // 2️⃣ Build Kafka message
            var kafkaMessage = new
            {
                //Id = DateTime.UtcNow.Ticks,
                model.TicketNo,
                model.ComplaintStatus,
                model.ComplaintSubStatus,
                model.EnteredBy,
                model.EnteredByID,
                model.Tdate
            };

            string jsonMessage = System.Text.Json.JsonSerializer.Serialize(kafkaMessage);


            // 3️⃣ Send message to Kafka
            await _producer.SendMessageAsync(
                "call-history",      // Kafka topic
                model.TicketNo, // key
                jsonMessage          // JSON message
            );

            return 0;
        }


        //return _repository.InsertAsync(model);


        public Task<bool> UpdateComplaintAsync(ComplaintHistory model)
        {
            return _repository.UpdateAsync(model);
        }

        public Task<bool> DeleteComplaintAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        public Task<List<ComplaintHistory>> GetAllComplaintsAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<ComplaintHistory?> GetComplaintByIdAsync(long id)
        {
            return _repository.GetByIdAsync(id);
        }
        public Task<List<ComplaintHistory>> GetAllComplaintsByTicketNoAsync(string ticketNo)
        {

            
            return _repository.GetAllByTicketNoAsync(ticketNo);

        }
    }
}
