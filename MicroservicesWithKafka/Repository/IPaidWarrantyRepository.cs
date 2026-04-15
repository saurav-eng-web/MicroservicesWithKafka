namespace MicroservicesWithKafka.Repository
{
    public interface IPaidWarrantyRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
