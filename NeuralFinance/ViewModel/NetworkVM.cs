using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using NeuralFinance.ViewModel.TrainingFactories;
using NeuralNetworks;

namespace NeuralFinance.ViewModel
{
    public class NetworkVM : ViewModelBase
    {
        #region Static Members
        static NetworkVM()
        {
            InitializeNetworkCommand = new RoutedUICommand("Netzwerk initialisieren",
                nameof(InitializeNetworkCommand), typeof(NetworkVM));
        }

        public static RoutedUICommand InitializeNetworkCommand { get; }
        #endregion

        public NetworkVM()
        {
            AddTextBoxCommand = new RelayCommand<object>(AddTextBox);
            RemoveTextBoxCommand = new RelayCommand<object>(RemoveTextBox, parameter => NetStructure.Count > 1);

            NetStructure = new ObservableCollection<ValueWrapper<int>>
            {
                new ValueWrapper<int>(10, Constraint<int>.greaterZero)
            };

            TrainingFactories = new List<TrainingFactory>
            {
                new AdamTrainingFactory(),
                new RMSPropTrainingFactory(),
                new RPropTrainingFactory(),
                new StandardTrainingFactory()
            };

            SelectedTrainingFactory = TrainingFactories[0];

            ActivationFunctions = new List<INetActivation>
            {
                NetFunctions.Rectifier,
                NetFunctions.Sigmoid,
                NetFunctions.SELU,
                NetFunctions.Softplus,
                NetFunctions.Identity
            };

            SelectedActivationFunction = ActivationFunctions[0];
        }

        public List<TrainingFactory> TrainingFactories { get; }

        public TrainingFactory SelectedTrainingFactory { get; set; }

        public List<INetActivation> ActivationFunctions { get; }

        public INetActivation SelectedActivationFunction { get; set; }

        public ObservableCollection<ValueWrapper<int>> NetStructure { get; }

        public ICommand AddTextBoxCommand { get; }

        public ICommand RemoveTextBoxCommand { get; }

        public void InitializeNetwork()
        {
            var fullStructure = new List<int>(NetStructure.Select(w => w.Value)) { 1 };

            List<TrainingPattern> trainingPatterns = null;
            List<TrainingPattern> validationPatterns = null;
            bool oldPatternsInvalid = true;

            if (App.NeuralSystem.SystemState == SystemState.DataInitialized)
            {
                oldPatternsInvalid = fullStructure[0] != App.Network.Structure[0];

                if (!oldPatternsInvalid)
                {
                    trainingPatterns = new List<TrainingPattern>(App.NeuralSystem.TrainingPatterns);
                    validationPatterns = new List<TrainingPattern>(App.NeuralSystem.ValidationPatterns);
                }                
            }

            App.NeuralSystem.Network = new Network(fullStructure, SelectedActivationFunction);
            App.NeuralSystem.Training = SelectedTrainingFactory.CreateTraining();

            if (!oldPatternsInvalid)
            {
                App.NeuralSystem.TrainingPatterns = trainingPatterns;
                App.NeuralSystem.ValidationPatterns = validationPatterns;
            }
        }

        public void AddTextBox(object parameter)
        {
            int standardValue = 1;

            if (NetStructure.Count > 0)
                standardValue = NetStructure[^1].Value;

            if (parameter != null)
                standardValue = (int)parameter;

            NetStructure.Add(new ValueWrapper<int>(standardValue, Constraint<int>.greaterZero));
        }

        public void RemoveTextBox(object parameter)
        {
            NetStructure.RemoveAt(NetStructure.Count - 1);
        }
    }
}
