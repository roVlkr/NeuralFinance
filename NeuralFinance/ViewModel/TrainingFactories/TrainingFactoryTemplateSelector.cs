using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class TrainingFactoryTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AdamTrainingFactory)
            {
                return (DataTemplate)Application.Current.FindResource("adamTrainingFactoryTemplate");
            }
            else if (item is RMSPropTrainingFactory)
            {
                return (DataTemplate)Application.Current.FindResource("rmspropTrainingFactoryTemplate");
            }
            else if (item is RPropTrainingFactory)
            {
                return (DataTemplate)Application.Current.FindResource("rpropTrainingFactoryTemplate");
            }
            else if (item is StandardTrainingFactory)
            {
                return (DataTemplate)Application.Current.FindResource("standardTrainingFactoryTemplate");
            }

            return base.SelectTemplate(item, container);
        }
    }
}
