using NeuralFinance.ViewModel.Helper;
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
        public TypeDependencyWrapper NumberTypeWrapper { get; set; }

        private Type NumberType => NumberTypeWrapper.Type;

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

                    return new ValidationResult(false, "Value is not of type Integer.");
                }
                else if (NumberType == typeof(double))
                {
                    if (double.TryParse(s, out _))
                    {
                        return ValidationResult.ValidResult;
                    }

                    return new ValidationResult(false, "Value is not of type Double.");
                }
            }

            return new ValidationResult(false, "Unknown number type or value null.");
        }
    }
}
