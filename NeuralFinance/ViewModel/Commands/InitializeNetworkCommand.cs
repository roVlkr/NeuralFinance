using NeuralFinance.ViewModel.TrainingFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeuralFinance.ViewModel.Commands
{
    public class InitializeNetworkCommand : ICommand
    {
        private readonly NetworkVM networkVM;

        public InitializeNetworkCommand(NetworkVM vm)
        {
            networkVM = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            // Structure with output layer (of 1 neuron)
            var intStructure = from wrapper in networkVM.NetStructure select (int)wrapper.Value;
            var fullStructure = new List<int>(intStructure) { 1 };
            App.Network = new NeuralNetworks.Net(fullStructure, networkVM.ActivationFunction);

            var trainingFactory = TrainingFactory.FromType(networkVM.Optimizer.Name);
            App.Training = trainingFactory.GetTraining(networkVM.Optimizer);
        }
    }
}
