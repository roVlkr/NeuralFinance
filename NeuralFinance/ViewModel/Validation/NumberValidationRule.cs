using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NeuralFinance.ViewModel.Validation
{
    public class NumberValidationRule : ValidationRule
    {
        public Type NumberType { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string s)
            {
                if (NumberType == typeof(int))
                {
                    if (int.TryParse(s, out _))
                    {
                        return ValidationResult.ValidResult;
                    }

                    return new ValidationResult(false, App.Current.TryFindResource("errorMessageTypeInt"));
                }
                else if (NumberType == typeof(double))
                {
                    if (s.EndsWith(',') || s.EndsWith('.'))
                    {
                        return new ValidationResult(false, App.Current.TryFindResource("errorMessageTypeDouble"));
                    }

                    if (double.TryParse(s, out _))
                    {
                        return ValidationResult.ValidResult;
                    }

                    return new ValidationResult(false, App.Current.TryFindResource("errorMessageTypeDouble"));
                }
            }

            return new ValidationResult(false, "Unknown number type or value null.");
        }
    }
}
