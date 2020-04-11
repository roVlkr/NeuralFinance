using NeuralFinance.Model;
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

        // ViewModel dependencies
        private readonly DataVM dataVM;

        // Property variables
        private Tuple<int, double> currentTrainingError;
        private Tuple<int, double> currentValidationError;
        private double minValidationError;
        private Network minNetworkImage;
        private int epochs;
        private double progress;
        private Estimate currentEstimate;

        // Training variables
        private Task trainingTask;
        private CancellationTokenSource trainingTokenSource;
        private CancellationTokenSource uiTokenSource;

        // Events
        private event Action TrainingStartEvent;
        private event Action TrainingStopEvent;

        public TrainingVM(DataVM dataVM)
        {
            CurrentTrainingError = new Tuple<int, double>(0, 0);
            CurrentValidationError = new Tuple<int, double>(0, 0);
            minValidationError = double.PositiveInfinity;
            Epochs = 1000;
            Progress = 0;
            CurrentEstimate = new Estimate(0, 0);
            this.dataVM = dataVM;

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

        public Tuple<int, double> CurrentTrainingError
        {
            get => currentTrainingError;
            set
            {
                currentTrainingError = value;
                OnPropertyChanged();
            }
        }

        public Tuple<int, double> CurrentValidationError
        {
            get => currentValidationError;
            set
            {
                currentValidationError = value;
                OnPropertyChanged();
            }
        }

        public double MinValidationError
        {
            get => minValidationError;
            set
            {
                minValidationError = value;
                OnPropertyChanged();
            }
        }

        public Network MinNetworkImage
        {
            get => minNetworkImage;
            set
            {
                minNetworkImage = value;
                OnPropertyChanged();
            }
        }

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

        public int Epochs
        {
            get => epochs;
            set 
            {
                epochs = value;
                OnPropertyChanged();
                ObserveConstraint(value > 0, "errorMessageGreaterZero");
            }
        }

        public Estimate CurrentEstimate
        {
            get => currentEstimate;
            set
            {
                currentEstimate = value;
                OnPropertyChanged();
            }
        }

        public void TrainingCommand_Execute()
        {
            if (TrainingRunning)
            {
                StopTrainingTask();
            }
            else
            {
                StartTrainingTask();
            }
        }

        public void StartTrainingTask()
        {
            trainingTokenSource = new CancellationTokenSource();

            // Reset training progress
            MinValidationError = double.PositiveInfinity;
            App.NeuralSystem.ResetTrainingProgress();

            trainingTask = App.Training.Run(Epochs, trainingTokenSource.Token);
            TrainingStartEvent?.Invoke();
            trainingTask.ContinueWith(task => TrainingStopEvent?.Invoke());
        }

        public void StopTrainingTask()
        {
            trainingTokenSource?.Cancel();
        }

        public void StartUIUpdateTask()
        {
            uiTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (TrainingRunning)
                {
                    await Task.Delay(50);
                    Progress = (double)App.Training.CurrentEpoch / App.Training.MaxEpochs;
                    CurrentTrainingError = new Tuple<int, double>(App.Training.CurrentEpoch,
                        CalculateError(App.NeuralSystem.TrainingPatterns));

                    CurrentValidationError = new Tuple<int, double>(App.Training.CurrentEpoch,
                        CalculateError(App.NeuralSystem.ValidationPatterns));

                    if (MinValidationError > CurrentValidationError.Item2)
                    {
                        MinNetworkImage = new Network(App.Network);
                        CurrentEstimate = CalculateCurrentEstimate();
                        MinValidationError = CurrentValidationError.Item2;
                    }
                }
            }, uiTokenSource.Token);
        }

        public double CalculateError(in List<TrainingPattern> patterns)
        {
            double error = 0;

            foreach (var pattern in patterns)
            {
                var netOutput = App.Network.Feed(pattern.Input);
                var difference = (netOutput - pattern.Output);
                var patternError = difference * difference;

                error += patternError / patterns.Count;
            }

            return error;
        }

        public Estimate CalculateCurrentEstimate()
        {
            var chart = dataVM.Chart;

            // Generate input vector
            int inputLength = App.NeuralSystem.Network.Structure[0];
            var lastData = chart.LogData.Values.ToList()
                .GetRange(chart.LogData.Count - inputLength, inputLength);
            var inputVector = new VectorMath.Vector(lastData.ToArray())
                .Apply(x => (x - chart.LogSampleMean) / Math.Sqrt(chart.LogSampleVariance));

            // Calculate output
            double output = App.Network.Feed(inputVector)[0];

            // "Denormalize" output: Invert the normalization function
            output = output * Math.Sqrt(chart.LogSampleVariance) + chart.LogSampleMean;

            // "Delogalize" output: Get percentage
            output = Math.Exp(output);

            return new Estimate(output - 1, output * chart.Data.Values.Last());
        }
    }
}
