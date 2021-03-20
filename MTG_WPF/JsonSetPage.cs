using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MTG_WPF
{
    public class JsonSetPage
    {
        [JsonProperty("object")]
        public string objectName { get; set; }
        [JsonProperty("has_more")]
        public bool hasMore { get; set; }
        [JsonProperty("next_page")]
        public string nextPage { get; set; }
        public List<string> warnings { get; set; }
        public List<CardSet> data { get; set; }
    }
}
