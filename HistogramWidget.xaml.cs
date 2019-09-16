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
using OxyPlot;

namespace ComputerGraphics_2_3
{
    /// <summary>
    /// Логика взаимодействия для HistogramWidget.xaml
    /// </summary>
    public partial class HistogramWidget : UserControl
    {
        private int[] histogram_data;
        public List<OxyPlot.Series.ColumnItem> points;
        private string title;
        public HistogramWidget()
        {
            InitializeComponent();
        }

        public void setup(int[] histogram_data, string title = null)
        {
            this.histogram_data = histogram_data;
            this.title = title;
            this.points = histogram_data.Select((x, i) => new OxyPlot.Series.ColumnItem(x, i)).ToList();

            series.ItemsSource = points;
        }
    }
}
