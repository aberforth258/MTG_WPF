using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MTG_WPF
{
    public class CardFace
    {
        [JsonProperty("object")]
        public string objectName { get; set; }
        [JsonProperty("name")]
        public string cardName { get; set; }
        [JsonProperty("mana_cost")]
        public string manaCost { get; set; }
        [JsonProperty("type_line")]
        public string typeLine { get; set; }
        [JsonProperty("oracle_text")]
        public string oracleText { get; set; }
        public List<string> colors { get; set; }
        public string power { get; set; }
        public string toughness { get; set; }
        public string artist { get; set; }
        [JsonProperty("artist_id")]
        public string artistId { get; set; }
        [JsonProperty("illustration_id")]
        public string illustrationId { get; set; }
        [JsonProperty("image_status")]
        public string imageStatus { get; set; }
        [JsonProperty("image_uris")]
        public CardImages images { get; set; }
        [JsonProperty("color_indicator")]
        public List<string> colorIndicators { get; set; }
    }
}
