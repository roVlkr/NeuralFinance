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
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_EpochAxis", Type = typeof(DoubleAxis))]
    [TemplatePart(Name = "PART_ValueAxis", Type = typeof(DoubleAxis))]
    [TemplatePart(Name = "PART_Path", Type = typeof(Polyline))]
    public class QuickDiagram : Control
    {
        public static readonly DependencyProperty CurrentPointProperty;
        public static readonly DependencyProperty MaxEpochsProperty;
        public static readonly DependencyProperty HintProperty;

        static QuickDiagram()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickDiagram),
                new FrameworkPropertyMetadata(typeof(QuickDiagram)));

            CurrentPointProperty = DependencyProperty.Register(nameof(CurrentPoint), typeof(Tuple<int, double>),
                typeof(QuickDiagram), new PropertyMetadata(new Tuple<int, double>(0, 0), CurrentPoint_PropertyChanged));

            MaxEpochsProperty = DependencyProperty.Register(nameof(MaxEpochs), typeof(int), typeof(QuickDiagram),
                new PropertyMetadata(100, MaxEpochs_PropertyChanged));

            HintProperty = DependencyProperty.Register(nameof(Hint), typeof(string), typeof(QuickDiagram),
                new PropertyMetadata(""));
        }

        private static void CurrentPoint_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is QuickDiagram diagram)
            {
                var point = (Tuple<int, double>)e.NewValue;

                if (diagram.valueAxis != null && diagram.epochAxis != null &&
                    diagram.path != null)
                {
                    diagram.AddPoint(point.Item1, point.Item2);
                }                
            }
        }

        private static void MaxEpochs_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is QuickDiagram diagram)
            {
                if (diagram.path != null)
                {
                    diagram.path.Points.Clear();
                }                    
            }
        }

        private Canvas canvas;
        private DoubleAxis valueAxis;
        private DoubleAxis epochAxis;
        private Polyline path;
        private bool firstPoint;

        private readonly Dictionary<int, double> modelPath;

        public QuickDiagram()
        {
            modelPath = new Dictionary<int, double>();
        }

        public Tuple<int, double> CurrentPoint
        {
            get => (Tuple<int, double>)GetValue(CurrentPointProperty);
            set => SetValue(CurrentPointProperty, value);
        }

        public int MaxEpochs
        {
            get => (int)GetValue(MaxEpochsProperty);
            set => SetValue(MaxEpochsProperty, value);
        }

        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

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
                epochAxis = Template.FindName("PART_EpochAxis", this) as DoubleAxis;
                path = Template.FindName("PART_Path", this) as Polyline;
            }
        }

        private void LazySizeChanged(object sender, RoutedEventArgs e)
        {
            // Repaint path
            path.Points.Clear();
            foreach (var entry in modelPath)
            {
                var uiPoint = new Point(epochAxis.TransformModelToView(entry.Key),
                    valueAxis.TransformModelToView(entry.Value));

                path.Points.Add(uiPoint);
            }
        }

        public void Reset()
        {
            firstPoint = true;
            modelPath.Clear();
            path.Points.Clear();
        }

        public void AddPoint(int epoch, double value)
        {
            if (!modelPath.ContainsKey(epoch))
                modelPath.Add(epoch, value);
            else
                modelPath[epoch] = value;

            if (firstPoint)
            {
                valueAxis.Maximum = value;
                firstPoint = false;
            }

            if (value <= valueAxis.Maximum)
            {
                var uiPoint = new Point(epochAxis.TransformModelToView(epoch),
                valueAxis.TransformModelToView(value));

                path.Points.Add(uiPoint);
            }         
        }
    }
}
