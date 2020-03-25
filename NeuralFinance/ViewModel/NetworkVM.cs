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
        private bool isUninitialized;

        public NetworkVM()
        {
            Optimizer = TrainingFactoryArgs.Templates[TrainingTypes.Adam];
            ActivationFunction = ActivationFunctions[0];

            NetStructure = new ObservableCollection<ValueWrapper>
            {
                new ValueWrapper(typeof(int), 10, Constraint.intGreaterZero)
            };

            AddTextBoxCommand = new RelayCommand<object>(AddTextBox);
            RemoveTextBoxCommand = new RelayCommand<object>(RemoveTextBox, parameter => NetStructure.Count > 1);
            IsUninitialized = true;
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

        public bool IsUninitialized
        {
            get => isUninitialized;
            set
            {
                isUninitialized = value;
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
            App.NeuralSystem.Network = new Net(fullStructure, ActivationFunction);

            var trainingFactory = TrainingFactory.FromType(Optimizer.Name);
            App.NeuralSystem.Training = trainingFactory.GetTraining(Optimizer);

            IsUninitialized = false;
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
