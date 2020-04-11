using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralFinance.ViewModel
{
    public class ValueWrapper<T> : ViewModelBase where T : IComparable
    {
        private T value;

        public ValueWrapper(T value, params Constraint<T>[] constraints)
        {
            Constraints = constraints ?? new Constraint<T>[0];
            Value = value;
        }

        public Constraint<T>[] Constraints { get; }

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();

                foreach (var constraint in Constraints)
                    ObserveConstraint(constraint.Fulfilled(value), constraint.ErrorMessageKey);
            }
        }
    }
}
