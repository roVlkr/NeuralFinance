using System;
using System.Collections.Generic;
using System.Text;
using NeuralNetworks;

namespace NeuralFinance.ViewModel
{
    public enum SystemState
    {
        NotInitialized,
        SystemInitialized,
        DataInitialized
    }

    public class NeuralSystem : ViewModelBase
    {
        private Net network;
        private Training training;
        private SystemState systemState;

        public NeuralSystem()
        {
            SystemState = SystemState.NotInitialized;
        }

        public Net Network
        {
            get => network;
            set
            {
                network = value;
                OnPropertyChanged();

                if (SystemInitialized)
                    SystemState = SystemState.SystemInitialized;
            }
        }

        public Training Training
        {
            get => training;
            set
            {
                training = value;
                OnPropertyChanged();

                if (SystemInitialized)
                    SystemState = SystemState.SystemInitialized;
            }
        }

        public SystemState SystemState
        {
            get => systemState;
            set
            {
                systemState = value;
                OnPropertyChanged();
            }
        }

        private bool SystemInitialized => Network != null && Training != null;
        private bool DataInitialized => Training.Patterns != null && Training.ValidationPatterns != null;

        public void UpdateTrainingData(List<TrainingPattern> patterns)
        {
            Training.Patterns = patterns;
            if (DataInitialized) SystemState = SystemState.DataInitialized;
        }

        public void UpdateValidationData(List<TrainingPattern> patterns)
        {
            Training.ValidationPatterns = patterns;
            if (DataInitialized) SystemState = SystemState.DataInitialized;
        }
    }
}
