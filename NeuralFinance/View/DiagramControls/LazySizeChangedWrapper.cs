using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace NeuralFinance.View.DiagramControls
{
    public class LazySizeChangedWrapper
    {
        private readonly int observingDuration = 100;
        private DispatcherTimer dispatcherTimer;

        public event SizeChangedEventHandler LazySizeChanged;

        public LazySizeChangedWrapper(int observingDuration = 100)
        {
            this.observingDuration = observingDuration;
        }

        public void InvokeDelayedEvent(object sender, SizeChangedEventArgs args)
        {
            dispatcherTimer?.Stop();

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(observingDuration)
            };

            dispatcherTimer.Tick += (s, e) =>
            {
                LazySizeChanged.Invoke(sender, args);
                dispatcherTimer.Stop();
            };

            dispatcherTimer.Start();
        }
    }
}
