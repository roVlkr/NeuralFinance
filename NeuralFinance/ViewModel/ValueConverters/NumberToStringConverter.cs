using NeuralFinance.ViewModel.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NeuralFinance.ViewModel.ValueConverters
{
    public class NumberToStringConverter : IValueConverter
    {
        public TypeDependencyWrapper NumberTypeWrapper { get; set; }

        private Type NumberType => NumberTypeWrapper.Type;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (NumberType == typeof(int))
                {
                    return ((int)value).ToString();
                }
                else if (NumberType == typeof(double))
                {
                    return ((double)value).ToString();
                }
            }            

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;

            try
            {
                if (NumberType == typeof(int))
                {
                    return int.Parse(s);
                }
                else if (NumberType == typeof(double))
                {
                    return double.Parse(s);
                }
            }
            catch (Exception) { }
            
            return Binding.DoNothing;
        }
    }
}
