using NeuralFinance.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NeuralFinance.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Gives the amount of errors per control
        private readonly Dictionary<Control, int> validationErrors;

        public MainWindow()
        {
            InitializeComponent();
            validationErrors = new Dictionary<Control, int>();
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

        private void InitializeNetworkCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Check if all textboxes have no error
            e.CanExecute = validationErrors?.All(entry => entry.Value == 0) ?? false;
        }

        private void InitializeNetworkCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is TabItem tabItem)
            {
                if (tabItem.DataContext is NetworkVM networkVM)
                {
                    networkVM.InitializeNetwork();
                }
            }
        }

        private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.OriginalSource is TextBox textBox)
            {
                if (validationErrors.ContainsKey(textBox))
                {
                    switch (e.Action)
                    {
                        case ValidationErrorEventAction.Added:
                            validationErrors[textBox]++;
                            break;
                        case ValidationErrorEventAction.Removed:
                            validationErrors[textBox]--;
                            break;
                    }
                }
                else
                {
                    switch (e.Action)
                    {
                        case ValidationErrorEventAction.Added:
                            validationErrors.Add(textBox, 1);
                            break;
                        case ValidationErrorEventAction.Removed:
                            break; // Can't be called if there was no error before
                    }
                }
            }
        }
    }
}
