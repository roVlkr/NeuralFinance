using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel
{
    public class Parameter : ValueWrapper
    {
        public Parameter(Type valueType, string name, object value,
            string displayName, params Constraint[] constraints)
            : base(valueType, value, constraints)
        {
            Name = name;
            DisplayName = displayName;
        }

        public Parameter(Type valueType, string name, object value, string displayName)
            : this(valueType, name, value, displayName, new Constraint(o => true, null))
        {
            // Has no constraint (always returns true)
        }

        public string Name { get; }

        public string DisplayName { get; }
    }
}
