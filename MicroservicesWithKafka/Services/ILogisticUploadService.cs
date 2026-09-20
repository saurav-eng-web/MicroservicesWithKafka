namespace MicroservicesWithKafka.Services
{
    public interface ILogisticUploadService
    {
        Task<(IEnumerable<dynamic> Data, int Count, int StatusCode, string StatusMsg)>
            ProcessAsync(IFormFile file, string uploadedBy);
    }
}
