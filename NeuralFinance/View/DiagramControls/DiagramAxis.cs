using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NeuralFinance.View.DiagramControls
{
    class DiagramAxis : Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AxisPattern axisPattern;

        #region Dependency Property Definitions
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(DiagramAxis),
                new PropertyMetadata(0.0, DimensionPropertyChanged));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(DiagramAxis),
                new PropertyMetadata(1.0, DimensionPropertyChanged));

        public static readonly DependencyProperty MaxStopsProperty =
            DependencyProperty.Register("MaxStops", typeof(int), typeof(DiagramAxis),
                new PropertyMetadata(5, DimensionPropertyChanged));

        public static readonly DependencyProperty StrokeWidthProperty =
            DependencyProperty.Register("StrokeWidth", typeof(double), typeof(DiagramAxis),
                new PropertyMetadata(2.0));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(DiagramAxis),
                new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty AxisNameProperty =
            DependencyProperty.Register("AxisName", typeof(string), typeof(DiagramAxis),
                new PropertyMetadata("Description"));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DiagramAxis),
                new PropertyMetadata(Orientation.Horizontal));
        #endregion

        public DiagramAxis()
        {
            Initialized += (sender, args) =>
            {
                AxisPattern = new AxisPattern(Minimum, Maximum, MaxStops);
            };

            Loaded += (sender, args) =>
            {
                InvalidateVisual();
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var preferredSize = new Size(0, 0);
            var description = new List<FormattedText>();

            if (Orientation == Orientation.Horizontal)
            {
                // Add height of the line
                preferredSize.Height += 5 + StrokeWidth;

                if (AxisPattern.IsValid)
                {
                    description.Clear();

                    for (int i = 0; i <= AxisPattern.StopCount; i++)
                    {
                        var markerText = (AxisPattern.MinStop + i * AxisPattern.Step).ToString();
                        description.Add(GenerateFormattedText(markerText));
                    }

                    // Height of marker texts plus a gap of 5
                    preferredSize.Height += description.Max(text => text.Height + 5);

                    // Width of marker texts plus a gap of 5
                    preferredSize.Width += description.Sum(text => text.Width + 5);
                }

                var displayName = GenerateFormattedText(AxisName);

                // Add axis name height plusvtwo gaps of 5
                preferredSize.Height += displayName.Height + 10;

                // The preferred width should not be less than the width of the axis name
                preferredSize.Width = Math.Max(preferredSize.Width, displayName.Width);
            }
            else if (Orientation == Orientation.Vertical)
            {
                // Add width of the line
                preferredSize.Width += 5 + StrokeWidth;

                if (AxisPattern.IsValid)
                {
                    description.Clear();

                    for (int i = 0; i <= AxisPattern.StopCount; i++)
                    {
                        var markerText = Math.Round((AxisPattern.MinStop + i * AxisPattern.Step), 12).ToString();
                        description.Add(GenerateFormattedText(markerText));
                    }

                    // Height of marker texts plus a gap of 5
                    preferredSize.Height += description.Sum(text => text.Height + 5);

                    // Width of marker texts plus a gap of 5
                    preferredSize.Width += description.Max(text => text.Width + 5);
                }

                var displayName = GenerateFormattedText(AxisName);

                // Add axis name height plus three? gaps of 5 (to the width because it's rotated)
                preferredSize.Width += displayName.Height + 15;

                // The preferred height should not be less than the width of the axis' name
                preferredSize.Height = Math.Max(preferredSize.Height, displayName.Width);
            }

            return preferredSize;
        }

        #region Render Methods
        protected override void OnRender(DrawingContext ctx)
        {
            if (IsLoaded)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    DrawHorizontalAxis(ctx);
                }          
                else if (Orientation == Orientation.Vertical)
                {
                    DrawVerticalAxis(ctx);
                }                    
            }
        }

        private void DrawVerticalAxis(DrawingContext ctx)
        {
            var pen = new Pen(Stroke, StrokeWidth);
            double maxDisplayMarkerWidth = 0;

            // Main line
            ctx.DrawLine(pen, new Point(ActualWidth - StrokeWidth / 2, 0),
                new Point(ActualWidth - StrokeWidth / 2, ActualHeight));

            if (AxisPattern.IsValid)
            {
                // Markers
                var visualStep = ActualHeight / AxisPattern.StopCount;
                for (int i = 0; i <= AxisPattern.StopCount; i++)
                {
                    var markerY = ActualHeight - visualStep * i;
                    if (i == 0) markerY -= Math.Floor(StrokeWidth / 2);
                    if (i == AxisPattern.StopCount) markerY += Math.Floor(StrokeWidth / 2);

                    ctx.DrawLine(pen, new Point(ActualWidth - StrokeWidth, markerY),
                        new Point(ActualWidth - StrokeWidth - 5, markerY));

                    var markerText = Math.Round((AxisPattern.MinStop + i * AxisPattern.Step), 12).ToString();
                    var displayMarker = GenerateFormattedText(markerText);
                    var textY = markerY - displayMarker.Height / 2;

                    // On the edges of the axis, set the text higher or lower
                    if (i == 0) textY -= displayMarker.Height / 2;
                    if (i == AxisPattern.StopCount) textY = 0;

                    ctx.DrawText(displayMarker, new Point(
                        ActualWidth - StrokeWidth - 10 - displayMarker.Width,
                        textY));

                    if (maxDisplayMarkerWidth < displayMarker.Width)
                        maxDisplayMarkerWidth = displayMarker.Width;
                }
            }

            // Axis name
            var displayName = GenerateFormattedText(AxisName);
            var rotateTransform = new RotateTransform(-90, 0, 0);

            ctx.PushTransform(rotateTransform);
            ctx.DrawText(displayName, new Point(-ActualHeight / 2 - displayName.Width / 2,
                ActualWidth - displayName.Height - StrokeWidth - Math.Round(maxDisplayMarkerWidth) - 20));
            ctx.Pop();
        }

        private void DrawHorizontalAxis(DrawingContext ctx)
        {
            var pen = new Pen(Stroke, StrokeWidth);
            FormattedText displayMarker = null;

            // Main line
            ctx.DrawLine(pen, new Point(0, Math.Ceiling(StrokeWidth / 2)),
                new Point(ActualWidth, Math.Ceiling(StrokeWidth / 2)));

            if (AxisPattern.IsValid)
            {
                // Markers
                var visualStep = ActualWidth / AxisPattern.StopCount;
                for (int i = 0; i <= AxisPattern.StopCount; i++)
                {
                    var markerX = visualStep * i;
                    if (i == 0) markerX += Math.Floor(StrokeWidth / 2);
                    if (i == AxisPattern.StopCount) markerX -= Math.Floor(StrokeWidth / 2);

                    ctx.DrawLine(pen, new Point(markerX, StrokeWidth), new Point(markerX, StrokeWidth + 5));

                    var markerText = (AxisPattern.MinStop + i * AxisPattern.Step).ToString();
                    displayMarker = GenerateFormattedText(markerText);
                    var textX = markerX - displayMarker.Width / 2;

                    // On the edges of the axis, set the text higher or lower
                    if (i == 0) textX = 0;
                    if (i == AxisPattern.StopCount) textX -= displayMarker.Width / 2;
                    ctx.DrawText(displayMarker, new Point(textX, StrokeWidth + 10));
                }                
            }

            // Axis name
            var displayName = GenerateFormattedText(AxisName);
            ctx.DrawText(displayName, new Point(ActualWidth / 2 - displayName.Width / 2,
                StrokeWidth + 15 + displayMarker?.Height ?? 0));
        }
        #endregion

        private static void DimensionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DiagramAxis ax)
            {
                if (e.Property == MinimumProperty)
                {
                    ax.AxisPattern = new AxisPattern((double)e.NewValue, ax.Maximum, ax.MaxStops);
                }
                else if (e.Property == MaximumProperty)
                {
                    ax.AxisPattern = new AxisPattern(ax.Minimum, (double)e.NewValue, ax.MaxStops);
                }
                else if (e.Property == MaxStopsProperty)
                {
                    ax.AxisPattern = new AxisPattern(ax.Minimum, ax.Maximum, (int)e.NewValue);
                }
            }
        }

        #region Properties
        #region Dependency Properties
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public string AxisName
        {
            get { return (string)GetValue(AxisNameProperty); }
            set { SetValue(AxisNameProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeWidth
        {
            get { return (double)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, value); }
        }

        public int MaxStops
        {
            get { return (int)GetValue(MaxStopsProperty); }
            set { SetValue(MaxStopsProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        #endregion

        public AxisPattern AxisPattern
        {
            get => axisPattern;
            set
            {
                axisPattern = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AxisPattern"));
            }
        }

        #endregion

        private FormattedText GenerateFormattedText(string text)
        {
            return new FormattedText(text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily.Source),
                FontSize,
                Stroke,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }
    }
}
