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
        public static readonly Constraint intGreaterZero = new Constraint(o => (int)o > 0, "Value is less or equal 0.");
        public static readonly Constraint doubleGreaterZero = new Constraint(o => (double)o > 0, "Value is less or equal 0.");
        public static readonly Constraint doubleGreaterOne = new Constraint(o => (double)o > 0, "Value is less or equal 1.");
        public static readonly Constraint doubleLessOne = new Constraint(o => (double)o < 1, "Value is greater or equal 1.");

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
