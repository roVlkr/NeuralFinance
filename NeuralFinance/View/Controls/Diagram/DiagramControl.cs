using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Shapes;
using System.Windows.Media;

namespace NeuralFinance.View.Controls.Diagram
{
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_ValueAxis", Type = typeof(DoubleAxis))]
    [TemplatePart(Name = "PART_TimeAxis", Type = typeof(DateTimeAxis))]
    [TemplatePart(Name = "PART_Path", Type = typeof(Polyline))]
    public class DiagramControl : Control
    {
        #region Static members
        public static readonly DependencyProperty SourceProperty;
        public static readonly DependencyProperty MaxSourceCountProperty;
        public static readonly DependencyProperty MinYStopsProperty;
        public static readonly DependencyProperty MinXStopsProperty;

        static DiagramControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagramControl),
                   new FrameworkPropertyMetadata(typeof(DiagramControl)));

            SourceProperty = DependencyProperty.Register(nameof(Source),
                typeof(Dictionary<DateTime, double>),
                typeof(DiagramControl), new PropertyMetadata(null, PropertyChanged));

            MaxSourceCountProperty = DependencyProperty.Register(nameof(MaxSourceCount), typeof(int),
                typeof(DiagramControl), new PropertyMetadata(int.MaxValue, PropertyChanged));

            MinXStopsProperty = DependencyProperty.Register(nameof(MinXStops), typeof(int),
                typeof(DiagramControl), new PropertyMetadata(0));

            MinYStopsProperty = DependencyProperty.Register(nameof(MinYStops), typeof(int),
                typeof(DiagramControl), new PropertyMetadata(0));
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiagramControl diagramControl)
            {
                if (diagramControl.IsLoaded)
                {
                    diagramControl.InitializeDiagram();
                }                    
            }
        }
        #endregion

        private Canvas canvas;
        private DoubleAxis valueAxis;
        private DateTimeAxis timeAxis;
        private Polyline path;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                canvas = Template.FindName("PART_Canvas", this) as Canvas;
                if (canvas != null)
                {
                    var lazySizeChanged = new LazyEventTrigger(25);
                    canvas.SizeChanged += lazySizeChanged.InvokeDelayedEvent;
                    lazySizeChanged.LazyEvent += LazySizeChanged;
                }

                valueAxis = Template.FindName("PART_ValueAxis", this) as DoubleAxis;
                timeAxis = Template.FindName("PART_TimeAxis", this) as DateTimeAxis;
                path = Template.FindName("PART_Path", this) as Polyline;

                InitializeDiagram();
            }
        }

        private void LazySizeChanged(object sender, RoutedEventArgs e)
        {
            InitializePath();
        }

        private void InitializePath()
        {
            if (path != null && timeAxis != null && valueAxis != null && Source != null &&
                timeAxis.IntervalPattern != null && valueAxis.IntervalPattern != null)
            {
                var visiblePoints = Source.Select(entry =>
                    new Point(timeAxis.TransformModelToView(entry.Key),
                    valueAxis.TransformModelToView(entry.Value)));

                path.Points = new PointCollection(visiblePoints);
            }
        }

        private void InitializeDiagram()
        {
            if (Source == null) return;

            // Set up the axes
            if (valueAxis != null)
            {
                valueAxis.Minimum = Source.Values.Min();
                valueAxis.Maximum = Source.Values.Max();
            }            

            if (timeAxis != null)
            {
                timeAxis.Minimum = Source.Keys.Min();
                timeAxis.Maximum = Source.Keys.Max();
            }               

            // Reduce source data
            if (Source.Count > MaxSourceCount)
            {
                var step = (double)Source.Count / MaxSourceCount;
                double d = 0;
                int i = 0;

                foreach (var entry in Source)
                {
                    if (i == (int)d) d += step;
                    else Source.Remove(entry.Key);
                    i++;
                }
            }

            InitializePath();
        }

        public Dictionary<DateTime, double> Source
        {
            get { return (Dictionary<DateTime, double>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public int MaxSourceCount
        {
            get { return (int)GetValue(MaxSourceCountProperty); }
            set { SetValue(MaxSourceCountProperty, value); }
        }

        public int MinXStops
        {
            get { return (int)GetValue(MinXStopsProperty); }
            set { SetValue(MinXStopsProperty, value); }
        }

        public int MinYStops
        {
            get { return (int)GetValue(MinYStopsProperty); }
            set { SetValue(MinYStopsProperty, value); }
        }
    }
}
