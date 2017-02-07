using System.Windows;
using GridStepAlternative.DataService;
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

        private void CalculateButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: Добавьте сюда реализации сервисов данных
            var calc = new Calculation.Calculation(new NodeService(), new СhainService(), new EntityService(),
                new EdgeService());

            foreach (var map in calc.GetMaps())
            {
                
            }
        }
    }

}