using System;
using System.Threading.Tasks;

namespace src.Services.Abstract
{
    public interface IAWSGateway
    {
        Task<String> GetMServiceResults(string ServiceType, string SearchType, string IPDomainAddress);
    }
}