using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuralFinance.View.DiagramControls.Axis
{
    /// <summary>
    /// Interaktionslogik für YMarker.xaml
    /// </summary>
    public partial class YMarker : UserControl
    {
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(YMarker), new PropertyMetadata(""));

        public YMarker()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                mainGrid.SetValue(Canvas.LeftProperty, -mainGrid.ActualWidth + 3.0);
                mainGrid.SetValue(Canvas.TopProperty, -mainGrid.ActualHeight / 2);
            };
        }
    }
}
