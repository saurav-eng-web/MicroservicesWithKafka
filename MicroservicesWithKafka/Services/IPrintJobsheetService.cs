namespace MicroservicesWithKafka.Services
{
    public interface IPrintJobsheetService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }
}
