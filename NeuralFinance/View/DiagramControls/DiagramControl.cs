
using NeuralFinance.View.DiagramControls.Axis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NeuralFinance.View.DiagramControls
{
    public class DiagramControl : Canvas
    {
        // The Visual Children
        private readonly Polyline path;
        private readonly ValueAxis valueAxis;
        private readonly TimeAxis timeAxis;

        private readonly double[] intervalFactors = new double[] { 0.2, 0.25, 0.5, 1, 1.5, 2, 2.5, 5, 10 };

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(Dictionary<DateTime, double>),
                typeof(DiagramControl), new PropertyMetadata(null, OnPointsChanged));

        public static readonly DependencyProperty PreferredYIntervalCountProperty =
            DependencyProperty.Register("PreferredYIntervalCount", typeof(int), typeof(DiagramControl), new PropertyMetadata(5));

        public static readonly DependencyProperty PreferredXIntervalCountProperty =
            DependencyProperty.Register("PreferredXIntervalCount", typeof(int), typeof(DiagramControl), new PropertyMetadata(5));

        public static readonly DependencyProperty MaxPointsVisibleProperty =
            DependencyProperty.Register("MaxPointsVisible", typeof(int), typeof(DiagramControl), new PropertyMetadata(int.MaxValue));

        public DiagramControl()
        {
            timeAxis = new TimeAxis();
            valueAxis = new ValueAxis();
            path = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeLineJoin = PenLineJoin.Round
            };

            Binding widthBinding = new Binding("ActualWidth") { Source = this };
            Binding heightBinding = new Binding("ActualHeight") { Source = this };

            timeAxis.SetBinding(TopProperty, heightBinding);
            timeAxis.SetValue(LeftProperty, 0.0);
            timeAxis.SetBinding(WidthProperty, widthBinding);

            valueAxis.SetValue(LeftProperty, 0.0);
            valueAxis.SetValue(TopProperty, 0.0);
            valueAxis.SetBinding(HeightProperty, heightBinding);

            Children.Add(valueAxis);
            Children.Add(timeAxis);
            Children.Add(path);

            var lazySizeChangedWrapper = new LazySizeChangedWrapper();
            SizeChanged += lazySizeChangedWrapper.InvokeDelayedEvent;
            lazySizeChangedWrapper.LazySizeChanged += DiagramControl_LazySizeChanged;
        }

        private void DiagramControl_LazySizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitializePath();
        }

        public Dictionary<DateTime, double> Points
        {
            get { return (Dictionary<DateTime, double>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public int PreferredYIntervalCount
        {
            get { return (int)GetValue(PreferredYIntervalCountProperty); }
            set { SetValue(PreferredYIntervalCountProperty, value); }
        }

        public int PreferredXIntervalCount
        {
            get { return (int)GetValue(PreferredXIntervalCountProperty); }
            set { SetValue(PreferredXIntervalCountProperty, value); }
        }

        public int MaxPointsVisible
        {
            get { return (int)GetValue(MaxPointsVisibleProperty); }
            set { SetValue(MaxPointsVisibleProperty, value); }
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiagramControl diag)
            {
                if (!diag.IsLoaded)
                {
                    diag.Loaded += (sender, args) => OnPointsChanged(d, e);
                    return;
                }

                diag.Initialize(e.NewValue as Dictionary<DateTime, double>);
            }
        }

        private void Initialize(Dictionary<DateTime, double> points)
        {
            if (points == null)
            {
                InitializeStandardValues();
                return;
            }

            // Value axis
            double minValue = Points.Values.Min();
            double maxValue = Points.Values.Max();
            double range = maxValue - minValue;

            double potency = Math.Pow(10, Math.Floor(Math.Log10(range)));

            int i;
            for (i = 0; true; i++)
            {
                var intervalHeight = intervalFactors[i + 1] * potency;
                var offset = Math.Floor(minValue / intervalHeight) * intervalHeight;

                if (intervalHeight > Math.Ceiling((maxValue - offset) / PreferredYIntervalCount))
                    break;
            }

            valueAxis.IntervalHeight = intervalFactors[i] * potency;

            valueAxis.Offset = Math.Floor(minValue / valueAxis.IntervalHeight) * valueAxis.IntervalHeight;
            valueAxis.IntervalCount = (int)Math.Ceiling((maxValue - valueAxis.Offset) / valueAxis.IntervalHeight);

            // Time axis
            timeAxis.Offset = Points.Keys.Min();
            timeAxis.Range = Points.Keys.Max() - timeAxis.Offset;

            // Path
            InitializePath();
        }

        private void InitializePath()
        {
            if ((Points?.Count ?? 0) == 0) return;

            List<Point> visiblePoints = (from entry in Points
                                         select new Point(
                                            timeAxis.Transform(entry.Key),
                                            valueAxis.Transform(entry.Value)))
                                            .ToList();

            if (visiblePoints.Count > MaxPointsVisible)
            {
                var remainList = new List<Point>
                {
                    visiblePoints[0], visiblePoints[visiblePoints.Count - 1]
                };

                double step = (double)visiblePoints.Count / MaxPointsVisible;

                for (double i = 0; i < visiblePoints.Count; i += step)
                    remainList.Add(visiblePoints[(int)i]);

                visiblePoints.RemoveAll(point => !remainList.Contains(point));
            }

            path.Points = new PointCollection(visiblePoints);
        }

        private void InitializeStandardValues()
        {
            valueAxis.Offset = 0;
            valueAxis.IntervalHeight = 0.2;
            valueAxis.IntervalCount = 5;

            timeAxis.Offset = DateTime.Today;
            timeAxis.Range = TimeSpan.FromSeconds(1);
        }
    }
}
