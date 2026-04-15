public interface IPORequestService
{
    Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
        ProcessAsync(object payload, string sfId);
}
