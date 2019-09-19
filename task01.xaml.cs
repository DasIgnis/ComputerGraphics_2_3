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
    /// Логика взаимодействия для task01.xaml
    /// </summary>
    public partial class task01 : Window
    {
        private Bitmap original_bitmap;
        public task01()
        {
            InitializeComponent();
        }

        private void load_image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true) {
                original_bitmap = new Bitmap(op.FileName);
                original_image.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void process_image(object sender, RoutedEventArgs e)
        {
            if(original_bitmap == null) { return; }

            Bitmap theoretic_gray_shades  = new Bitmap(original_bitmap.Width, original_bitmap.Height);
            Bitmap average_gray_shades = new Bitmap(original_bitmap.Width, original_bitmap.Height);
            Bitmap difference_gray_shades = new Bitmap(original_bitmap.Width, original_bitmap.Height);

            int[] theoretic_shades_hist_data = new int[256];
            theoretic_shades_hist_data = theoretic_shades_hist_data.Select(n => 0).ToArray();
            int[] average_shades_hist_data = new int[256];
            average_shades_hist_data = average_shades_hist_data.Select(n => 0).ToArray();

            for (int i = 0; i < original_bitmap.Width; i++)
            {
                for (int j = 0; j < original_bitmap.Height; j++)
                {
                    System.Drawing.Color pixel = original_bitmap.GetPixel(i, j);

                    byte theoretic_sum = (byte)(0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B);
                    theoretic_shades_hist_data[theoretic_sum] += 1;
                    System.Drawing.Color theoretic_transform = System.Drawing.Color.FromArgb(pixel.A, theoretic_sum, theoretic_sum, theoretic_sum);
                    theoretic_gray_shades.SetPixel(i, j, theoretic_transform);

                    byte average_sum = (byte)(((int)pixel.R + (int)pixel.G + (int)pixel.B)/3);
                    average_shades_hist_data[average_sum] += 1;
                    System.Drawing.Color average_transform = System.Drawing.Color.FromArgb(pixel.A, average_sum, average_sum, average_sum);
                    average_gray_shades.SetPixel(i, j, average_transform);

                    byte difference_sum = (byte)(theoretic_sum - average_sum);
                    System.Drawing.Color difference_transform = System.Drawing.Color.FromArgb(pixel.A, difference_sum, difference_sum, difference_sum);
                    difference_gray_shades.SetPixel(i, j, difference_transform);
                }
            }
            BitmapImage[] images = { ToBitmapImage(theoretic_gray_shades), ToBitmapImage(average_gray_shades), ToBitmapImage(difference_gray_shades) };
            string[] titles = { "Theoretic Shaded Image", "Average Shaded Image", "Difference between Theoretical and Average" };
            ImagesViewerWindow window = new ImagesViewerWindow(images, titles);
            window.Title = "Gray Shaded Images";
            window.Show();
            HistogramWindow w1 = new HistogramWindow(theoretic_shades_hist_data, "Theoretical Shades Histogram");
            w1.Show();
            HistogramWindow w2 = new HistogramWindow(average_shades_hist_data, "Average Shades Histogram");
            w2.Show();
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
    }
}
