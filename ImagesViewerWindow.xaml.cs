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
using System.Windows.Shapes;

namespace ComputerGraphics_2_3
{
    /// <summary>
    /// Логика взаимодействия для ImagesViewerWindow.xaml
    /// </summary>
    public partial class ImagesViewerWindow : Window
    {
        public ImagesViewerWindow(BitmapImage[] images, string[] titles = null)
        {
            InitializeComponent();
            image_viewer.setup(images, titles);
        }
    }
}
