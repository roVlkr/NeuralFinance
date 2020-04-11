using Microsoft.Win32;
using NeuralFinance.Model;
using NeuralFinance.ViewModel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NeuralFinance.ViewModel
{
    public class TimeRange
    {
        public TimeRange(DateTime startTime, DateTime stopTime)
        {
            StartTime = startTime;
            StopTime = stopTime;
        }

        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
    }

    public class DataVM : ViewModelBase
    {
        public static RoutedUICommand ConfirmDataCommand { get; }

        static DataVM()
        {
            ConfirmDataCommand = new RoutedUICommand("Daten verwenden",
                nameof(ConfirmDataCommand), typeof(DataVM));
        }

        private ChartTable chartTable;
        private Chart chart;
        private string selectedColumn;
        private ObservableCollection<TimeRange> trainingRanges;
        private ObservableCollection<TimeRange> validationRanges;
        private bool dataLoaded;
        private int estimateLength;
        private bool chooseValidationRanges;

        public DataVM()
        {
            TrainingRanges = new ObservableCollection<TimeRange>();
            ValidationRanges = new ObservableCollection<TimeRange>();

            LoadDataFileCommand = new RelayCommand<object>(parameter => LoadData());

            AddTrainingRangeCommand = new RelayCommand<object>(parameter => AddTrainingRange(),
                parameter => DataLoaded);
            RemoveTrainingRangeCommand = new RelayCommand<object>(paramter => RemoveTrainingRange(),
                parameter => TrainingRanges.Count > 1 && DataLoaded);

            AddValidationRangeCommand = new RelayCommand<object>(parameter => AddValidationRange(),
                parameter => DataLoaded);
            RemoveValidationRangeCommand = new RelayCommand<object>(paramter => RemoveValidationRange(),
                parameter => TrainingRanges.Count > 1 && ChooseValidationRanges && DataLoaded);

            EstimateLength = 5;
        }

        public string SelectedColumn
        {
            get => selectedColumn;
            set
            {
                selectedColumn = value;

                if (ChartTable != null)
                {
                    Chart = new Chart
                    {
                        Data = (from entry in chartTable.Data
                                select entry)
                               .ToDictionary(entry => entry.Key, entry => entry.Value[value])
                    };
                }

                OnPropertyChanged();
            }
        }

        public Chart Chart
        {
            get => chart;
            set
            {
                chart = value;
                OnPropertyChanged();
            }
        }

        public ChartTable ChartTable
        {
            get => chartTable;
            set
            {
                chartTable = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TimeRange> TrainingRanges
        {
            get => trainingRanges;
            set
            {
                trainingRanges = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TimeRange> ValidationRanges
        {
            get => validationRanges;
            set
            {
                validationRanges = value;
                OnPropertyChanged();
            }
        }

        public bool ChooseValidationRanges
        {
            get => chooseValidationRanges;
            set
            {
                chooseValidationRanges = value;
                OnPropertyChanged();
            }
        }

        public int EstimateLength
        {
            get { return estimateLength; }
            set
            {
                estimateLength = value;
                OnPropertyChanged();
                ObserveConstraint(value > 0, "errorMessageGreaterZero");
            }
        }


        public bool DataLoaded
        {
            get => dataLoaded;
            set
            {
                dataLoaded = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataFileCommand { get; }

        public ICommand AddTrainingRangeCommand { get; }

        public ICommand RemoveTrainingRangeCommand { get; }

        public ICommand AddValidationRangeCommand { get; }

        public ICommand RemoveValidationRangeCommand { get; }

        public void LoadData()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Comdirect chart tables (*.csv)|*.csv",
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() == true)
            {
                ChartTable chartTable = null;

                // Read data from all selected tables in chartTable
                foreach (string fileName in dialog.FileNames)
                {
                    using var comdirectReader = new ComdirectCSVReader(fileName);

                    if (chartTable == null)
                        chartTable = comdirectReader.ReadTable();
                    else
                        chartTable += comdirectReader.ReadTable();
                }

                ChartTable = chartTable;
                SelectedColumn = ChartTable.ValueColumnNames[0];
                TrainingRanges.Clear();
                AddTrainingRange();
                ValidationRanges.Clear();
                AddValidationRange();

                DataLoaded = true;
            }
        }

        public void AddTrainingRange()
        {
            TrainingRanges.Add(new TimeRange(
                ChartTable.Data.Keys.Min(),
                ChartTable.Data.Keys.Max()));
        }

        public void RemoveTrainingRange()
        {
            TrainingRanges.RemoveAt(TrainingRanges.Count - 1);
        }

        public void AddValidationRange()
        {
            ValidationRanges.Add(new TimeRange(
                ChartTable.Data.Keys.Min(),
                ChartTable.Data.Keys.Max()));
        }

        public void RemoveValidationRange()
        {
            ValidationRanges.RemoveAt(ValidationRanges.Count - 1);
        }

        public void ConfirmData()
        {
            var trainingPatterns = PatternFactory.CreateTrainingPatterns(
                Chart, TrainingRanges,
                App.Network.Structure[0],
                EstimateLength);

            App.NeuralSystem.TrainingPatterns = trainingPatterns;

            IEnumerable<TimeRange> validationRanges =
                ChooseValidationRanges ? ValidationRanges :
                PatternFactory.CreateAlternateTimeRanges(TrainingRanges,
                    new TimeRange(Chart.Data.Keys.Min(), Chart.Data.Keys.Max()));

            var validationPatterns = PatternFactory.CreateTrainingPatterns(
                Chart, validationRanges,
                App.Network.Structure[0],
                EstimateLength);

            App.NeuralSystem.ValidationPatterns = validationPatterns;
        }
    }
}
