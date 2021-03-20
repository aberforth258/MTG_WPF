using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Svg;
using RestSharp;

using Newtonsoft.Json;

namespace MTG_WPF
{
    public class APICardSearcher
    {
        // ***********************************************************************************
        // ************************* PRIVATE VARIABLES ***************************************
        RestClient apiClient;
        RestRequest apiRequest;
        private string url = "https://api.scryfall.com/cards/named";
        private const string exactParameters = "?exact=";
        private const string fuzzyParameters = "?fuzzy=";
        private const string contentType = "application/json";

        public enum SearchMode
        {
            Exact,
            Fuzzy
        }

        public CardScryfall GetCard(string _searchText, string _set = "")
        {
            IRestResponse response;
            CardScryfall newCard;
            response = RetrieveCardFromAPI(_searchText, _set, SearchMode.Exact);

            if(response.IsSuccessful)
            {
                newCard = ParseToScryFall(response);
            }
            else
            {
                response = RetrieveCardFromAPI(_searchText, _set, SearchMode.Fuzzy);

                if(response.IsSuccessful)
                {
                    newCard = ParseToScryFall(response);
                }
                else
                {
                    Console.WriteLine("Error: " + response.ErrorMessage);
                    newCard = null;
                }
            }

            return newCard;
        }
        
        private IRestResponse RetrieveCardFromAPI(string _searchText, string _set = "", SearchMode _mode = SearchMode.Exact)
        {
            string apiParameters;

            if(_mode == SearchMode.Exact)
            {
                apiParameters = exactParameters + _searchText;
            }
            else
            {
                apiParameters = fuzzyParameters + _searchText;
            }


            if(_set != "")
            {
                string setCode = GetSetCode(_set);
                apiParameters = apiParameters + "&set=" + setCode;
            }

            //Setup client and request
            apiClient = new RestClient(url);
            apiRequest = new RestRequest(apiParameters, Method.GET);

            apiRequest.OnBeforeDeserialization = response => { response.ContentType = contentType; };

            //Fetch Response from Scryfall API
            return apiClient.Execute(apiRequest);

        }


        private string GetSetCode(string _set)
        {
            return _set;
        }

        private CardScryfall ParseToScryFall(IRestResponse response)
        {
            CardScryfall newCard = new CardScryfall();

            //Settings to ignore null values during deserialization
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            //Deserialize response to ScryfallCard object
            newCard = JsonConvert.DeserializeObject<CardScryfall>(response.Content, settings);
            
            Console.WriteLine("CardName: " + newCard.cardName);

            return newCard;

        }
        

        public IRestResponse GetIResponse(string uri)
        {
            apiClient = new RestClient(uri);
            apiRequest = new RestRequest(Method.GET);

            return apiClient.Execute(apiRequest);
        }

        //Downloads and saves all set icons as jpg
        public void GetSetList()
        {
            string requestURL = @"https://api.scryfall.com/sets";
            List<CardSet> setList = new List<CardSet>();
            JsonSetPage page = new JsonSetPage();

            do
            {
                page = JsonConvert.DeserializeObject<JsonSetPage>(GetIResponse(requestURL).Content);
                if (page.data != null)
                {
                    foreach (CardSet set in page.data)
                    {
                        setList.Add(set);
                    }
                    requestURL = page.nextPage;
                }
            } while (page.hasMore);

            DBHandler db = new DBHandler();
            db.InsertSets(setList);

        }

        public void GetCardList()
        {
            string requestURL = @"https://api.scryfall.com/cards/search?q=lang%3Aen+include%3Aextras+unique%3Aprints";
            List<CardScryfall> cardList = new List<CardScryfall>();
            JsonCardPage page = new JsonCardPage();

            do
            {
                page = JsonConvert.DeserializeObject<JsonCardPage>(GetIResponse(requestURL).Content);
                if (page.data != null)
                {
                    foreach (CardScryfall card in page.data)
                    {
                        card.CreateCardFace();
                        cardList.Add(card);
                    }
                    requestURL = page.nextPage;
                }
            } while (page.hasMore);

            DBHandler db = new DBHandler();
            db.InsertCards(cardList);

        }

    }
}
