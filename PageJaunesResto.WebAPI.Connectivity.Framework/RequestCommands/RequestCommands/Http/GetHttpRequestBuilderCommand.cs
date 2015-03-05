using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using PageJaunesResto.WebAPI.Connectivity.Framework.Helpers;

namespace PageJaunesResto.WebAPI.Connectivity.Framework.RequestCommands.RequestCommands.Http
{
    public class GetHttpRequestBuilderCommand : IRequestBuilderCommand
    {
        private readonly string _methodName;

        public GetHttpRequestBuilderCommand(string methodName)
        {
            _methodName = methodName;
        }

        public async Task<TReturnType> BuildRequest<TReturnType>(string url, params KeyValuePair<string, string>[] parameters)
        {
            var request = new HttpClient();
            Uri uri = new Uri(url);

            uri = new Uri(uri + _methodName.ToLower());

            if (parameters.Any())
                uri = UriBuildingHelpers.AttachParameters(uri, parameters);


            Debug.WriteLine(uri.ToString() + "\r\n " + parameters.Aggregate(string.Empty, (x, y) => x + (y.Key + " " + y.Value + "\r\n")));
            var result = await request.GetStringAsync(uri);
            Debug.WriteLine(uri.ToString() + "SUCCESS \r\n " + parameters.Aggregate(string.Empty, (x, y) => x + (y.Key + " " + y.Value + "\r\n")));
            try
            {
                return JsonConvert.DeserializeObject<TReturnType>(result);
            }
            catch (JsonSerializationException)
            {
                // retry as an array 
                return JsonConvert.DeserializeObject<IEnumerable<TReturnType>>(result).First();
            }
        }

        public async Task BuildRequest(string url, params KeyValuePair<string, string>[] parameters)
        {
            var request = new HttpClient();
            Uri uri = new Uri(url);

            // TODO : Add method name

            if (parameters.Any())
                uri = UriBuildingHelpers.AttachParameters(uri, parameters);

            Debug.WriteLine(uri.ToString() + "\r\n " + parameters.Aggregate(string.Empty, (x, y) => x + (y.Key + " " + y.Value + "\r\n")));
            await request.GetStringAsync(uri);
        }

    }
}