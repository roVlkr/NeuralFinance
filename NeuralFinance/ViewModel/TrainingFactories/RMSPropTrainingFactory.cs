using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class RMSPropTrainingFactory : TrainingFactory
    {
        protected override Training CreateTraining(TrainingFactoryArgs args)
        {
            if (args.Name == TrainingTypes.RMSProp)
            {
                var template = TrainingFactoryArgs.Templates[args.Name];
                var batchSizeKey = template.Parameters[0].Name;
                var learningRateKey = template.Parameters[1].Name;

                return new RMSPropTraining(App.Network, (int)args[batchSizeKey], (double)args[learningRateKey]);
            }

            return null;
        }
    }
}
