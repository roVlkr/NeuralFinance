using System;
using System.Windows.Input;

namespace NeuralFinance.ViewModel.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> executeHandler;
        private readonly Predicate<object> canExecuteHandler;

        public RelayCommand(Action<object> executeHandler, Predicate<object> canExecuteHandler)
        {
            this.executeHandler = executeHandler;
            this.canExecuteHandler = canExecuteHandler;
        }

        public RelayCommand(Action<object> executeHandler)
        {
            this.executeHandler = executeHandler;
            canExecuteHandler = o => true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteHandler.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            executeHandler.Invoke(parameter);
        }
    }
}
