using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace NeuralFinance.ViewModel
{
    public class ValueWrapperConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                try
                {
                    var result = new ValueWrapper(typeof(int), int.Parse(s));

                    return result;
                }
                catch (FormatException) { }

                try
                {
                    return new ValueWrapper(double.Parse(s));
                }
                catch (FormatException) { }
            }

            return new ValueWrapper(value);
        }
    }
}
