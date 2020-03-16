using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeuralFinance.ViewModel.Commands
{
    public class StartTrainingCommand : ICommand
    {
        private readonly TrainingVM trainingVM;
        private CancellationTokenSource tokenSource;

        public StartTrainingCommand(TrainingVM vm)
        {
            trainingVM = vm;
        }

        public event EventHandler CanExecuteChanged
        {
            add     { CommandManager.RequerySuggested += value; }
            remove  { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return App.Network != null && App.Training != null && App.Training.Patterns != null;
        }

        public void Execute(object parameter)
        {
            if (trainingVM.TrainingRunning)
            {
                tokenSource.Cancel();
                trainingVM.TrainingRunning = false;
            }
            else if(int.TryParse(parameter as string, out int epochs))
            {
                tokenSource = new CancellationTokenSource();
                App.Training.Run(epochs, tokenSource.Token);
                trainingVM.TrainingRunning = true;
            }
        }
    }
}
