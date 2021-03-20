using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace MTG_WPF
{
    public class CardSet
    {
        [JsonProperty("object")]
        public string objectName {get; set;}
        public string id { get; set; }
        public string code { get; set; }
        [JsonProperty("tcgplayer_id")]
        public string tcgplayerId { get; set; }
        [JsonProperty("mtgo_code")]
        public string mtgoCode { get; set; }
        [JsonProperty("name")]
        public string setName { get; set; }
        [JsonProperty("set_type")]
        public string setType { get; set; }
        [JsonProperty("released_at")]
        public string releasedAt { get; set; }
        [JsonProperty("block_code")]
        public string blockCode { get; set; }
        public string block { get; set; }
        [JsonProperty("parent_set_code")]
        public string parentSetCode { get; set; }
        [JsonProperty("card_count")]
        public int cardCount { get; set; }
        [JsonProperty("printed_size")]
        public int printedSize { get; set; }
        public bool digital { get; set; }
        [JsonProperty("nonfoil_only")]
        public bool nonfoilOnly { get; set; }
        [JsonProperty("foil_only")]
        public bool foilOnly { get; set; }
        [JsonProperty("scryfall_uri")]
        public string scryfallUri { get; set; }
        public string uri { get; set; }
        [JsonProperty("icon_svg_uri")]
        public string iconSVGUri { get; set; }
        [JsonProperty("search_uri")]
        public string searchUri { get; set; }
    }
}
