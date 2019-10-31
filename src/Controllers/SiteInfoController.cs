using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using src.Models;
using src.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.Extensions.Logging;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SiteInfoController : ControllerBase
    {
        private readonly IAWSGateway _mserviceresults;
        private readonly ILogger _logger;
        
        public SiteInfoController(IAWSGateway mserviceresults, ILogger<SiteInfoController> logger){
           _mserviceresults = mserviceresults;
            _logger = logger;
        }

        /// <summary>
        /// Gets site lookup information based on service(s) provided as paramater 
        /// </summary>
        /// <param name="service">Provide a service to check site (rdap, geoip, freegeoip)</param>
        [HttpGet]
        public async Task<string> Get([FromQuery] AvailableServices service)
        {
            //validate inputs - here I am only validating the form of the input. I could actually
            //validate whether the IP or domain exists - but that seems to be part of what this 
            //service is for - site status - inactive, exists, etc.
            //check IP address format - determine if valid
            IPAddress result;
            IPAddress.TryParse(service.Address, out result);
            _logger.LogDebug("out result: " + result);
            if (IPAddress.TryParse(service.Address, out result))
            {
                 _logger.LogDebug("Valid IP address: " + service.Address);
                //valid IP
            } else {
                //not a valid IP number - now check if this is a valid domain name 
                //loose check as this can be a partially qualified domain name
                if(!Uri.CheckHostName(service.Address).Equals(UriHostNameType.Dns)){
                    //we dont have a valid IP or a valid domain name - let user know in the response.
                    return "{\"Address\":[\"Please provide a valid IP or domain name\"]}";
                }
                _logger.LogDebug("Valid domain name provided: " + service.Address);
            }
           
            //user has requested data from a given service or services
            string fullResults = "";
            if(service.Service != null && service.Service.Count != 0){
                foreach(var reqservice in service.Service){
                    if(reqservice.ToLower().Equals("rdap")){
                        var rdap_results = await _mserviceresults.GetMServiceResults(reqservice.ToLower(), service.SearchType, service.Address);
                        fullResults += "{\"RDAP\": [" + rdap_results + "]}\n\n";
                    } else if (reqservice.ToLower().Equals("freegeoip")){
                        var freegeoip_results = await _mserviceresults.GetMServiceResults(reqservice.ToLower(), null, service.Address);
                        fullResults += "{\"FreeGeoIP\": [" + freegeoip_results + "]}\n\n";
                    } else if (reqservice.ToLower().Equals("geoip")){
                        var freegeoip_results = await _mserviceresults.GetMServiceResults(reqservice.ToLower(), null, service.Address);
                        fullResults += "{\"GeoIP\": [" + freegeoip_results + "]}\n\n";
                    }
                }
            } else { //user did not provide a service so return a set of default information
                //geoip && rdap will the default two
                var rdap_results = await _mserviceresults.GetMServiceResults("rdap", service.SearchType, service.Address);
                fullResults += "{\"RDAP\": [" + rdap_results + "]}\n\n";
                var freegeoip_results = await _mserviceresults.GetMServiceResults("geoip", null, service.Address);
                fullResults += "{\"GeoIP\": [" + freegeoip_results + "]}\n\n";
            }
            return fullResults;
        }
    }
}
