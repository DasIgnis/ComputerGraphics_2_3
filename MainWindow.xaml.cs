using Microsoft.Win32;
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
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Drawing.Imaging;

namespace ComputerGraphics_2_3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int hueOffset = 0;
        private int satOffset = 0;
        private int valOffset = 0;

        private BitmapImage img;
        private Bitmap processedImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void uploadImg(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                img = new BitmapImage(new Uri(op.FileName));
                imgBefore.Source = img;
            }
        }

        private void convertImg(object sender, RoutedEventArgs e)
        {
            processedImage = BitmapImage2Bitmap(img);
            for (int i = 0; i < processedImage.Width; i++)
            {
                for (int j = 0; j < processedImage.Height; j++)
                {
                    System.Drawing.Color pixelColor = processedImage.GetPixel(i, j);
                    double R = pixelColor.R / 255.0;
                    double G = pixelColor.G / 255.0;
                    double B = pixelColor.B / 255.0;
                    double MAX = Math.Max(Math.Max(R, G), B);
                    double MIN = Math.Min(Math.Min(R, G), B);
                    int H, S, V;

                    if (MAX == MIN)
                    {
                        H = 0;
                    }
                    else if (MAX == R && G >= B)
                    {
                        H = (int)(60 * (G - B) / (MAX - MIN)) + 0;
                    }
                    else if (MAX == R && G < B)
                    {
                        H = (int)(60 * (G - B) / (MAX - MIN) + 360);
                    }
                    else if (MAX == G)
                    {
                        H = (int)((B - R) / (MAX - MIN) + 120);
                    }
                    else
                    {
                        H = (int)((R - G) / (MAX - MIN) + 240);
                    }
                    S = (int)((MAX == 0 ? 1 : (1 - MIN / MAX)) * 100);
                    V = (int)(MAX * 100);

                    H = (hueOffset + H < 0 ? 0 : hueOffset + H) % 361;
                    S = satOffset + S < 0 ? 0 : (satOffset + S > 100 ? 100 : satOffset + S);
                    V = valOffset + V < 0 ? 0 : (valOffset + V > 100 ? 100 : valOffset + V);

                    double Vmin = (100 - S) * V / 100;
                    double a = (V - Vmin) * (H % 60) / 60;
                    double Vinc = Vmin + a;
                    double Vdec = V - a;

                    int Hi = (int)Math.Floor(H / 60.0) % 6;
                    int Rnew, Gnew, Bnew;

                    switch (Hi)
                    {
                        case 0:
                            Rnew = procToValue(V);
                            Gnew = procToValue(Vinc);
                            Bnew = procToValue(Vmin);
                            break;
                        case 1:
                            Rnew = procToValue(Vdec);
                            Gnew = procToValue(V);
                            Bnew = procToValue(Vmin);
                            break;
                        case 2:
                            Rnew = procToValue(Vmin);
                            Gnew = procToValue(V);
                            Bnew = procToValue(Vinc);
                            break;
                        case 3:
                            Rnew = procToValue(Vmin);
                            Gnew = procToValue(Vdec);
                            Bnew = procToValue(V);
                            break;
                        case 4:
                            Rnew = procToValue(Vinc);
                            Gnew = procToValue(Vmin);
                            Bnew = procToValue(V);
                            break;
                        case 5:
                            Rnew = procToValue(V);
                            Gnew = procToValue(Vmin);
                            Bnew = procToValue(Vdec);
                            break;
                        default:
                            Rnew = 0;
                            Gnew = 0;
                            Bnew = 0;
                            break;
                    }

                    processedImage.SetPixel(i, j, System.Drawing.Color.FromArgb(255, Rnew, Gnew, Bnew));
                }
            }
            imgAfter.Source = ToBitmapImage(processedImage);
        }

        private int procToValue(double val)
        {
            return (int)(val * 255 / 100);
        }

        private void SliderHue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            hueOffset = (int)e.NewValue - 180;
        }

        private void SliderSat_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            satOffset = (int)e.NewValue - 50;
        }

        private void SliderVal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            valOffset = (int)e.NewValue - 50;
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private BitmapImage ToBitmapImage(Bitmap bmp)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void saveToFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document";
            dlg.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            if (dlg.ShowDialog() == true)
            {
                var encoder = new JpegBitmapEncoder(); // Or PngBitmapEncoder, or whichever encoder you want
                encoder.Frames.Add(BitmapFrame.Create(ToBitmapImage(processedImage)));
                using (var stream = dlg.OpenFile())
                {
                    encoder.Save(stream);
                }
            }
        }

                
    }
}
