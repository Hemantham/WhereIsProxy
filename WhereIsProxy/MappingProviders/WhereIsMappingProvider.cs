using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using WhereIsProxy.Images;
using WhereIsProxy.Models.json.Maps;
using WhereIsProxy.WhereIsSchemaReference;
using System.Text.RegularExpressions;

namespace WhereIsProxy.MappingProviders
{
    public abstract class WhereIsMappingProvider<TIN, TOUT>
        where TIN : class, new()
        where TOUT : class, new()
    {
        protected static string WhereisUri = ConfigurationManager.AppSettings.Get("WhereisUri");

        protected IRestClient client;

        protected WhereIsMappingProvider()
            : this(new RestClient(WhereisUri))
        {

        }

        protected WhereIsMappingProvider(IRestClient client)
        {
            this.client = client;
        }

        protected static RestRequest getRestRequest<T>(T requestJson, string resource, string token, string password)
        {

            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("X-Auth-Password", password);
            request.AddHeader("X-Auth-Token", token);
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestJson, Formatting.Indented),
                ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            return request;
        }


        public abstract TOUT Map(TIN input);
    }






   
}