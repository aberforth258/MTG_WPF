using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace MTG_WPF
{
    class ImageProcessor
    {
        // ***********************************************************************************
        // ************************* PRIVATE VARIABLES ***************************************
        private Mat imageToProcess;
        private string newLine = System.Environment.NewLine;
        private OCREngine ocrEngine = new OCREngine();

        // ***********************************************************************************
        // ************************* PRIVATE VARIABLES ***************************************
        public Mat croppedImageMat { get; private set; }
        public Bitmap croppedImageBitmap { get; private set; }
        public Mat titleImageMat { get; private set; }
        public Bitmap titleImageBitmap { get; private set; }
        public Mat setImageMat { get; private set; }
        public Bitmap setImageBitmap { get; private set; }
        public Mat artistImageMat { get; private set; }
        public Bitmap artistImageBitmap { get; private set; }
        public string errorText { get; private set; }

        public string imageTitleText { get; private set; }
        public List<string> imageTitleTextLines { get; private set; }
        public int imageTitleTextLinesCount { get; private set; }
        public string imageArtistText { get; private set; }
        public List<string> imageArtistTextLines { get; private set; }
        public int imageArtistTextLinesCount { get; private set; }

        public string imageCardNumber { get; private set; }

        /// <summary>
        /// Initializes ImageProcessor class with provided Bitmap
        /// </summary>
        /// <param name="_img">Bitmap to be used in processor</param>
        public ImageProcessor(Bitmap _img)
        {
            imageToProcess = BitmapConverter.ToMat((Bitmap)_img.Clone());
        }
        /// <summary>
        /// Initializes ImageProcessor class with provided Mat image
        /// </summary>
        /// <param name="_img">Mat image to be used in processor</param>
        public ImageProcessor(Mat _img)
        {
            imageToProcess = _img.Clone();
        }
        /// <summary>
        /// Initializes ImageProcessor class
        /// </summary>
        /// <param name="_img">Mat image to be used in processor</param>
        public ImageProcessor()
        {

        }

        /// <summary>
        /// Processes the image with default image provided in contructor.
        /// </summary>
        /// <param name="_img">Mat image to be used in processor</param>
        public async Task<bool> ProcessImage()
        {

            return await ProcessImage(0);
        }
        /// <summary>
        /// Processes the image with default image provided in contructor
        /// </summary>
        /// <param name="_img">Mat image to be used in processor</param>
        public async Task<bool> ProcessImage(Mat _img)
        {

            return await ProcessImage(0);
        }

        /// <summary>
        /// Private process image to have single instance of return
        /// </summary>
        /// <param name="_i">Not used, default 0</param>
        private async Task<bool> ProcessImage(int _i = 0)
        {
            bool result = false;
            GetCroppedImages();
            
            if(croppedImageMat != null && titleImageMat != null && setImageMat !=null)
            {
                await ParseImagesToText();
                result = true;
            }

            return result;
        }

        private void GetCroppedImages()
        {
            Mat matImage = imageToProcess.Clone();
            Mat processedImage = new Mat();
            Mat croppedImage = new Mat();
            Mat titleImg = new Mat();
            Mat setImg = new Mat();
            Mat artImg = new Mat();
            Mat biggestBlob = new Mat();
            Mat thresImg = new Mat();
            Rect bBox = new Rect();
            int i;
            //Main Code
            //matImage.SaveImage("snap_org.jpg");
            processedImage = matImage.CvtColor(ColorConversionCodes.RGB2GRAY);                  //Convert to Black/White
                                                                                                //processedImage.SaveImage("Images\\snap_gray.jpg");

            for (i = 20; i <= 200; i += 10)
            {
                thresImg = processedImage.Threshold(i, 300, ThresholdTypes.Binary);             //Apply threshold
                thresImg.SaveImage("Images\\snap_thres_" + i + ".jpg");

                FindCardContour(thresImg, out bBox);                                           //Find Card contour
                if (bBox.Height > 0 && bBox.Width > 0)
                {
                    break;
                }
            }

            //biggestBlob.SaveImage("Images\\snap_Blob.jpg");

            if (bBox.Height == 0 || bBox.Width == 0)
            {
                Console.WriteLine("Box not found");
                croppedImage = null;
                titleImg = null;
                setImg = null;
                artImg = null;
            }
            else
            {
                //Mat imageClone = matImage.Clone();

                croppedImage = new Mat(matImage, bBox);                                         //Create cropped image
                croppedImage.SaveImage(@"Images\snap_cropped.jpg");

                titleImg = CropToTitle(croppedImage);
                titleImg.SaveImage(@"Images\snap_title.jpg");

                setImg = CropToSet(croppedImage);
                setImg.SaveImage(@"Images\snap_set.jpg");

                artImg = CropToArtist(croppedImage);
                artImg.SaveImage(@"Images\snap_art.jpg");
            }


            croppedImageMat = croppedImage;
            titleImageMat = titleImg;
            setImageMat = setImg;
            artistImageMat = artImg;
            
            croppedImageBitmap = croppedImageMat == null ? null : BitmapConverter.ToBitmap(croppedImageMat);
            titleImageBitmap = titleImageMat == null ? null : BitmapConverter.ToBitmap(titleImageMat);
            setImageBitmap = setImageMat == null ? null : BitmapConverter.ToBitmap(setImageMat);
            artistImageBitmap = artistImageMat == null ? null : BitmapConverter.ToBitmap(artistImageMat);

        }

        //Function to get the Card rectangle
        private void FindCardContour(Mat inputImage, out Rect box)
        {
            //Variables
            Mat biggestBlob = inputImage.Clone();
            OpenCvSharp.Point[][] contours;
            Mat copyImage = new Mat();
            Rect bBox = new Rect();
            List<Rect> recList = new List<Rect>();
            int imageSize = inputImage.Height * inputImage.Width;
            const double desiredRatio = (double)GlobalParameters.mtgCardHeight / (double)GlobalParameters.mtgCardWidth;                 //Get ratio of MTG card - Card size = 88x63
            const double ratioOffSet = 0.2;                                                                                             //Ratio range offset - 0.1 work initially. May require adjusting
            const double upperRatio = desiredRatio + ratioOffSet;                                                                       //max upper ratio
            const double lowerRatio = desiredRatio - ratioOffSet;                                                                       //min upper ratio
            double contourRatio;                                                                                                        //ratio of the contour found
            double minA;

            //Main code
            // Find the contours in the image CV_RETR_CCOMP, CV_CHAIN_APPROX_SIMPLE);
            biggestBlob.FindContours(out contours, out HierarchyIndex[] hierarchy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);

            box = new Rect();                                                                                                           //Return empty box in case no contour found
            Console.WriteLine("Found " + (int)contours.Length + " contours...");
            minA = ((double)inputImage.Height * (double)inputImage.Width) / (double)100;

            for (int i = 0; i < (int)contours.Length; i++)                                                                              //Loop through each contour
            {

                double a = Cv2.ContourArea(contours[i], false);                                                                         //Find the area of the contour
                if (a > minA 
                    //&& a != null
                    )                                                                                                              //Immediately discard any area of 5 -- BOOST PERFORMANCE SIGNIFICANTLY
                {
                    Mat newMat = biggestBlob.Clone();                                                                                   //Mat to draw contour on
                    Mat newMat2 = biggestBlob.Clone();                                                                                  //Output Mat with only one contour


                    Cv2.DrawContours(newMat, contours, i, new Scalar(0),                                                               //Must be Scalar(0) to work properly. Otherwise newMat2 comes blank and no box found 
                              8, LineTypes.Link8, hierarchy);

                    newMat2 = newMat2 - newMat;                                                                                         //Clear mat of any unnecessary contours
                    bBox = Cv2.BoundingRect(newMat2);                                                                                   //Create rectangle of the contour
                    //newMat2.SaveImage("Images\\snap_Cont_" + i + "_A" + a + "_H" + bBox.Height + "_W" + bBox.Width + ".jpg");
                    recList.Add(bBox);

                    newMat.Dispose();
                    newMat2.Dispose();
                }
            }
            int topOffset = 0;                                                                                                  //bBox top offset from frame
            int bottomOffset = 0;                                                                                               //bBox bottom offset from frame
            double boxArea = 0;
            int frameOffset = 20;
            List<Rect> sortedRect = SortRectByArea(recList);
            minA = ((double)inputImage.Height * (double)inputImage.Width) / (double)5;
            foreach (var b in sortedRect)
            {
                if (b.Height == inputImage.Height || b.Width == inputImage.Width)
                {
                    continue;
                }
                if (b.Height > b.Width)
                {
                    contourRatio = (double)b.Height / (double)b.Width;
                }
                else
                {
                    contourRatio = (double)b.Width / (double)b.Height;
                }

                topOffset = b.Top;
                bottomOffset = inputImage.Height - b.Bottom;
                boxArea = b.Width * b.Height;

                if (contourRatio <= upperRatio
                    && contourRatio >= lowerRatio
                    && b.Height >= 150
                    && topOffset >= frameOffset
                    && bottomOffset >= frameOffset
                    && boxArea >= minA
                    )
                {

                    Console.WriteLine("Bbox H: " + bBox.Height + " W: " + bBox.Width);
                    copyImage = inputImage.Clone();
                    Cv2.Rectangle(copyImage, b, new Scalar(255, 0, 0), 5);
                    //copyImage.SaveImage("snap_withRect.jpg");
                    {
                        Console.WriteLine("Box W: " + b.Width + " H: " + b.Height + " R: " + (double)b.Height / (double)b.Width + " T: " + b.Top + " B: " + b.Bottom);
                    }
                    box = b;                                                                                                     //Assign output parameter of found Rect
                    break;
                }
            }
        }

        private List<Rect> SortRectByArea(List<Rect> _recList)
        {
            List<Rect> newList = new List<Rect>();
            bool isItemAdded = false;

            for (int i = 0; i < _recList.Count; i++)
            {
                isItemAdded = false;
                if (newList.Count == 0)
                {
                    newList.Add(_recList[i]);
                }
                else
                {
                    for (int k = 0; k < newList.Count; k++)
                    {
                        int inputArea = _recList[i].Height * _recList[i].Width;
                        int newArea = newList[k].Height * newList[k].Width;

                        if (inputArea > newArea)
                        {
                            newList.Insert(k, _recList[i]);
                            isItemAdded = true;
                            break;
                        }

                        if (!isItemAdded)
                        {
                            newList.Add(_recList[i]);
                            break;
                        }
                    }
                }
            }

            return newList;
        }

        private Mat CropToTitle(Mat _img)
        {
            Mat returnMat = new Mat();
            Rect titleBox = new Rect(0, 0, _img.Width, (int)((double)_img.Height * GlobalParameters.mtgCardTitleHeightRatio));

            if (titleBox.Height > 0 && titleBox.Width > 0)
            {
                returnMat = new Mat(_img, titleBox);
            }

            return returnMat;
        }

        private Mat CropToSet(Mat _img, bool isSaga = false)
        {
            Mat newMat = _img.Clone();
            Mat setMat = new Mat();
            Rect setRect = new Rect();
            double xOffset;
            double yOffset;
            double heightCap;
            double widthCap;
            double imgHeight = (double)newMat.Height;
            double imgWidht = (double)newMat.Width;

            if ( !isSaga)
            {
                xOffset = 0.8;                     //Width Offset from frame
                yOffset = 0.53;                     //Height Offset from frame
                heightCap = 0.1;                    //Percentage Height of set icon
                widthCap = 0.15;                     //Percentage Width of set icon
                //x = 920 y = 790
                //x = 1040 y= 870
                int x = (int)(imgWidht * xOffset);
                int y = (int)(imgHeight * yOffset);
                int w = (int)(imgWidht * widthCap);
                int h = (int)(imgHeight * heightCap);

                setRect = new Rect(x, y, w, h);
                setMat = new Mat(newMat, setRect);
                setMat.SaveImage("Images\\snap_set.jpg");
            }
            

            return setMat;
        }

        private Mat CropToArtist(Mat _img)
        {
            Mat croppedImg = new Mat();
            int x = 0;
            int y = (int)((float)_img.Height * 0.85f);
            int w = _img.Width;
            int h = (int)((float)_img.Height * 0.15f);
            Rect artistBox = new Rect(x, y, w, h);

            croppedImg = new Mat(_img, artistBox);
            return croppedImg;
        }

        private async Task ParseImagesToText()
        {
            Console.WriteLine("Parsing image to Text");

            if (ocrEngine == null)
            {
                Console.WriteLine("Error initializing OCR Engine...");
                return;
            }

            await ocrEngine.GetTextFromImage(titleImageBitmap); //wait for text from image

            titleImageBitmap.Save(@"Images\snap_haha.jpg");
            if (ocrEngine.textLinesCount > 0) //Check if any text is retrieved and assign to local variables
            {
                imageTitleText = ocrEngine.text;
                imageTitleTextLines = new List<string>(ocrEngine.textLines);
                imageTitleTextLinesCount = ocrEngine.textLinesCount;
            }

            ocrEngine.ClearTextResults();
            await ocrEngine.GetTextFromImage(artistImageBitmap);

            if (ocrEngine.textLinesCount > 0) //Check if any text is retrieved and assign to local variables
            {
                imageArtistText = ocrEngine.text;
                imageArtistTextLines = new List<string>(ocrEngine.textLines);
                imageArtistTextLinesCount = ocrEngine.textLinesCount;
                ocrEngine = null;   //Dispose of OCR Engine to save resource
            }
            ocrEngine = null;   //Dispose of OCR Engine to save resource
            GetCardNumber();
            Console.WriteLine("Parsing image to Text - Found " + imageTitleTextLinesCount + " lines of title text");

            Console.WriteLine("Parsing image to Text - Found " + imageArtistTextLinesCount + " lines of artist text");

            foreach(string line in imageArtistTextLines)
            {
                Console.WriteLine(line);
            }
        }

        private void GetCardNumber()
        {
            foreach(string line in imageArtistTextLines)
            {
                bool isSlashFound = line.Contains("/");

                if(isSlashFound)
                {
                    int slashPos = line.IndexOf("/");
                    string cardNumber = "";
                    try
                    {
                        cardNumber = line.Substring(slashPos - 3, 3);
                        int cardNumberInt = 0;
                        if(int.TryParse(cardNumber,out cardNumberInt))
                        {
                            imageCardNumber = cardNumberInt.ToString();
                        }
                    }
                    catch
                    {

                    }
                    
                }
            }
        }
    }
}

