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

namespace NeuralFinance.View.Controls.Diagram
{
    public class AxisMarkerControl : Control
    {
        public static readonly DependencyProperty OrientationProperty;
        public static readonly DependencyProperty DescriptionProperty;

        static AxisMarkerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisMarkerControl),
                new FrameworkPropertyMetadata(typeof(AxisMarkerControl)));

            OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation),
                typeof(AxisMarkerControl), new PropertyMetadata(Orientation.Horizontal));

            DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string),
                typeof(AxisMarkerControl), new PropertyMetadata(""));
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
    }
}
