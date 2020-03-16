using Microsoft.Win32;
using NeuralFinance.Model;
using NeuralFinance.ViewModel.Commands;
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
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
    }

    public class DataVM : ViewModelBase
    {
        private ChartTable chartTable;
        private Chart chart;
        private string selectedColumn;
        private ObservableCollection<TimeRange> trainingRanges;
        private bool dataLoaded;

        public DataVM()
        {
            TrainingRanges = new ObservableCollection<TimeRange>();

            LoadDataFileCommand = new RelayCommand(parameter => LoadData());
            AddTrainingRangeCommand = new RelayCommand(parameter => AddTrainingRange(),
                parameter => DataLoaded);
            RemoveTrainingRangeCommand = new RelayCommand(paramter => RemoveTrainingRange(),
                parameter => TrainingRanges.Count > 1 && DataLoaded);
            ConfirmDataCommand = new ConfirmDataCommand(this);
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

        public ICommand ConfirmDataCommand { get; }

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

                DataLoaded = true;
            }
        }

        public void AddTrainingRange()
        {
            TrainingRanges.Add(new TimeRange
            {
                StartTime = ChartTable.Data.Keys.Min(),
                StopTime = ChartTable.Data.Keys.Max()
            });
        }

        public void RemoveTrainingRange()
        {
            TrainingRanges.RemoveAt(TrainingRanges.Count - 1);
        }
    }
}
