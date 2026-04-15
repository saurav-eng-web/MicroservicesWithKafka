namespace MicroservicesWithKafka.Services
{
    public interface IPaidWarrantyService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }
}
