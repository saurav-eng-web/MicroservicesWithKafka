using Confluent.Kafka;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Services;
using MicroservicesWithKafka.Services.Mongo;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly MongoDBService _mongoService;
   // private readonly IComplaintHistoryRepository _complaintHistoryRepository;
    private readonly string _bootstrapServers;
    private readonly string _insertTopic;
    private readonly string _updateTopic;
    private readonly string _payload;
    private readonly string _callhistorylog;
    private readonly ComplaintHistorySqlService _complaintHistorySqlService;
    public KafkaConsumerService(IConfiguration config, MongoDBService mongoService, ComplaintHistorySqlService ComplaintHistorySqlService)
    {
        _config = config;
        _mongoService = mongoService;
        _complaintHistorySqlService = ComplaintHistorySqlService;

        _bootstrapServers = _config["Kafka:BootstrapServers"]
            ?? throw new ArgumentNullException("Kafka:BootstrapServers is not configured.");

        _insertTopic = _config["Kafka:Topics:InsertCall"]
            ?? throw new ArgumentNullException("Kafka:Topics:InsertCall is not configured.");

        _updateTopic = _config["Kafka:Topics:UpdateCall"]
            ?? throw new ArgumentNullException("Kafka:Topics:UpdateCall is not configured.");
        _payload = _config["Kafka:Topics:callpayload"]
            ?? throw new ArgumentNullException("Kafka:Topics:callpayload is not configured.");
        _callhistorylog = _config["Kafka:Topics:callhistorylog"]
            ?? throw new ArgumentNullException("Kafka:Topics:callhistorylog is not configured.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("✅ KafkaConsumerService started.");

        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "call-register-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(new[] { _insertTopic, _updateTopic, _callhistorylog }); // subscribe to all topics

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(TimeSpan.FromSeconds(1));
                if (result?.Message?.Value == null)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                Console.WriteLine($"📨 Message received from topic [{result.Topic}]: {result.Message.Value}");

                try
                {
                    var model = JsonSerializer.Deserialize<CallRegisterModel>(result.Message.Value, jsonOptions);

                    if (model != null && !string.IsNullOrEmpty(model.FIRSTNAME) && !string.IsNullOrEmpty(model.MOBILE))
                    {
                        var ticketNo = result.Message.Key ?? Guid.NewGuid().ToString();

                        if (result.Topic == _insertTopic)
                        {
                            // Handle Insert
                            await _mongoService.InsertCallAsync(model, ticketNo);
                            await _mongoService.InsertCustomerAsync(model, ticketNo);
                            Console.WriteLine($"Inserted new Call & Customer with TicketNo: {ticketNo}");
                        }
                        else if (result.Topic == _updateTopic)
                        {
                            // Handle Update
                            await _mongoService.UpsertCallAsync(ticketNo, model);
                            Console.WriteLine($"Updated Call with TicketNo: {ticketNo}");
                        }



                    }


                    else if (result.Topic == _callhistorylog)
                    {
                        // ComplaintHistory handling
                        //using var scope = _scopeFactory.CreateScope();
                        //var sqlService = scope.ServiceProvider.GetRequiredService<ComplaintHistorySqlService>();
                        //long insertedId = await sqlService.InsertAsync(complaint);

                        {
                            Console.WriteLine("KafkaConsumerService received a message for callhistorylog");

                            try
                            {
                                var complaint = JsonSerializer.Deserialize<ComplaintHistory>(result.Message.Value, jsonOptions);

                                if (complaint != null)
                                {
                                    Console.WriteLine("Complaint deserialized successfully. Preparing for SQL insert...");

                                    // Ensure required fields
                                    complaint.TicketNo ??= Guid.NewGuid().ToString(); // Ensure TicketNo is set
                                    complaint.Tdate ??= DateTime.UtcNow; // Default Tdate if null

                                    // Log the complaint details before insertion
                                    Console.WriteLine($"TicketNo: {complaint.TicketNo}, ComplaintStatus: {complaint.ComplaintStatus}, ComplaintSubStatus: {complaint.ComplaintSubStatus}, EnteredBy: {complaint.EnteredBy}, Tdate: {complaint.Tdate}");

                                    // Insert into SQL
                                    long insertedId = await _complaintHistorySqlService.InsertAsync(complaint);

                                    Console.WriteLine($"Inserted into ComplaintHistory DB via SQL service with Id: {insertedId}");
                                    await _mongoService.InsertCallhistoryAsync(complaint);
                                }
                                else
                                {
                                    Console.WriteLine("Complaint deserialization failed");
                                }
                            }

                            catch (Exception ex)
                            {
                                Console.WriteLine($"SQL insert failed: {ex.Message}");
                            }
                        }
                    }


                    else
                    {
                        Console.WriteLine("Skipping invalid message (missing required fields).");
                        await LogInvalidMessage(result.Message.Value);
                    }
                    
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"JSON parsing error: {jsonEx.Message}");
                    await LogInvalidMessage(result.Message.Value);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in consumer loop: {ex.Message}");
        }
        finally
        {
            consumer.Close();
            Console.WriteLine("🧹 KafkaConsumerService stopped.");
        }
    }

    private async Task LogInvalidMessage(string message)
    {
        try
        {
            var errorLogCollection = _mongoService.GetErrorLogCollection();
            await errorLogCollection.InsertOneAsync(new { RawMessage = message, LoggedAt = DateTime.UtcNow });
        }
        catch
        {
            Console.WriteLine("⚠️ Failed to log invalid message.");
        }
    }
}
