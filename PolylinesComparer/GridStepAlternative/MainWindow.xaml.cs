using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;

namespace GridStepAlternative
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection SeriesCollection { get; set; }

        public string[] Labels { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Город",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 },
                    LineSmoothness = 0
                }
            };

            Labels = new[] { "1", "5", "7", "9", "12" };
        }
    }
}
