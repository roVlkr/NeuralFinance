using NeuralFinance.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeuralFinance.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_SelectAllText(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox_SelectAllText(sender, e);
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox && !Keyboard.FocusedElement.Equals(textBox))
            {
                Keyboard.Focus(textBox);
                e.Handled = true;
            }
        }

        #region Commands

        private void ConfirmDataCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(dataTab.HasError || Validation.GetHasError(estimateLengthTextBox)) &&
                App.NeuralSystem.SystemState == SystemState.SystemInitialized;
        }

        private void ConfirmDataCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ((DataVM)dataTab.ViewModel).ConfirmData();
        }

        #endregion
    }
}
