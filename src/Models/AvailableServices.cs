using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace src.Models
{
    public class AvailableServices
    {
        /// <summary>Provide a service to check site (rdap, geoip, freegeoip)</summary>
        public List<String> Service { get; set; }
        /// <summary>Provide search type (ip, domain)</summary>
        public string SearchType { get; set; }
        /// <summary>[Required] Provide ip or domain address</summary>
        [Required(ErrorMessage = "Please provide an IP or domain name")]
        public string Address { get; set; }
        
    }
}