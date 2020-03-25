using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NeuralFinance.ViewModel;
using NeuralNetworks;

namespace NeuralFinance
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static NeuralSystem NeuralSystem { get; }
        public static Net Network => NeuralSystem?.Network;
        public static Training Training => NeuralSystem?.Training;

        static App()
        {
            NeuralSystem = new NeuralSystem();
        }
    }
}

