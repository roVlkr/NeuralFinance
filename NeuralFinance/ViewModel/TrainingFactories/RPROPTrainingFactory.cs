using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class RPropTrainingFactory : TrainingFactory
    {
        private double decreaseFactor;
        private double increaseFactor;

        public RPropTrainingFactory()
        {
            DecreaseFactor = 0.5;
            IncreaseFactor = 1.2;
        }

        public override string Name => "RProp";

        public double DecreaseFactor
        {
            get => decreaseFactor;
            set
            {
                decreaseFactor = value;
                OnPropertyChanged();
                ObserveConstraint(value > 0, "errorMessageGreaterZero");
                ObserveConstraint(value < 1, "errorMessageLessOne");
            }
        }

        public double IncreaseFactor
        {
            get => increaseFactor;
            set
            {
                increaseFactor = value;
                OnPropertyChanged();
                ObserveConstraint(value > 1, "errorMessageGreaterOne");
            }
        }

        public override Training CreateTraining()
        {
            if (!HasErrors)
                return new RPROPTraining(App.Network, DecreaseFactor, IncreaseFactor);
            else
                return null;
        }
    }
}
