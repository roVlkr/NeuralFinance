using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace NeuralFinance.View.Controls.Diagram
{
    class DrawingCanvas : FrameworkElement
    {
        private const int delayMilliseconds = 100;

        public delegate void RenderMethod(DrawingContext ctx);

        public static readonly DependencyProperty DrawingMethodProperty =
            DependencyProperty.Register("DrawingMethod", typeof(RenderMethod), typeof(DrawingCanvas),
                new PropertyMetadata(null));

        private DispatcherTimer dispatcherTimer;

        public DrawingCanvas()
        {
            SizeChanged += DrawingCanvas_SizeChanged;
        }

        private void DrawingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            IsResizing = true;

            dispatcherTimer?.Stop();

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(delayMilliseconds)
            };

            dispatcherTimer.Tick += (s, args) =>
            {
                // After waiting a delay of delayMilliseconds after the last
                // SizeChanged event set the IsResizing flag to false.
                IsResizing = false; 
                InvalidateVisual();
            };

            dispatcherTimer.Start();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            base.OnRender(ctx);

            if (!IsResizing)
                DrawingMethod?.Invoke(ctx);
        }

        public bool IsResizing { get; set; }

        public RenderMethod DrawingMethod
        {
            get { return (RenderMethod)GetValue(DrawingMethodProperty); }
            set { SetValue(DrawingMethodProperty, value); }
        }
    }
}
