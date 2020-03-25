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

namespace NeuralFinance.View
{
    public enum AlertState
    {
        Error,
        Warning,
        Nothing
    }

    public class AlertControl : Control
    {
        public static readonly DependencyProperty AlertStateProperty;
        public static readonly DependencyProperty TextProperty;

        static AlertControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlertControl),
                new FrameworkPropertyMetadata(typeof(AlertControl)));

            AlertStateProperty = DependencyProperty.Register("AlertState", typeof(AlertState),
                typeof(AlertControl), new PropertyMetadata(AlertState.Error));

            TextProperty = DependencyProperty.Register("Text", typeof(string),
                typeof(AlertControl), new PropertyMetadata(""));
        }

        public AlertState AlertState
        {
            get { return (AlertState)GetValue(AlertStateProperty); }
            set { SetValue(AlertStateProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
