using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NeuralFinance.ViewModel.Commands;
using NeuralFinance.ViewModel.TrainingFactories;
using NeuralNetworks;

namespace NeuralFinance.ViewModel
{
    public class NetworkVM : ViewModelBase
    {
        #region Static Members
        static NetworkVM()
        {
            ActivationFunctions = new List<INetActivation>
            {
                NetFunctions.Rectifier,
                NetFunctions.Sigmoid,
                NetFunctions.SELU,
                NetFunctions.Softplus,
                NetFunctions.Identity
            };

            InitializeNetworkCommand = new RoutedUICommand(
                "Netzwerk initialisieren", nameof(InitializeNetworkCommand),
                typeof(NetworkVM));
        }

        public static List<INetActivation> ActivationFunctions { get; }

        public static RoutedUICommand InitializeNetworkCommand { get; }
        #endregion

        private TrainingFactoryArgs optimizer;

        private INetActivation activationFunction;

        public NetworkVM()
        {
            NetStructure = new ObservableCollection<ValueWrapper>
            {
                new ValueWrapper(typeof(int), 10, Constraint.intGreaterZero)
            };

            AddTextBoxCommand = new RelayCommand(AddTextBox);
            RemoveTextBoxCommand = new RelayCommand(RemoveTextBox, o => NetStructure.Count > 1);
        }

        public INetActivation ActivationFunction
        {
            get => activationFunction;
            set
            {
                activationFunction = value;
                OnPropertyChanged();
            }
        }

        public TrainingFactoryArgs Optimizer
        {
            get => optimizer;
            set
            {
                optimizer = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ValueWrapper> NetStructure { get; }

        public ICommand AddTextBoxCommand { get; }

        public ICommand RemoveTextBoxCommand { get; }

        public void InitializeNetwork()
        {
            var intStructure = from wrapper in NetStructure select (int)wrapper.Value;
            var fullStructure = new List<int>(intStructure) { 1 };
            App.Network = new NeuralNetworks.Net(fullStructure, ActivationFunction);

            var trainingFactory = TrainingFactory.FromType(Optimizer.Name);
            App.Training = trainingFactory.GetTraining(Optimizer);
        }

        public void AddTextBox(object parameter)
        {
            int standardValue = 1;

            if (parameter != null)
                standardValue = (int)parameter;

            NetStructure.Add(new ValueWrapper(typeof(int), standardValue, Constraint.intGreaterZero));
        }

        public void RemoveTextBox(object parameter)
        {
            NetStructure.RemoveAt(NetStructure.Count - 1);
        }
    }
}
