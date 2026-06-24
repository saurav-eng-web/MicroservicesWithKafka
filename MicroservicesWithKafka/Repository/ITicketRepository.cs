public interface ITicketRepository
{
    Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
        ExecuteAsync(object request);
}