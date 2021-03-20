using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Windows.Media.Ocr;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Graphics;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Runtime;

namespace MTG_WPF
{
    
    class OCREngine
    {
        // ***********************************************************************************
        // ************************* PRIVATE VARIABLES ***************************************
        private OcrEngine       ocr;


        // ***********************************************************************************
        // ************************* PUBLIC VARIABLES ****************************************
        public string           text            { get; private set; }
        public List<string> textLines = new List<string>();
        public int              textLinesCount  { get; private set; }

        // ***********************************************************************************
        // ************************* PUBLIC FUNCTIONS ****************************************
        #region
        public OCREngine()
        {
            
            Language lang = new Language("en");

            ocr = OcrEngine.TryCreateFromLanguage(lang);

            if(ocr == null)
            {
                Console.WriteLine("Failed initiating OCR Class");
            }

        }

        public async Task GetTextFromImage(Bitmap _img)
        {

            SoftwareBitmap s_Bitmap;
            //Convert Bitmap to SoftwareBitmap
            using (Windows.Storage.Streams.InMemoryRandomAccessStream stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                _img.Save(stream.AsStream(), ImageFormat.Bmp);//choose the specific image format by your own bitmap source
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                s_Bitmap = await decoder.GetSoftwareBitmapAsync();
            }

            OcrResult result = await ocr.RecognizeAsync(s_Bitmap);
            ParseResult(result);
            //PrintResult(result);
        }

        public void ClearTextResults()
        {
            text = null;
            textLines.Clear();
            textLinesCount = 0;
        }

        #endregion

        // ***********************************************************************************
        // ************************* PRIVATE FUNCTIONS ****************************************
        #region
        private void CropImageToBorders(Bitmap _img)
        {

            Mat matNewImage = new Mat();
            Mat matImg = BitmapConverter.ToMat(_img);

            Cv2.Canny(matImg, matNewImage, 50, 200);

            matNewImage.SaveImage("snap_mat_canny.jpg");
            //return newImage;
        }

        private void ParseResult(OcrResult _result)
        {
            text = _result.Text; //Assign whole result into string

            //Assign to string array
            int i = 0;
            string lineText;
            foreach (var line in _result.Lines)
            {
                lineText = "";
                foreach (var word in line.Words)
                {
                    lineText = lineText + word.Text + " ";
                }
                textLines.Add(lineText);
                i++;
            }
            textLinesCount = _result.Lines.Count;
        }

        #endregion
    }
}
