using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class StandardTrainingFactory : TrainingFactory
    {
        protected override Training CreateTraining(TrainingFactoryArgs args)
        {
            if (args.Name == TrainingTypes.Standard)
            {
                var template = TrainingFactoryArgs.Templates[args.Name];
                var learningRateKey = template.Parameters[0].Name;

                return new StandardTraining(App.Network, (double)args[learningRateKey]);
            }

            return null;
        }
    }
}
