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
    /// Interaktionslogik für Marker.xaml
    /// </summary>
    public partial class XMarker : UserControl
    {
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(XMarker), new PropertyMetadata(""));

        public XMarker()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                mainGrid.SetValue(Canvas.LeftProperty, -mainGrid.ActualWidth / 2);
                mainGrid.SetValue(Canvas.TopProperty, -3.0);
            };
        }
    }
}
