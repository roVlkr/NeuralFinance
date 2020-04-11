using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class RMSPropTrainingFactory : TrainingFactory
    {
        private int batchSize;
        private double learningRate;

        public RMSPropTrainingFactory()
        {
            BatchSize = 100;
            LearningRate = 0.02;
        }

        public override string Name => "RMSProp";

        public int BatchSize
        {
            get => batchSize;
            set
            {
                batchSize = value;
                OnPropertyChanged();
                ObserveConstraint(value > 0, "errorMessageGreaterZero");
            }
        }

        public double LearningRate
        {
            get => learningRate;
            set
            {
                learningRate = value;
                OnPropertyChanged();
                ObserveConstraint(value > 0, "errorMessageGreaterZero");
            }
        }

        public override Training CreateTraining()
        {
            if (!HasErrors)
                return new RMSPropTraining(App.Network, BatchSize, LearningRate);
            else
                return null;
        }
    }
}
