using NeuralFinance.ViewModel.Commands;
using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralFinance.ViewModel
{
    public class TrainingVM : ViewModelBase
    {
        private string epochs;
        private bool trainingRunning;

        public TrainingVM()
        {
            trainingRunning = false;
            TrainingCommand = new StartTrainingCommand(this);
        }

        public bool TrainingRunning
        {
            get => trainingRunning;
            set
            {
                trainingRunning = value;
                OnPropertyChanged();
            }
        }

        public StartTrainingCommand TrainingCommand { get; set; }

        public string Epochs
        {
            get => epochs;
            set 
            {
                epochs = value;
                OnPropertyChanged();
            }
        }
    }
}
