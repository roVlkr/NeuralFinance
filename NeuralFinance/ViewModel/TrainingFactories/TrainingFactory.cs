using NeuralNetworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public abstract class TrainingFactory : ViewModelBase
    {
        public abstract string Name { get; }

        public abstract Training CreateTraining();
    }
}
