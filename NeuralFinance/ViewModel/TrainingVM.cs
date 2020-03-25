using NeuralFinance.Model;
using NeuralFinance.ViewModel.Commands;
using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NeuralFinance.ViewModel
{
    public class TrainingVM : ViewModelBase
    {
        public static RoutedCommand TrainingCommand { get; }

        static TrainingVM()
        {
            TrainingCommand = new RoutedCommand(nameof(TrainingCommand), typeof(TrainingVM));
        }

        private ValueWrapper epochs;
        private double progress;
        private Task trainingTask;
        private CancellationTokenSource trainingTokenSource;
        private CancellationTokenSource uiTokenSource;

        private event Action TrainingStartEvent;
        private event Action TrainingStopEvent;

        public TrainingVM()
        {
            TrainingAccuracy = new Dictionary<int, double>();
            ValidationAccuracy = new Dictionary<int, double>();
            Epochs = new ValueWrapper(typeof(int), 1000, Constraint.intGreaterZero);
            Progress = 0;

            TrainingStartEvent += () =>
            {
                StartUIUpdateTask();
                OnPropertyChanged(nameof(TrainingRunning));
            };

            TrainingStopEvent += () =>
            {
                uiTokenSource.Cancel();
                OnPropertyChanged(nameof(TrainingRunning));
            }; 
        }

        public Dictionary<int, double> TrainingAccuracy { get; }

        public Dictionary<int, double> ValidationAccuracy { get; }

        public double Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        public bool TrainingRunning
        {
            get
            {
                if (trainingTask != null)
                    return trainingTask.Status == TaskStatus.Running;

                return false;
            }
        }

        public ValueWrapper Epochs
        {
            get => epochs;
            set 
            {
                epochs = value;
                OnPropertyChanged();
            }
        }

        public void ExecuteTrainingCommand()
        {
            if (TrainingRunning)
            {
                trainingTokenSource?.Cancel();
            }
            else
            {
                StartTrainingTask();
            }
        }

        public void StartTrainingTask()
        {
            trainingTokenSource = new CancellationTokenSource();
            TrainingAccuracy.Clear();
            ValidationAccuracy.Clear();

            trainingTask = App.Training.Run((int)Epochs.Value, trainingTokenSource.Token);
            TrainingStartEvent?.Invoke();
            trainingTask.ContinueWith(task => TrainingStopEvent?.Invoke());
        }

        public void StartUIUpdateTask()
        {
            uiTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (TrainingRunning)
                {
                    await Task.Delay(200);
                    Progress = (double)App.Training.CurrentEpoch / App.Training.MaxEpochs;
                    TrainingAccuracy.Add(App.Training.CurrentEpoch, CalculateAccuracy(App.Training.Patterns));
                    OnPropertyChanged(nameof(TrainingAccuracy));  // From a different thread we have to do a workaround

                    ValidationAccuracy.Add(App.Training.CurrentEpoch, CalculateAccuracy(App.Training.ValidationPatterns));
                    OnPropertyChanged(nameof(ValidationAccuracy));
                }
            }, uiTokenSource.Token);
        }

        public double CalculateAccuracy(in List<TrainingPattern> patterns)
        {
            double accuracy = 0;

            foreach (var pattern in patterns)
            {
                var netOutput = App.Network.Feed(pattern.Input);
                var difference = (netOutput - pattern.Output);
                var patternAccuracy = 1 - Math.Min(difference * difference, 1);

                accuracy += patternAccuracy / patterns.Count;
            }

            return accuracy;
        }
    }
}
