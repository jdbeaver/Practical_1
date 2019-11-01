using System.Collections.Generic;
using Moq;
using src.Controllers;
using src.Services.Abstract;
using src.Models;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace tests
{
    public class TestSiteInfoController
    {
        [Fact]
        public void Get_ReturnsBadResponse_ModelStateIsValid()
        {
            //with .NET Core v2.1 - ModelState validation is done automatically with APIController attribute 
            //may now be superfluous but will keep this for now
            // Arrange
            var mockAWSGatewayService = new Mock<IAWSGateway>();
            var mockLogger = new Mock<ILogger<SiteInfoController>>();

            var controller = new SiteInfoController(mockAWSGatewayService.Object, mockLogger.Object);
            controller.ModelState.AddModelError("Address","Required");

            // Act
            Task<string> result = controller.Get(new AvailableServices());

            // Assert
            Assert.False(controller.ModelState.IsValid);
            //not creating a customized result so not checking the actual return here
            //using the default 400 response supplied the ApiController attribute.
        }

        [Fact]
        public async Task Get_ReturnResponse_InvalidAddress()
        {
            // Arrange
            var mockAWSGatewayService = new Mock<IAWSGateway>();
            var mockLogger = new Mock<ILogger<SiteInfoController>>();
            var controller = new SiteInfoController(mockAWSGatewayService.Object, mockLogger.Object);
            AvailableServices request = new AvailableServices(){
                Address = "-invaliddomainaddress",
                Service = new List<string>{"rdap", "freegeoip"},
                SearchType = "ip"
            };

            // Act
            var result = await controller.Get(request);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, "{\"Address\":[\"Please provide a valid IP or domain name\"]}");
        }

        [Fact]
        public async Task Get_ReturnResponse_Valid()
        {
            // Arrange
            var mockAWSGatewayService = new Mock<IAWSGateway>();
            var mockLogger = new Mock<ILogger<SiteInfoController>>();
            var controller = new SiteInfoController(mockAWSGatewayService.Object, mockLogger.Object);
            AvailableServices request = new AvailableServices(){
                Address = "34.218.207.214",
                Service = new List<string>{"rdap"},
                SearchType = "ip"
            };
            mockAWSGatewayService.Setup(m => m.GetMServiceResults("rdap", request.SearchType, request.Address)).ReturnsAsync(GenerateMserviceData());
            
            // Act
            var result = await controller.Get(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("{\"RDAP\": " + "[{\"objectClassName\":\"domain\",\"bunchofotherdata\":\"blahblah\"}]}", result);
        }


        private string GenerateMserviceData(){
            string rdap_response = "{\"objectClassName\":\"domain\",\"bunchofotherdata\":\"blahblah\"}";
            return rdap_response;
        }
    }
}
