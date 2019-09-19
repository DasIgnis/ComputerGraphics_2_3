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
using System.Drawing.Imaging;

namespace ComputerGraphics_2_3
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        private BitmapImage[] images;
        private string[] titles;
        private int index;

        public ImageViewer()
        {
            InitializeComponent();
        }

        public void setup(BitmapImage[] images, string[] titles = null)
        {
            this.images = images;
            this.titles = titles;
            this.index = 0;

            move_image(0);
        }

        private void previous_button(object sender, RoutedEventArgs e)
        {
            move_image(-1);
        }

        private void next_button(object sender, RoutedEventArgs e)
        {
            move_image(1);
        }

        private void move_image(int step)
        {
            index += step;
            while(index < 0) { index += images.Length; }
            while(images.Length <= index) { index -= images.Length; }

            this.current_image.Source = images[index];
            this.image_title.Text = titles[index];
            this.text_information.Text = String.Format("{0} of {1}", index + 1, images.Length);
        }
    }
}
