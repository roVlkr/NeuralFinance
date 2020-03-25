using System;
using System.Windows.Input;

namespace NeuralFinance.ViewModel.Commands
{
    public class RelayCommand<E> : ICommand
    {
        private readonly Action<E> executeHandler;
        private readonly Predicate<E> canExecuteHandler;

        public RelayCommand(Action<E> executeHandler, Predicate<E> canExecuteHandler = null)
        {
            this.executeHandler = executeHandler ?? throw new ArgumentNullException(nameof(executeHandler));
            this.canExecuteHandler = canExecuteHandler ?? (o => true);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteHandler((E)parameter);
        }

        public void Execute(object parameter)
        {
            executeHandler((E)parameter);
        }
    }
}
