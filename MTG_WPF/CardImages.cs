using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MTG_WPF
{
    public class CardImages
    {
        public string small { get; set; }
        public string normal { get; set; }
        public string large { get; set; }
        public string png { get; set; }
        public string art_crop { get; set; }
        public string border_crop { get; set; }


        public Bitmap GetLargeImage()
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(large);
            System.Net.WebResponse response = request.GetResponse();
            System.IO.Stream responseStream =
            response.GetResponseStream();
            Bitmap bitmap2 = new Bitmap(responseStream);

            return bitmap2;
        }
    }
}
