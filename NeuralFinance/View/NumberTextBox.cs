using NeuralFinance.ViewModel;
using NeuralFinance.ViewModel.Validation;
using NeuralFinance.ViewModel.ValueConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeuralFinance.View
{
    public class NumberTextBox : TextBox
    {
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty UpdateSourceTriggerProperty;

        static NumberTextBox()
        {
            ValueProperty = DependencyProperty.Register(nameof(Value), typeof(ValueWrapper),
                typeof(NumberTextBox), new PropertyMetadata(null, Value_PropertyChanged));
            UpdateSourceTriggerProperty =  DependencyProperty.Register(nameof(UpdateSourceTrigger),
                typeof(UpdateSourceTrigger), typeof(NumberTextBox), new PropertyMetadata(UpdateSourceTrigger.Default));
        }

        private static void Value_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NumberTextBox numberTextBox)
            {
                numberTextBox.UpdateTextBinding((ValueWrapper)e.NewValue);
            }
        }

        private void UpdateTextBinding(ValueWrapper newValue)
        {
            SetBinding(TextProperty, new Binding("Value")
            {
                Source = Value,
                Converter = new NumberToStringConverter { NumberType = newValue.ValueType },
                ValidationRules =
                {
                    new NotifyDataErrorValidationRule(),
                    new NumberValidationRule { NumberType = newValue.ValueType }
                },
                NotifyOnValidationError = true,
                UpdateSourceTrigger = UpdateSourceTrigger
            });
        }

        public ValueWrapper Value
        {
            get { return (ValueWrapper)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return (UpdateSourceTrigger)GetValue(UpdateSourceTriggerProperty); }
            set { SetValue(UpdateSourceTriggerProperty, value); }
        }
    }
}
