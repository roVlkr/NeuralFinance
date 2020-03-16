using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class RPROPTrainingFactory : TrainingFactory
    {
        protected override Training CreateTraining(TrainingFactoryArgs args)
        {
            if (args.Name == TrainingTypes.RPROP)
            {
                var template = TrainingFactoryArgs.Templates[args.Name];
                var decreaseFactorKey = template.Parameters[0].Name;
                var increaseFactorKey = template.Parameters[1].Name;

                return new RPROPTraining(App.Network, (double)args[decreaseFactorKey], (double)args[increaseFactorKey]);
            }

            return null;
        }
    }
}
