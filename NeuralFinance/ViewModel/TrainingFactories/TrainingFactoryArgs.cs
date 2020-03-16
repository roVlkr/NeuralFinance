using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralFinance.ViewModel.TrainingFactories
{
    public class TrainingFactoryArgs
    {
        #region Static Dictionary
        public static Dictionary<TrainingTypes, TrainingFactoryArgs> Templates { get; }

        static TrainingFactoryArgs()
        {
            Templates = new Dictionary<TrainingTypes, TrainingFactoryArgs>
            {
                { TrainingTypes.Adam, new TrainingFactoryArgs
                {
                    Name = TrainingTypes.Adam,
                    Parameters = new List<Parameter>
                    {
                        new Parameter(typeof(int), "BatchSize", 100, "Größe des Mini-Batch", Constraint.intGreaterZero),
                        new Parameter(typeof(double), "LearningRate", 0.001, "Lernrate", Constraint.doubleGreaterZero)
                    }
                } },
                { TrainingTypes.RMSProp, new TrainingFactoryArgs
                {
                    Name = TrainingTypes.RMSProp,
                    Parameters = new List<Parameter>
                    {
                        new Parameter(typeof(int), "BatchSize", 100, "Größe des Mini-Batch", Constraint.intGreaterZero),
                        new Parameter(typeof(double), "LearningRate", 0.001, "Lernrate", Constraint.doubleGreaterZero)
                    }
                } },
                { TrainingTypes.RPROP, new TrainingFactoryArgs
                {
                    Name = TrainingTypes.RPROP,
                    Parameters = new List<Parameter>
                    {
                        new Parameter(typeof(double), "DecreaseValue", 0.5, "Lernraten werden verringert um Faktor",
                            Constraint.doubleGreaterZero, Constraint.doubleLessOne),
                        new Parameter(typeof(double), "IncreaseValue", 1.05, "Lernraten werden erhöht um Faktor",
                            Constraint.doubleGreaterOne)
                    }
                } },
                { TrainingTypes.Standard, new TrainingFactoryArgs
                {
                    Name = TrainingTypes.Standard,
                    Parameters = new List<Parameter>
                    {
                        new Parameter(typeof(double), "LearningRate", 0.02, "Lernrate", Constraint.doubleGreaterZero)
                    }
                } }
            };
        }
        #endregion

        public TrainingTypes Name { get; set; }

        public List<Parameter> Parameters { get; set; }

        public object this[string s]
        {
            get => Parameters[Parameters.FindIndex(parameter => parameter.Name == s)].Value;
        }
    }
}