using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MTG_WPF
{
    public class CardScryfall
    {
        [JsonProperty("object") ]
        public string objectName { get; set; }
        public string id { get; set; }
        [JsonProperty("oracle_id")]
        public string oracleId { get; set; }
        [JsonProperty("mtgo_id")]
        public int mtgoId { get; set; }
        [JsonProperty("mtgo_foild_id")]
        public int mtgoFoilId { get; set; }
        [JsonProperty("tcgplayer_id")]
        public int tcgplayerId { get; set; }
        [JsonProperty("cardmarket_id")]
        public int cardmarketId { get; set; }
        [JsonProperty("name")]
        public string cardName { get; set; }
        public string lang { get; set; }
        [JsonProperty("released_at")]
        public DateTime releasedAt { get; set; }
        [JsonProperty("uri")]
        public string apiUri { get; set; }
        [JsonProperty("scryfall_uri")]
        public string scryfallUri { get; set; }
        public string layout { get; set; }
        [JsonProperty("highres_image")]
        public string highresImage { get; set; }
        [JsonProperty("image_status")]
        public string imageStatus { get; set; }
        public float cmc { get; set; }
        [JsonProperty("type_line")]
        public string typeLine { get; set; }
        public CardLegalities legalities { get; set; }
        public List<string> games { get; set; }
        public bool reserved { get; set; }
        public bool foil { get; set; }
        public bool nonfoil { get; set; }
        public bool oversized { get; set; }
        public bool promo { get; set; }
        public bool reprint { get; set; }
        public bool variation { get; set; }
        public string set { get; set; }
        [JsonProperty("set_name")]
        public string setName { get; set; }
        [JsonProperty("set_type")]
        public string setType { get; set; }
        [JsonProperty("set_uri")]
        public string setUri { get; set; }
        [JsonProperty("set_search_uri")]
        public string setSearchUri { get; set; }
        [JsonProperty("scryfall_set_uri")]
        public string scryfallSetUri { get; set; }
        [JsonProperty("rulings_uri")]
        public string rulingsUri { get; set; }
        [JsonProperty("prints_search_uri")]
        public string printsSearchUri { get; set; }
        [JsonProperty("collector_number")]
        public string collectorNumber { get; set; }
        public bool digital { get; set; }
        public string rarity { get; set; }
        [JsonProperty("card_back_id")]
        public string cardBackId { get; set; }
        public string artist { get; set; }
        [JsonProperty("border_color")]
        public string borderColor { get; set; }
        public string frame { get; set; }
        [JsonProperty("frame_effect")]
        public List<string> frameEffects { get; set; }
        [JsonProperty("fullArt")]
        public bool full_art { get; set; }
        public bool textless { get; set; }
        public bool booster { get; set; }
        [JsonProperty("story_spotlight")]
        public bool storySpotlight { get; set; }
        [JsonProperty("edhrec_rank")]
        public uint edhrecRank { get; set; }
        public CardPrices prices { get; set; }
        [JsonProperty("related_uris")]
        public CardURIs relatedUris { get; set; }
        [JsonProperty("purchase_uris")]
        public CardPurchaseURIs purchaseUris { get; set; }

        [JsonProperty("multiverse_ids")]
        public List<int> multiverseIds { get; set; }
        [JsonProperty("arena_id")]
        public string arenaId { get; set; }
        [JsonProperty("image_uris")]
        public CardImages images { get; set; }
        [JsonProperty("mana_cost")]
        public string manaCost { get; set; }
        [JsonProperty("oracle_text")]
        public string oracleText { get; set; }
        public List<string> colors { get; set; }
        [JsonProperty("color_identity")]
        public List<string> colorIdentity { get; set; }
        public List<string> keywords { get; set; }
        [JsonProperty("flavor_text")]
        public string flavorText { get; set; }
        [JsonProperty("illustration_id")]
        public string illustrationId { get; set; }
        public string power { get; set; }
        public string toughness { get; set; }
        public List<CardFace> cardFaces { get; set; }

        /// <summary>
        /// Creates Card Face instance if card is single faced
        /// </summary>
        public void CreateCardFace()
        {
            if(this.cardFaces == null)
            {
                this.cardFaces = new List<CardFace>();
                this.cardFaces.Add(new CardFace()
                {
                    cardName = this.cardName,
                    manaCost = this.manaCost,
                    typeLine = this.typeLine,
                    oracleText = this.oracleText,
                    power = this.power,
                    toughness = this.toughness,
                    artist = this.artist,
                    illustrationId = this.illustrationId,
                    imageStatus = this.imageStatus,
                    images = this.images
                });
            }

        }
    }
}

