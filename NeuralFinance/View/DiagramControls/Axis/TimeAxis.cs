using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NeuralFinance.View.DiagramControls.Axis
{
    public class TimeAxis : Canvas
    {
        public static readonly DependencyProperty IntervalCountProperty =
            DependencyProperty.Register("IntervalCount", typeof(int), typeof(TimeAxis), new PropertyMetadata(5, OnPropertyChanged));

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(DateTime), typeof(TimeAxis), new PropertyMetadata(DateTime.Today, OnPropertyChanged));

        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(TimeSpan), typeof(TimeAxis), new PropertyMetadata(TimeSpan.FromSeconds(1), OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimeAxis axis)
            {
                axis.Initialize();
            }
        }

        public DateTime Offset
        {
            get { return (DateTime)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }        

        public TimeSpan Range
        {
            get { return (TimeSpan)GetValue(RangeProperty); }
            set { SetValue(RangeProperty, value); }
        }        

        public int IntervalCount
        {
            get { return (int)GetValue(IntervalCountProperty); }
            set { SetValue(IntervalCountProperty, value); }
        }

        private void Initialize()
        {
            Children.Clear();

            var widthBinding = new Binding("ActualWidth")
            {
                Source = this
            };

            var mainLine = new Line { X1 = 0, Y1 = 0, Y2 = 0, Stroke = Brushes.Black };
            mainLine.SetBinding(Line.X2Property, widthBinding);
            Children.Add(mainLine);

            for (int i = 0; i <= IntervalCount; i++)
            {
                string description = Range / IntervalCount >= TimeSpan.FromDays(1) ?
                    (Offset + i * Range / IntervalCount).ToShortDateString() :
                    (Offset + i * Range / IntervalCount).ToShortTimeString();

                var positionBinding = new Binding("ActualWidth")
                {
                    Source = this,
                    Converter = new DimensionToPositionConverter(),
                    ConverterParameter = (double)i / IntervalCount
                };

                var marker = new XMarker { Description = description };
                marker.SetBinding(LeftProperty, positionBinding);
                Children.Add(marker);
            }
        }

        public double Transform(DateTime t)
        {
            return (t - Offset) / Range * ActualWidth;
        }
    }
}
