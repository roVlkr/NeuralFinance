using NeuralNetworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class StandardTrainingFactory : TrainingFactory
    {
        private double learningRate;

        public StandardTrainingFactory()
        {
            LearningRate = 0.02;
        }

        public override string Name => "Standard";

        public double LearningRate
        {
            get => learningRate;
            set
            {
                learningRate = value;
                OnPropertyChanged();
                ObserveConstraint(value > 0, "errorMessageGreaterOne");
            }
        }

        public override Training CreateTraining()
        {
            if (!HasErrors)
                return new StandardTraining(App.Network, LearningRate);
            else
                return null;
        }
    }
}
