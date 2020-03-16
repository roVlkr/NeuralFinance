using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public abstract class TrainingFactory
    {
        protected abstract Training CreateTraining(TrainingFactoryArgs args);

        public Training GetTraining(TrainingFactoryArgs args)
        {
            return CreateTraining(args);
        }

        public static TrainingFactory FromType(TrainingTypes type)
        {
            switch (type)
            {
                case TrainingTypes.Adam:
                    return new AdamTrainingFactory();
                case TrainingTypes.RMSProp:
                    return new RMSPropTrainingFactory();
                case TrainingTypes.RPROP:
                    return new RPROPTrainingFactory();
                case TrainingTypes.Standard:
                    return new StandardTrainingFactory();
                default:
                    return null;
            }
        }
    }
}
