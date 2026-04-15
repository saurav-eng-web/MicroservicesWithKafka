using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repositorie;
using MicroservicesWithKafka.Services;

public class POMasterService : IPOMasterService
{
    private readonly IPOMasterRepository _repo;

    public POMasterService(IPOMasterRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<dynamic>> ManagePOMasterAsync(POMasterRequest request)
    {
        return await _repo.ManagePOMasterAsync(request);  // return all records
    }
}
