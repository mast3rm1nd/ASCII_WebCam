using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



//using AForge;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

using System.ComponentModel;
using System.IO;


//using System.Windows.Forms;
//using 

namespace ASCII_WebCam
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FillDevicesComboBox();
        }

        static Bitmap bitmap;
        public static BitmapSource ToWpfBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        bool rotate = false;
        BitmapSource img;
        static int processEachNFrame = 12;
        static int currFrame = 0;
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            currFrame++;            
            if (currFrame != processEachNFrame) return;
            currFrame = 1;
            
            // get new frame
            //if(IsMirrorred_CheckBox.IsChecked == false)
            //    bitmap = (Bitmap)eventArgs.Frame.Clone();
            //else
            //    bitmap = (Bitmap)eventArgs.Frame.Clone();
            //bitmap = (Bitmap)eventArgs.Frame.Clone();
            //if (bitmap != null)
            //    bitmap.Dispose();

            bitmap = new Bitmap(eventArgs.Frame);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsMirrorred_CheckBox.IsChecked == true)
                    rotate = true;
                else
                    rotate = false;
            }));

            if (rotate)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);

            //bitmap = AForge.Imaging.Image.Clone(eventArgs.Frame);

            //resizing to width of 160 max
            var targetWidth = 160;
            var targetHeight = 120;
            var scalingFactor = Math.Min(targetWidth / (double)bitmap.Width, targetHeight / (double)bitmap.Height);
            if (scalingFactor < 1)
                scalingFactor = 1 / scalingFactor;

            Bitmap resized = new Bitmap(bitmap, new System.Drawing.Size((int)(bitmap.Width / scalingFactor), (int)(bitmap.Height / scalingFactor)));

            var ascii = ASCII_From_Image.Get(resized);

            //img = ToWpfBitmap(bitmap);

            //bitmap.Save("image.jpg", ImageFormat.Jpeg);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ASCII_textBox.Text = ascii;
                //CapturedImage_Image.Source = BitmapToBitmapSource.ToBitmapSource(bitmap);
                //CapturedImage_Image.Source = ToWpfBitmap();
                //CapturedImage_Image.Source = img;

                //ImageSourceConverter c = new ImageSourceConverter();

                //var image = (ImageSource)c.ConvertFrom(bitmap);

                //bitmap.Dispose();
                //CapturedImage_Image.Resources.Clear();
                //CapturedImage_Image.SetValue(System.Windows.Controls.Image.SourceProperty, null);

                //var image = BitmapToImagesource(bitmap);


                //IntPtr hBitmap = bitmap.GetHbitmap(); 
                //try 
                //{
                //    var image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                //    CapturedImage_Image.Source = image;
                //}
                //finally 
                //{
                //    DeleteObject(hBitmap);
                //    bitmap.Dispose();
                //}

            }));
            //videoSource.SignalToStop();
            //videoSource.Stop();
            //videoSource.WaitForStop();
            //videoSource.NewFrame += null;
            //videoSource = null;
            //Environment.Exit(0);
            // process the frame
        }



        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);


        private void Get_WebCamImage_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource.IsRunning)
            {
                StartCapture_Button.Content = "Начать получать изображение";

                videoSource.Stop();

                //SaveImage_Button.IsEnabled = false;

                return;
            }

            videoSource.VideoResolution = videoSource.VideoCapabilities[Resolutions_ComboBox.SelectedIndex];

            var width = videoSource.VideoCapabilities[Resolutions_ComboBox.SelectedIndex].FrameSize.Width;
            var height = videoSource.VideoCapabilities[Resolutions_ComboBox.SelectedIndex].FrameSize.Height;


            //CapturedImage_Image.Width = width;
            //CapturedImage_Image.Height = height;

            //this.Width = width + 36;
            //this.Height = height + 190;

            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

            videoSource.Start();

            //SaveImage_Button.IsEnabled = true;

            StartCapture_Button.Content = "Остановить захват изображения";
            //CapturedImage_Image.IsEnabled = false;
        }


        void FillDevicesComboBox()
        {
            foreach (FilterInfo vd in videoDevices)
            {
                Devices_ComboBox.Items.Add(vd.Name);
            }

            if (Devices_ComboBox.Items.Count != 0)
            {
                Devices_ComboBox.SelectedIndex = 0;

                FillResolutionsComboBox();
            }
        }


        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

        VideoCaptureDevice videoSource;
        void FillResolutionsComboBox()
        {
            videoSource = new VideoCaptureDevice(
                            videoDevices[Devices_ComboBox.SelectedIndex].MonikerString);


            Resolutions_ComboBox.Items.Clear();

            if (videoSource.VideoCapabilities.Length != 0)
                foreach (var resolution in videoSource.VideoCapabilities)
                {
                    Resolutions_ComboBox.Items.Add(string.Format("{0}x{1}", resolution.FrameSize.Width, resolution.FrameSize.Height));
                }
            else
            {
                Resolutions_ComboBox.Items.Add("Захват изображения не поддерживается");

                Resolutions_ComboBox.SelectedIndex = 0;

                StartCapture_Button.IsEnabled = false;

                return;
            }

            Resolutions_ComboBox.SelectedIndex = 0;

            StartCapture_Button.IsEnabled = true;
        }


        int imageIndex = 0;
        private void SaveImage_Button_Click(object sender, RoutedEventArgs e)
        {
            var fileName = String.Format("Фото {0}.jpg", ++imageIndex);


            try
            {
                var cloned = (Bitmap)bitmap.Clone();
                cloned.Save(fileName, ImageFormat.Jpeg);
            }
            catch
            {
                return;
            }

            MessageBox.Show(String.Format("Сохранено в \"{0}\"", fileName));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
                videoSource.SignalToStop();
        }

        private void Devices_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillResolutionsComboBox();
        }
    }
}
