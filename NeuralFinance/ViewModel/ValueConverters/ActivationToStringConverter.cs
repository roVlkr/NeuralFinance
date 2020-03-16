using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NeuralFinance.ViewModel.ValueConverters
{
    [ValueConversion(typeof(INetActivation), typeof(string))]
    public class ActivationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == NetFunctions.Rectifier)
                return "RELU";
            if (value == NetFunctions.Sigmoid)
                return "Sigmoid";
            if (value == NetFunctions.SELU)
                return "SELU";
            if (value == NetFunctions.Softplus)
                return "Softplus";
            if (value == NetFunctions.Identity)
                return "Linear";

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;

            switch (s)
            {
                case "RELU":
                    return NetFunctions.Rectifier;
                case "Sigmoid":
                    return NetFunctions.Sigmoid;
                case "SELU":
                    return NetFunctions.SELU;
                case "Softplus":
                    return NetFunctions.Softplus;
                case "Linear":
                case "Identity":
                    return NetFunctions.Identity;
            }

            return null;
        }
    }
}
