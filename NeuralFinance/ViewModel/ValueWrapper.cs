using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel
{
    public class ValueWrapper : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        protected object value;

        protected List<string> valueErrors;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public ValueWrapper(Type valueType, object value, params Constraint[] constraints)
        {
            valueErrors = new List<string>();

            ValueType = valueType;
            Constraints = constraints;

            Value = value;
        }

        public Type ValueType { get; }

        public Constraint[] Constraints { get; }

        public object Value
        {
            get => value;
            set
            {
                this.value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));

                if (!CheckValueErrors(value, out List<string> newErrors))
                {
                    valueErrors = newErrors;
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Value)));
                }
            }
        }

        public bool HasErrors => valueErrors.Count > 0;

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName != nameof(Value))
                return null;

            return valueErrors;
        }

        public bool CheckValueErrors(object newValue, out List<string> newErrors)
        {
            newErrors = new List<string>();

            if (newValue == null)
            {
                newErrors.Add("The new value is null");
            }
            else if (ValueType != newValue.GetType())
            {
                newErrors.Add("The new value has the wrong type!");
            }
            else
            {
                foreach (var constraint in Constraints)
                {
                    if (!constraint.Fulfilled(newValue))
                        newErrors.Add(constraint.ErrorMessage);
                }
            }

            // Returns true if the errors in the lists are the same
            return valueErrors.All(newErrors.Contains) &&
                newErrors.Count == valueErrors.Count;
        }
    }
}
