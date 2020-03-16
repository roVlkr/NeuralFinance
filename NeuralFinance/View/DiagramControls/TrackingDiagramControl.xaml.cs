using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuralFinance.View.DiagramControls
{
    /// <summary>
    /// Interaktionslogik für TrackingDiagramControl.xaml
    /// </summary>
    public partial class TrackingDiagramControl : UserControl
    {
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(List<double>), typeof(TrackingDiagramControl),
                new PropertyMetadata(new List<double>(), ValuesPropertyChanged));

        public static readonly DependencyProperty DiagramNameProperty =
            DependencyProperty.Register("DiagramName", typeof(string), typeof(TrackingDiagramControl),
                new PropertyMetadata(""));

        public TrackingDiagramControl()
        {
            InitializeComponent();

            canvas.DrawingMethod = RenderChart;
        }

        private void RenderChart(DrawingContext ctx)
        {
            if (!IsLoaded) return;

            var pen = new Pen(Brushes.Black, 1);

            for (int i = 1; i < Values.Count; i++)
            {
                var point0 = new Point(
                    CalculateVisualX(i - 1),
                    CalculateVisualY(Values[i - 1]));

                var point1 = new Point(
                    CalculateVisualX(i),
                    CalculateVisualY(Values[i]));

                ctx.DrawLine(pen, point0, point1);
            }
        }

        private static void ValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackingDiagramControl diagram)
            {
                if (!diagram.IsLoaded)
                {
                    diagram.Loaded += (sender, args) => ValuesPropertyChanged(d, e);
                    return;
                }

                if (diagram.Values.Count > diagram.xAxis.Maximum)
                    diagram.xAxis.Maximum *= 1.5;

                diagram.canvas.InvalidateVisual();
            }
        }

        public List<double> Values
        {
            get { return (List<double>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        public string DiagramName
        {
            get { return (string)GetValue(DiagramNameProperty); }
            set { SetValue(DiagramNameProperty, value); }
        }

        private double CalculateVisualX(double xValue)
        {
            var minValue = xAxis.AxisPattern.MinStop;
            var maxValue = xAxis.AxisPattern.MaxStop;
            var range = maxValue - minValue;

            return (xValue - minValue) / range * xAxis.ActualWidth;
        }

        private double CalculateVisualY(double yValue)
        {
            var minValue = yAxis.AxisPattern.MinStop;
            var maxValue = yAxis.AxisPattern.MaxStop;
            var range = maxValue - minValue;

            return (maxValue - yValue) / range * yAxis.ActualHeight;
        }
    }
}
