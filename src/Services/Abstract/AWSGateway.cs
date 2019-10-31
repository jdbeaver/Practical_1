using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace src.Services.Abstract
{
    public class AWSGateway : IAWSGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AWSGateway(IHttpClientFactory httpClientFactory){
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetMServiceResults(string MServiceType, string SearchType, string IPDomainAddress)
        {
            var _client = _httpClientFactory.CreateClient("AWSGatewayClient");
            string result = "";
            try
            {
                if (MServiceType.Equals("rdap"))
                {
                    result = await _client.GetStringAsync("/mservices/" + MServiceType + "/" + SearchType + "/" + IPDomainAddress);
                }
                else if (MServiceType.Equals("freegeoip"))
                {
                    result = await _client.GetStringAsync("/mservices/" + MServiceType + "/" + IPDomainAddress);
                }
                else if (MServiceType.Equals("geoip"))
                {
                    result = await _client.GetStringAsync("/mservices/" + MServiceType + "/?q=" + IPDomainAddress);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}