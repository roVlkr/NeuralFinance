using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NeuralFinance.ViewModel.ValueConverters
{
    public class BooleanToStringConverter : IValueConverter
    {
        public BooleanToStringConverter()
        {
            // Standard values
            TrueValue = "True";
            FalseValue = "False";
        }

        public string TrueValue { get; set; }

        public string FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? b = value as bool?;

            if (b == false)
                return FalseValue;
            else if (b == true)
                return TrueValue;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;

            if (s == FalseValue)
                return false;
            else if (s == TrueValue)
                return true;

            return null;
        }
    }
}
