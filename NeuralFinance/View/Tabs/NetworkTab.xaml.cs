using NeuralFinance.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace NeuralFinance.View.Tabs
{
    /// <summary>
    /// Interaktionslogik für NetworkTab.xaml
    /// </summary>
    public partial class NetworkTab : Form
    {
        public NetworkTab() : base()
        {
            InitializeComponent();
        }

        private void InitializeNetworkCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !HasError;
        }

        private void InitializeNetworkCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Form form)
            {
                if (form.ViewModel is NetworkVM networkVM)
                {
                    MessageBox.Show(networkVM.NetStructure.ToString());
                    networkVM.InitializeNetwork();
                }
            }
        }
    }
}
