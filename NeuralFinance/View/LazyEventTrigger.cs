using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace NeuralFinance.View
{
    public class LazyEventTrigger
    {
        private readonly int observingDuration;
        private DispatcherTimer dispatcherTimer;

        public event RoutedEventHandler LazyEvent;

        public LazyEventTrigger(int observingDuration = 100)
        {
            this.observingDuration = observingDuration;
        }

        public void InvokeDelayedEvent(object sender, RoutedEventArgs args)
        {
            dispatcherTimer?.Stop();

            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(observingDuration)
            };

            dispatcherTimer.Tick += (s, e) =>
            {
                LazyEvent.Invoke(sender, args);
                dispatcherTimer.Stop();
            };

            dispatcherTimer.Start();
        }
    }
}
