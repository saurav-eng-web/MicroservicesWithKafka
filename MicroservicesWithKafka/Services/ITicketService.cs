public interface ITicketService
{
    Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
        ProcessAsync(object payload);
}