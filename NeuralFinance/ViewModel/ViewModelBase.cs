using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralFinance.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected IDictionary<string, IList<string>> errors;

        public ViewModelBase()
        {
            errors = new Dictionary<string, IList<string>>();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
                return errors[propertyName];

            return null;
        }

        public bool HasErrors => errors.Values.Any(errorList => errorList.Count > 0);

        #region HelperFunctions
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ObserveConstraint(bool constraint, object errorMessageKey,
            [CallerMemberName] string propertyName = null)
        {
            string errorMessage = GetAppErrorMessage(errorMessageKey);

            if (constraint)
            {
                if (errors.ContainsKey(propertyName))
                {
                    if (errors[propertyName].Contains(errorMessage))
                    {
                        errors[propertyName].Remove(errorMessage);
                        OnErrorsChanged(propertyName);
                    }
                }                
            }
            else
            {
                if (errors.ContainsKey(propertyName))
                {
                    if (!errors[propertyName].Contains(errorMessage))
                    {
                        errors[propertyName].Add(errorMessage);
                        OnErrorsChanged(propertyName);
                    }
                }
                else
                {
                    errors.Add(propertyName, new List<string> { errorMessage });
                    OnErrorsChanged(propertyName);
                }
            }
        }

        protected string GetAppErrorMessage(object key)
        {
            return (string)Application.Current.TryFindResource(key);
        }
        #endregion
    }
}
