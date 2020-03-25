using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NeuralFinance.ViewModel.ValueConverters
{
    [ValueConversion(typeof(SystemState), typeof(string))]
    public class SystemStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SystemState systemState)
            {
                switch (systemState)
                {
                    case SystemState.NotInitialized:
                        return "Netzwerk und Training noch nicht geladen!";
                    case SystemState.SystemInitialized:
                        return "Daten noch nicht geladen!";
                    case SystemState.DataInitialized:
                        return "";
                }
            }

            return "Systemstatus nicht erkannt.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
