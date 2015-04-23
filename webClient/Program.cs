using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using webClient.ServiceReference1;

namespace webClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var proxy = new WebProxy("127.0.0.1", 8888);
            proxy.BypassProxyOnLocal = false;
            GlobalProxySelection.Select = proxy;

            ServiceReference1.WhereIsAdapterServiceClient rerefernce = new WhereIsAdapterServiceClient();
            
          
            var code =  rerefernce.geocode(new geocode
            { 
                authorisationField  = new soapAuth{ passwordField = "pword"}
            });
        }
    }
}
