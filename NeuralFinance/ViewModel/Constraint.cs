using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel
{
    public class Constraint
    {
        public static readonly Constraint intGreaterZero;
        public static readonly Constraint doubleGreaterZero;
        public static readonly Constraint doubleGreaterOne;
        public static readonly Constraint doubleLessOne;

        static Constraint()
        {
            intGreaterZero = new Constraint(o => (int)o > 0, (string)App.Current.TryFindResource("errorMessageGreaterZero"));
            doubleGreaterZero = new Constraint(o => (double)o > 0, (string)App.Current.TryFindResource("errorMessageGreaterZero"));
            doubleGreaterOne = new Constraint(o => (double)o > 0, (string)App.Current.TryFindResource("errorMessageLessOne"));
            doubleLessOne = new Constraint(o => (double)o < 1, (string)App.Current.TryFindResource("errorMessageGreaterOne"));
        }

        private readonly Predicate<object> predicate;

        public Constraint(Predicate<object> predicate, string errorMessage)
        {
            this.predicate = predicate;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        public bool Fulfilled(object o)
        {
            return predicate(o);
        }
    }
}
