using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Avalon.Windows.Dialogs;
using GridStepAlternative.DataService;
using LiveCharts;
using LiveCharts.Wpf;
using NLog;
using PolylinesComparer;
using PolylinesComparer.Model;

namespace GridStepAlternative
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public SeriesCollection SeriesCollection { get; set; }

        public List<string> Labels { get; set; }

        /// <summary>
        /// Начальное значение
        /// </summary>
        public int FromValue
        {
            get
            {
                int value;

                if (ParamsTab.SelectedIndex == 0)
                {
                    if (int.TryParse(FromS.Text, out value))
                        return value;
                    FromS.Text = "0";
                    return 0;
                }

                if (int.TryParse(FromC.Text, out value) && value <= 100)
                    return value;
                FromC.Text = "0";
                return 0;
            }
        }

        /// <summary>
        /// Конечное значение
        /// </summary>
        public int ToValue
        {
            get
            {
                int value;

                if (ParamsTab.SelectedIndex == 0)
                {
                    if (int.TryParse(ToS.Text, out value))
                        return value;
                    ToS.Text = "100";
                    return 100;
                }

                
                if (int.TryParse(ToC.Text, out value) && value <= 100)
                    return value;
                ToC.Text = "100";
                return 100;
            }
        }

        /// <summary>
        /// Шаг изменения
        /// </summary>
        public int StepValue
        {
            get
            {
                int value;

                if (ParamsTab.SelectedIndex == 0)
                {
                    if (int.TryParse(StepS.Text, out value))
                        return value;
                    StepS.Text = "1";
                    return 1;
                }

                if (int.TryParse(StepC.Text, out value))
                    return value;
                StepC.Text = "1";
                return 1;
            }
        }

        /// <summary>
        /// Не изменяемый параметр
        /// </summary>
        public int ConstValue
        {
            get
            {
                int value;

                if (ParamsTab.SelectedIndex == 0)
                {
                    if (int.TryParse(ConstParamS.Text, out value))
                        return value ;
                    ConstParamS.Text = "80";
                    return 80;
                }

                if (int.TryParse(ConstParamC.Text, out value) && value <= 100)
                    return value;
                ConstParamC.Text = "50";
                return 50;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Сущность",
                    Values = new ChartValues<double> {4, 6, 5, 2, 4},
                    LineSmoothness = 0
                }
            };

            Labels = new List<string> {"1", "5", "7", "9", "12"};
        }

        private void CalculateButtonClick(object sender, RoutedEventArgs e)
        {
            Calculate.IsEnabled = false;

            SeriesCollection.Clear();
            Labels.Clear();
            for (int grid = FromValue; grid <= ToValue; grid += StepValue)
                Labels.Add(grid.ToString());

            var worker = new BackgroundWorker();

            worker.DoWork += (s, ea) =>
            {
                var log = LogManager.GetCurrentClassLogger();

                int fromValue = 0, toValue = 0, stepValue = 0;
                string folder = "";
                bool allCompare = true, isStepDifferent = true;
                int constValue = 80;
                Dispatcher.Invoke(new Action(() =>
                {
                    fromValue = FromValue;
                    toValue = ToValue;
                    stepValue = StepValue;
                    constValue = ConstValue;

                    folder = FolderPath.Text;

                    allCompare = AllCompare.IsChecked.Value;
                    isStepDifferent = ParamsTab.SelectedIndex == 0;              
                }));

                // TODO: Можно добавить сюда свои реализации сервисов данных: INodeService, IСhainService, IEntityService, IEdgeService
                var calc = new Calculation.Calculation(new NodeService(folder), new СhainService(folder),
                    new EntityService(folder),
                    new EdgeService(folder));

                var numberService = new IndexesNumberService();

                // Перебрать все сущности
                foreach (var map in calc.GetMaps())
                {
                    log.Trace($"Сущность {map.Entity.Id}");

                    LineSeries line = null;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        line = new LineSeries
                        {
                            Title = map.Entity.Name,
                            Values = new ChartValues<int>(),
                            LineSmoothness = 0
                        };
                        SeriesCollection.Add(line);
                    }));

                    var count = map.Edges.GetLength(0);
                    if (allCompare)
                    {
                        // Сравнивать все рёбра со всеми

                        // Получить список всех рёбер
                        var collection = new List<List<Coordinate>>();
                        // Обход всех рёбер сущности
                        for (int i = 0; i < count; i++)
                        for (int j = 0; j < count; j++)
                            if (map.Edges[i, j] != null)
                                collection.AddRange(map.Edges[i, j].Select(edge => edge.Coordinates).ToList());

                        // Изменение размеров ячеек индексной сетки
                        for (int value = fromValue;
                            value <= toValue || !isStepDifferent && value <= 100;
                            value += stepValue)
                        {
                            var grid = isStepDifferent ? value : constValue;
                            var compliance = isStepDifferent ? (double)constValue / 100 : (double)value / 100;

                            var center = new Coordinate(map.Entity.Center.Lon - grid * 0.5,
                                map.Entity.Center.Lat - grid * 0.5);

                            // Определить количество уникальных рёбер
                            var result = value == 0
                                ? collection.Count
                                : numberService.DifferentIndexesNumber2D(collection, grid, compliance,
                                    center);
                            Dispatcher.Invoke(new Action(() =>
                            {
                                line.Values.Add(result);
                            }));

                            log.Trace($"{value} - результат {result}");
                        }
                    }
                    else
                    {
                        // Сравнивать только те рёбра, которые связывают одинаковые вершины

                        for (int value = fromValue;
                            value <= toValue || !isStepDifferent && value <= 100;
                            value += stepValue)
                        {
                            var grid = isStepDifferent ? value : constValue;
                            var compliance = isStepDifferent ? (double)constValue / 100 : (double)value / 100;

                            var result = 0;
                            for (int i = 0; i < count; i++)
                            for (int j = 0; j < count; j++)
                                if (map.Edges[i, j] != null)
                                {
                                    if (map.Edges[i, j].Count == 1)
                                        result++;
                                    else if (value == 0)
                                        result += map.Edges[i, j].Count;
                                    else
                                    {
                                        var collection = map.Edges[i, j].Select(edges => edges.Coordinates).ToList();
                                        var minX = collection.SelectMany(c => c).Min(c => c.Lon) - grid * 0.5;
                                        var minY = collection.SelectMany(c => c).Min(c => c.Lat) - grid * 0.5;
                                        var center = new Coordinate(minX, minY);

                                        result += numberService.DifferentIndexesNumber2D(collection, grid, compliance,
                                            center);
                                    }
                                }

                            Dispatcher.Invoke(new Action(() =>
                            {
                                line.Values.Add(result);
                            }));

                            log.Trace($"{value} - результат {result}");
                        }
                    }
                }
            };

            worker.RunWorkerCompleted += (o, args) =>
            {
                Calculate.IsEnabled = true;

                if (args.Error != null)
                    MessageBox.Show(args.Error.Message, "Ошибка");
            };

            worker.RunWorkerAsync();
        }

        #region Обеспечивает вовод только целых положительных чисел

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            var regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }

        #endregion

        private void FolderDialogClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == true)
                FolderPath.Text = dialog.SelectedPath;
        }
    }

}