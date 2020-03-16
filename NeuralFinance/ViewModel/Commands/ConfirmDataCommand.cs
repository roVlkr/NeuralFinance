using NeuralFinance.Model;
using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace NeuralFinance.ViewModel.Commands
{
    public class ConfirmDataCommand : ICommand
    {
        private readonly DataVM dataVM;

        public ConfirmDataCommand(DataVM vm)
        {
            dataVM = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return App.Training != null;
        }

        public void Execute(object parameter)
        {

        }
    }
}
