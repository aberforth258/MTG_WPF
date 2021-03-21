using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MTG_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // ***********************************************************************************
        // ************************* PRIVATE VARIABLES ***************************************
        private VideoCapture capture;

        private Thread camera;
        private Thread search;
        private OCREngine ocrEngine;
        private List<CardScryfall> scannedCardList = new List<CardScryfall>();
        private APICardSearcher cardSearcher;
        private Bitmap imageToProcess;
        private bool isCameraRunning = false;
        private bool stopCameraRequested = false;
        private bool isOCRRunning = false;
        private bool liveOcr = false;
        private bool singleOCR = false;
        private bool isRatioSet = false;
        private bool ratioUpdateRequired = true;
        private string softwareVersion;
        private string newLine = System.Environment.NewLine;
        private string imageTitleText;
        private List<string> imageTitleTextLines;
        private int imageTitleTextLinesCount;
        private string imageArtistText;
        private string imageArtistTextLines;
        private string imageArtistTextLinesCount;
        private BitmapImage defaultPhoto = new BitmapImage(new Uri(@"swamp.jpg",UriKind.Relative));
        private const int waitBetweenOCR = 3000;


        public MainWindow()
        {
            InitializeComponent();
            cameraImage.Source = defaultPhoto;
        }



        // **********************************************************************************
        // ************************* CAMERA FUNCTIONS **************************************
        #region
        private void CaptureCamera()
        {
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.Start();
        }

        private void CaptureCameraCallback()
        {
            Mat frame = new Mat();
            Bitmap image;
            BitmapImage bitImg;
            BitmapSource bitSrc;
            capture = new VideoCapture(CaptureDevice.DShow, 0);
            capture.Open(0);
            
            isCameraRunning = true;
            ratioUpdateRequired = true;

            if (capture.IsOpened())
            {
                if (capture.FrameHeight <= 1080 || capture.FrameWidth <= 1080) //Set max resolution
                {
                    Console.WriteLine("Resolution: " + capture.FrameWidth + "x" + capture.FrameHeight);
                    capture.Set(CaptureProperty.FrameHeight, 2160);
                    capture.Set(CaptureProperty.FrameWidth, 3840);
                    Console.WriteLine("Resolution: " + capture.FrameWidth + "x" + capture.FrameHeight);
                }

                //Console.WriteLine("Camera H: " + capture.Get(CaptureProperty.FrameHeight) + "Camera W: " + capture.Get(CaptureProperty.FrameWidth));
                while (!stopCameraRequested)
                {
                    capture.Read(frame);
                    try
                    {
                        //GetPictureRatio(frame.Height, frame.Width);
                        
                        image = BitmapConverter.ToBitmap(frame);
                    }
                    catch
                    {
                        Console.WriteLine("Frame failed...");
                        image = null;
                    }


                    if ((liveOcr || singleOCR) && !isOCRRunning)
                    {
                        imageToProcess = (Bitmap)image.Clone();
                        //new Thread(delegate () { ProcessImage(imageCopy); }).Start();
                        isOCRRunning = true;
                        //search = new Thread(new ThreadStart(ProcessImage));
                        //search.Start();
                        //ProcessImage(imageCopy);
                        //SearchForCard();


                        //if (singleOCR)
                        //{
                        //    ToggleSingleOCR();
                        //}
                    }
                    //bitSrc = image.ToBitmapSource();

                    if (frame.Height > 0 && frame.Width > 0)
                    {
                        this.Dispatcher.Invoke(() =>
                                                {
                                                    cameraImage.Source = ConvertBitmap(BitmapConverter.ToBitmap(frame));
                                                }
                                                );
                    }
                    
                }
            }

            capture.Dispose();

            this.Dispatcher.Invoke(() =>
            {
                cameraImage.Source = defaultPhoto;
            }
            );
            isCameraRunning = false;
        }

        private void startCamera_Click(object sender, RoutedEventArgs e)
        {
            if ((string)startCamera.Content == "Start")
            {
                if(!isCameraRunning)
                {
                    stopCameraRequested = false;
                    CaptureCamera();                    //Start Camera in new Thread
                    startCamera.Content = "Stop";
                    
                }
            }
            else
            {
                startCamera.Content = "Start";
                stopCameraRequested = true;
                isRatioSet = false;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitMTGScanner();
        }

        private void fileFetch_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage image;
            OpenFileDialog fileDialog = new OpenFileDialog();
            if ((bool)fileDialog.ShowDialog())
            {
                string fileName = fileDialog.FileName;
                if (fileName != "" && fileName != null)
                {
                    image = new BitmapImage(new Uri(fileName));
                    cameraImage.Source = image;
                    imageToProcess = BitmapConverter.ToBitmap(image.ToMat());

                }
            }
            Thread process = new Thread(new ThreadStart(ProcessImage) );
            process.Start();
        }
        
        public BitmapImage ConvertBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }
        
        #endregion

        // **********************************************************************************
        // ************************* UI FUNCTIONS *******************************************
        #region
        private async void buttonOCRFromFile_Click(object sender, EventArgs e)
        {
            string fileName;
            BitmapImage image;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                fileName = openFileDialog.FileName;
                image = new BitmapImage(new Uri(fileName));
                

                //await ParseImageToText(image);
                //SearchForCard();
                
            }
        }

        #endregion


        // **********************************************************************************
        // ************************* SUPPORT FUNCTIONS***************************************

        private void ExitMTGScanner()
        {
            Application.Current.Shutdown();
        }

        private async Task ParseImageToText(Bitmap _img)
        {
            Console.WriteLine("Parsing image to Text");
            _img.Save("Images\\snap_search.jpg");
            ocrEngine = new OCREngine();

            if (ocrEngine == null)
            {
                Console.WriteLine("Error initializing OCR Engine...");
                return;
            }

            await ocrEngine.GetTextFromImage(_img); //wait for text from image

            if (ocrEngine.textLinesCount > 0) //Check if any text is retrieved and assign to local variables
            {
                imageTitleText = ocrEngine.text;
                imageTitleTextLines = ocrEngine.textLines;
                imageTitleTextLinesCount = ocrEngine.textLinesCount;
                ocrEngine = null;   //Dispose of OCR Engine to save resource
            }

            Console.WriteLine("Parsing image to Text - Found " + imageTitleTextLinesCount + " lines of text");
        }

        private void SearchForCard()
        {
            Console.WriteLine("Searching for card...");
            CardScryfall retrievedCard;
            cardSearcher = new APICardSearcher();

            for (int i = 0; i < imageTitleTextLines.Count; i++)
            {
                Console.WriteLine("Search for Card: " + imageTitleTextLines[i]);
                retrievedCard = cardSearcher.GetCard(imageTitleTextLines[i]);

                if (retrievedCard == null)
                {
                    Console.WriteLine("No Card Found");

                }
                else
                {
                    scannedCardList.Add(retrievedCard);
                   
                    Console.WriteLine("Items in CardList: " + scannedCardList.Count);
                    Console.WriteLine(retrievedCard.cardName);
                    SystemSounds.Beep.Play();
                    break;
                }
            }
        }

        private async void ProcessImage()
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            ImageProcessor imgProc = new ImageProcessor(imageToProcess);

            bool isImageSuccess = await imgProc.ProcessImage();
            endTime = DateTime.Now;
            Console.WriteLine("Image Process Success | " + endTime.Subtract(startTime).TotalMilliseconds + "ms");
            if (isImageSuccess)
            {
                if (imgProc.imageTitleTextLinesCount > 0)
                {
                    imageTitleText = imgProc.imageTitleText;
                    imageTitleTextLines = imgProc.imageTitleTextLines;
                    imageTitleTextLinesCount = imgProc.imageTitleTextLinesCount;
                    SearchForCard();
                }

                //ToggleCardStatusVisibility(isImageSuccess, "Image Process Success");
            }
            else
            {
                Console.WriteLine("Image Process Failed");
                //ToggleCardStatusVisibility(isImageSuccess, "Image Process Failed");
                return;
            }

            endTime = DateTime.Now;

            //ToggleElapsedVisibility(true, "Elapsed in " + endTime.Subtract(startTime).TotalMilliseconds + "ms");
            ResetSearchObjects();
            await StopCameraReading(1000);
            isOCRRunning = false;

        }

        private async Task StopCameraReading(int _milliseconds)
        {
            isOCRRunning = true;
            Console.WriteLine("OCR on Break: " + isOCRRunning);
            await Task.Delay(_milliseconds);
            isOCRRunning = false;
            Console.WriteLine("OCR on Break: " + isOCRRunning);
        }

        private void ResetSearchObjects()
        {
            imageToProcess.Dispose();
            imageTitleText = "";
            if (imageTitleTextLines != null)
            {
                imageTitleTextLines.Clear();
            }
            imageTitleTextLinesCount = 0;
        }

        
        private Bitmap FixOrientation(Bitmap _img)
        {
            PropertyItem pi = SafeGetPropertyItem(_img, 0x112);
            Bitmap retImage;
            if (pi != null)
            {
                int orientationValue = BitConverter.ToInt16(pi.Value, 0);
                Console.WriteLine(orientationValue);

                if (pi != null && pi.Type == 3)
                {
                    if (orientationValue == 1)
                    {
                        //Do Nothing
                    }
                    else if (orientationValue == 2)
                    {

                    }
                    else if (orientationValue == 3)
                    {

                    }
                    else if (orientationValue == 4)
                    {

                    }
                    else if (orientationValue == 5)
                    {

                    }
                    else if (orientationValue == 6)
                    {
                        _img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else if (orientationValue == 7)
                    {

                    }
                    else if (orientationValue == 8)
                    {

                    }
                }
            }

            retImage = _img;
            return retImage;
        }

        // A file without the desired EXIF property record will throw ArgumentException.
        private PropertyItem SafeGetPropertyItem(System.Drawing.Image image, int propid)
        {
            try
            {
                return image.GetPropertyItem(propid);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private void cameraImage_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if(ratioUpdateRequired)
            {
                float imgRatio = (float)cameraImage.Source.Height / (float)cameraImage.Width;
                float myRatio = (float)cameraImage.Height / (float)cameraImage.Width;

                if(imgRatio != myRatio)
                {
                    cameraImage.Width = cameraImage.Height * imgRatio;
                }
            }

            ratioUpdateRequired = false;

        }
    }
}
