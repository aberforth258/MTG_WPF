using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Diagnostics;

using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serialization.Json;
using Newtonsoft.Json;

namespace MTG_WPF
{
    class APIHandler
    {
        //Variables
        private string uri;
        private string parameters = "?exact=";
        private const string contentType = "application/json";

        public APIHandler(string _apiURL, string _apiParameters = "")
        {
            uri = _apiURL;
            //parameters = _apiParameters;
        }

        public CardScryfall GetCard(string _cardName, string _setCode = "" )
        {
            string apiParameters = parameters + _cardName;
            RestClient client = new RestClient(uri);
            RestRequest request = new RestRequest(apiParameters, Method.GET);
            CardScryfall card;

            request.OnBeforeDeserialization = response => { response.ContentType = contentType; };

            //Fetch response from API
            IRestResponse queryResult = client.Execute(request);

            if(queryResult.IsSuccessful)
            {
                //card = ParseToScryFall(queryResult);
                card = new CardScryfall();
            }
            else
            {
                card = new CardScryfall();
            }

            Console.WriteLine("URL: " + uri + apiParameters);
            Console.WriteLine("Result:/n" + queryResult.Content);

            return card;
        }


    }
}

