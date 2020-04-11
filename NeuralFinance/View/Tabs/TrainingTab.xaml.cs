using NeuralFinance.ViewModel;
using System;

namespace NeuralFinance.View.Tabs
{
    /// <summary>
    /// Interaktionslogik für TrainingTab.xaml
    /// </summary>
    public partial class TrainingTab : Form
    {
        public TrainingTab()
        {
            InitializeComponent();
        }

        private void TrainingCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(HasError || App.Network == null || App.Training == null ||
                App.Training.Patterns == null);
        }

        private void TrainingCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var viewModel = (TrainingVM)ViewModel;

            if (!viewModel.TrainingRunning)  // Training is to be started
            {
                trainingAccuracyDiagram.Reset();
                validationAccuracyDiagram.Reset();
            }
            
            viewModel.TrainingCommand_Execute();
        }
    }
}
