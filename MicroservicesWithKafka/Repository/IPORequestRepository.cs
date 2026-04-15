public interface IPORequestRepository
{
    Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
        ExecuteAsync(object request);
}
