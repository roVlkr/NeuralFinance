using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralFinance.ViewModel.Helper
{
    public class TypeDependencyWrapper : DependencyObject
    {
        public static readonly DependencyProperty TypeProperty =
           DependencyProperty.Register("Type", typeof(Type), typeof(TypeDependencyWrapper),
               new PropertyMetadata(typeof(void)));

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
    }
}
