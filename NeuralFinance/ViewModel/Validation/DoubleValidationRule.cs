using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NeuralFinance.ViewModel.Validation
{
    public class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string s)
            {
                if (double.TryParse(s, out double _))
                {
                    return ValidationResult.ValidResult;
                }

                return new ValidationResult(false, Application.Current.TryFindResource("errorMessageTypeDouble"));
            }

            return new ValidationResult(false, "Value is not of type string");
        }
    }
}
