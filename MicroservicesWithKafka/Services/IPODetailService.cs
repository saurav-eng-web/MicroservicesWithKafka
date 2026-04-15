using MicroservicesWithKafka.Models;

public interface IPODetailService
{
    Task<PODetailResponse> ManagePODetailAsync(PODetailRequest request);
}
