using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace NeuralFinance.View.DiagramControls
{
    /// <summary>
    /// Interaktionslogik für TrackingDiagramControl.xaml
    /// </summary>
    public partial class TrackingDiagramControl : UserControl
    {
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(IDictionary<int, double>), typeof(TrackingDiagramControl),
                new PropertyMetadata(null, Values_PropertyChanged));

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
            if (!IsLoaded || Values == null) return;

            var pen = new Pen(Brushes.Black, 1);

            var keyEnumerator = Values.Keys.GetEnumerator();
            keyEnumerator.MoveNext();
            var firstKey = keyEnumerator.Current;
            while(keyEnumerator.MoveNext())
            {
                var secondKey = keyEnumerator.Current;

                var point0 = new Point(
                    CalculateVisualX(firstKey),
                    CalculateVisualY(Values[firstKey]));

                var point1 = new Point(
                    CalculateVisualX(secondKey),
                    CalculateVisualY(Values[secondKey]));

                ctx.DrawLine(pen, point0, point1);

                firstKey = secondKey;
            }
        }

        private static void Values_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackingDiagramControl diagram)
            {
                if (!diagram.IsLoaded)
                {
                    diagram.Loaded += (sender, args) => Values_PropertyChanged(d, e);
                    return;
                }

                if (e.NewValue is IDictionary<int, double> values)
                {
                    var maximum = values.Count == 0 ? 0 : values.Keys.Max();

                    if (maximum > diagram.xAxis.Maximum)
                        diagram.xAxis.Maximum *= 1.5;

                    diagram.canvas.InvalidateVisual();
                }               
            }
        }

        public IDictionary<int, double> Values
        {
            get { return (IDictionary<int, double>)GetValue(ValuesProperty); }
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
