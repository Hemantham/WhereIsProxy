using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WhereIsProxy.MappingProviders;
using WhereIsProxy.Models;


namespace WhereIsProxy.Controllers
{
    public class WhereIsMapsController : ApiController
    {
       
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Map(SOAPEnvelopeBase inputEnvelop)
        {
            string responseString = ResponseXmlFactory.Get(inputEnvelop);

            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent(responseString, Encoding.UTF8, "text/xml");

            return response;

        }
    }
}