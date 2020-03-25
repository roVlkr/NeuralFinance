using NeuralFinance.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NeuralFinance.ViewModel.ValueConverters
{
    [ValueConversion(typeof(SystemState), typeof(AlertState))]
    class SystemStateToAlertStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SystemState systemState)
            {
                switch (systemState)
                {
                    case SystemState.NotInitialized:
                        return AlertState.Error;
                    case SystemState.SystemInitialized:
                        return AlertState.Warning;
                    case SystemState.DataInitialized:
                        return AlertState.Nothing;
                }
            }

            return AlertState.Error;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
