using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralFinance.ViewModel
{
    public class Constraint<T> where T : IComparable
    {
        public static readonly Constraint<T> greaterZero;
        public static readonly Constraint<T> greaterOne;
        public static readonly Constraint<T> lessOne;

        static Constraint()
        {
            greaterZero = new Constraint<T>(value => value.CompareTo(0) > 0,  // value greater than 0
                "errorMessageGreaterZero");
            lessOne = new Constraint<T>(value => value.CompareTo(1) < 0,      // value less than 1
                "errorMessageLessOne");
            greaterOne = new Constraint<T>(value => value.CompareTo(1) > 0,   // value greater than 1
                "errorMessageGreaterOne");
        }

        private readonly Predicate<T> predicate;

        public Constraint(Predicate<T> predicate, object errorMessageKey)
        {
            this.predicate = predicate;
            ErrorMessageKey = errorMessageKey;
        }

        public object ErrorMessageKey { get; }

        public bool Fulfilled(T t)
        {
            return predicate(t);
        }
    }
}
