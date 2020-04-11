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
        private Network network;
        private Training training;
        private List<TrainingPattern> validationPatterns;
        private SystemState systemState;

        public NeuralSystem()
        {
            SystemState = SystemState.NotInitialized;
        }

        public Network Network
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

        public List<TrainingPattern> TrainingPatterns
        {
            get => Training.Patterns;
            set
            {
                Training.Patterns = value;
                OnPropertyChanged();

                if (DataInitialized)
                    SystemState = SystemState.DataInitialized;
            }
        }

        public List<TrainingPattern> ValidationPatterns
        {
            get => validationPatterns;
            set
            {
                validationPatterns = value;
                OnPropertyChanged();
                
                if (DataInitialized)
                    SystemState = SystemState.DataInitialized;
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
        private bool DataInitialized => Training.Patterns != null && ValidationPatterns != null;

        public void ResetTrainingProgress()
        {
            Network.Reset();
            Training.ResetNetwork(Network);
        }
    }
}
