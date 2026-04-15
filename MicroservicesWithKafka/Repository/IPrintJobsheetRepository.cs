namespace MicroservicesWithKafka.Repository
{
    public interface IPrintJobsheetRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
