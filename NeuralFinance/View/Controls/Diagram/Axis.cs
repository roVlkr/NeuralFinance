using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NeuralFinance.View.Controls.Diagram
{
    public abstract class Axis<T> : Canvas where T : IComparable
    {
        #region Static members
        // Dependency Properties
        public static readonly DependencyProperty MinStopsProperty;
        public static readonly DependencyProperty IntervalPatternModeProperty;
        public static readonly DependencyProperty OrientationProperty;
        public static readonly DependencyProperty MinimumProperty;
        public static readonly DependencyProperty MaximumProperty;

        // Readonly Dependency Properties
        private static readonly DependencyPropertyKey StopsPropertyKey;
        public static readonly DependencyProperty StopsProperty;

        static Axis()
        {
            MinStopsProperty = DependencyProperty.Register(nameof(MinStops), typeof(int),
                typeof(Axis<T>), new PropertyMetadata(2, PropertyChanged, MinStops_CoerceValue));

            IntervalPatternModeProperty = DependencyProperty.Register(nameof(IntervalPatternMode),
                typeof(IntervalPatternMode), typeof(Axis<T>), new PropertyMetadata(IntervalPatternMode.Inner));

            OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation),
                typeof(Axis<T>), new PropertyMetadata(Orientation.Vertical, PropertyChanged));

            StopsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Stops), typeof(List<AxisMarkerControl>),
                typeof(Axis<T>), new PropertyMetadata(new List<AxisMarkerControl>(), Stops_PropertyChanged));

            StopsProperty = StopsPropertyKey.DependencyProperty;

            MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(T),
                typeof(Axis<T>), new PropertyMetadata(PropertyChanged));

            MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(T),
                typeof(Axis<T>), new PropertyMetadata(PropertyChanged));
        }

        private static void Stops_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Axis<T> axis)
            {
                if (axis.IsLoaded)
                {
                    axis.Children.Clear();

                    axis.Children.Add(axis.mainLine);
                    foreach (var marker in axis.Stops)
                    {
                        axis.Children.Add(marker);

                        // Adjust the transformation
                        marker.UpdateLayout();

                        if (marker.Orientation == Orientation.Horizontal)
                            marker.RenderTransform = new TranslateTransform(
                                -marker.ActualWidth, -marker.ActualHeight / 2);
                        else if (marker.Orientation == Orientation.Vertical)
                            marker.RenderTransform = new TranslateTransform(
                                -marker.ActualWidth / 2, 0);
                    }
                }
                else
                {
                    axis.Loaded += (sender, args) => Stops_PropertyChanged(d, e);
                }
            }
        }

        protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Axis<T> axis)
            {
                axis.InitializeMainLine();
                axis.InitializeAxisMarkers();
            }
        }

        private static object MinStops_CoerceValue(DependencyObject d, object baseValue)
        {
            if ((int)baseValue < 2)
                return 2;

            return baseValue;
        }
        #endregion

        private Line mainLine;

        private void InitializeMainLine()
        {
            mainLine = new Line
            {
                X1 = 0,
                Y1 = 0,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            if (Orientation == Orientation.Vertical)
            {
                mainLine.X2 = 0;                
                mainLine.SetBinding(Line.Y2Property, new Binding("ActualHeight")
                {
                    Source = this
                });
            }
            else if (Orientation == Orientation.Horizontal)
            {
                mainLine.Y2 = 0;
                mainLine.SetBinding(Line.X2Property, new Binding("ActualWidth")
                {
                    Source = this
                });
            }

            Children.Add(mainLine);
        }

        protected AxisMarkerControl CreateAxisMarker(string description, double percentage)
        {
            var marker = new AxisMarkerControl { Description = description };

            if (Orientation == Orientation.Horizontal)
            {
                marker.Orientation = Orientation.Vertical;
                marker.SetBinding(LeftProperty, new Binding("ActualWidth")
                {
                    Source = this,
                    Converter = new DimensionToPositionConverter(),
                    ConverterParameter = percentage
                });
            }
            else if (Orientation == Orientation.Vertical)
            {
                marker.Orientation = Orientation.Horizontal;
                marker.SetBinding(TopProperty, new Binding("ActualHeight")
                {
                    Source = this,
                    Converter = new DimensionToPositionConverter(),
                    ConverterParameter = 1 - percentage
                });
            }

            return marker;
        }

        private void InitializeAxisMarkers()
        {
            try
            {
                IntervalPattern = CreateIntervalPattern();
            }
            catch (Exception)
            {
                return;  // Wait for better times
            }

            var stops = new List<AxisMarkerControl>();

            while (IntervalPattern.MoveNext())
            {
                var marker = CreateAxisMarker(GetMarkerDescription(IntervalPattern.Current),
                    IntervalPattern.CurrentPercentage);
                stops.Add(marker);
            }

            Stops = stops;
        }

        public double TransformModelToView(T modelPosition)
        {
            double percentage = IntervalPattern.GetPercentage(modelPosition);

            if (Orientation == Orientation.Vertical)
                return ActualHeight * (1 - percentage);
            else
                return ActualWidth * percentage;
        }

        #region Abstract methods
        protected abstract string GetMarkerDescription(T value);
        protected abstract IntervalPattern<T> CreateIntervalPattern();        
        #endregion

        #region Setters and Getters
        public int MinStops
        {
            get => (int)GetValue(MinStopsProperty);
            set => SetValue(MinStopsProperty, value);
        }

        public List<AxisMarkerControl> Stops
        {
            get => (List<AxisMarkerControl>)GetValue(StopsProperty);
            protected set => SetValue(StopsPropertyKey, value);
        }

        public IntervalPatternMode IntervalPatternMode
        {
            get => (IntervalPatternMode)GetValue(IntervalPatternModeProperty);
            set => SetValue(IntervalPatternModeProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public T Minimum
        {
            get => (T)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public T Maximum
        {
            get => (T)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public IntervalPattern<T> IntervalPattern { get; protected set; }
        #endregion
    }
}
