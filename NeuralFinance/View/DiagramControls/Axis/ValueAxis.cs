using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NeuralFinance.View.DiagramControls.Axis
{
    public class ValueAxis : Canvas
    {
        public static readonly DependencyProperty IntervalCountProperty =
            DependencyProperty.Register("IntervalCount", typeof(int), typeof(ValueAxis), new PropertyMetadata(5, OnPropertyChanged));

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(ValueAxis), new PropertyMetadata(0.0, OnPropertyChanged));

        public static readonly DependencyProperty IntervalHeightProperty =
            DependencyProperty.Register("IntervalHeight", typeof(double), typeof(ValueAxis), new PropertyMetadata(0.2, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ValueAxis axis)
            {
                axis.Initialize();
            }
        }

        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public double IntervalHeight
        {
            get { return (double)GetValue(IntervalHeightProperty); }
            set { SetValue(IntervalHeightProperty, value); }
        }

        public double Range => IntervalHeight * IntervalCount;

        public int IntervalCount
        {
            get { return (int)GetValue(IntervalCountProperty); }
            set { SetValue(IntervalCountProperty, value); }
        }

        private void Initialize()
        {
            Children.Clear();

            var heightBinding = new Binding("ActualHeight")
            {
                Source = this
            };

            var mainLine = new Line { X1 = 0, Y1 = 0, X2 = 0, Stroke = Brushes.Black };
            mainLine.SetBinding(Line.Y2Property, heightBinding);
            Children.Add(mainLine);

            for (int i = 0; i <= IntervalCount; i++)
            {
                string description = (Offset + i * IntervalHeight).ToString();

                var positionBinding = new Binding("ActualHeight")
                {
                    Source = this,
                    Converter = new DimensionToPositionConverter(),
                    ConverterParameter = 1.0 - (double)i / IntervalCount // = MarkerCount
                };

                var marker = new YMarker { Description = description };
                marker.SetBinding(TopProperty, positionBinding);

                var widthBinding = new Binding("ActualWidth")
                {
                    RelativeSource = new RelativeSource
                    {
                        Mode = RelativeSourceMode.FindAncestor,
                        AncestorType = typeof(DiagramControl)
                    }
                };

                var helperLine = new Line { X1 = 0, Stroke = Brushes.LightGray };
                helperLine.SetBinding(Line.X2Property, widthBinding);
                helperLine.SetBinding(Line.Y1Property, positionBinding);
                helperLine.SetBinding(Line.Y2Property, positionBinding);

                Children.Add(helperLine);
                Children.Add(marker);
            }
        }

        public double Transform(double x) => (1.0 - (x - Offset) / Range) * ActualHeight;
    }
}
