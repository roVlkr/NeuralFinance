using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralFinance.ViewModel.Data
{
    public class ChartTable
    {
        public ChartTable()
        {
            Data = new SortedDictionary<DateTime, Dictionary<string, double>>();
        }

        public SortedDictionary<DateTime, Dictionary<string, double>> Data { get; set; }

        public List<string> ColumnNames { get; set; }

        public List<string> ValueColumnNames { get; set; }

        public int Columns { get => ColumnNames.Count; }

        public int Rows { get => Data.Count; }

        public string StockName { get; set; }

        public string WKN { get; set; }

        public string Exchange { get; set; }

        public static ChartTable operator +(ChartTable t1, ChartTable t2)
        {
            // First compare the tables and abort if they are not mergable
            if (t1.Columns != t2.Columns)
            {
                throw new Exception("Die Tabellen haben nicht dieselben Dimensionen.");
            }

            if (t1.StockName != t2.StockName || t1.WKN != t2.WKN || t1.Exchange != t2.Exchange)
            {
                throw new Exception("Die Tabellen haben nicht dieselbe Bezeichnung.");
            }

            for (int i = 0; i < t1.Columns; i++)
            {
                if (t1.ColumnNames[i] != t2.ColumnNames[i])
                    throw new Exception("Die Tabellen haben nicht dieselben Spalten.");
            }

            // Now merge the tables
            ChartTable merged = new ChartTable
            {
                ColumnNames = new List<string>(t1.ColumnNames),
                ValueColumnNames = new List<string>(t1.ValueColumnNames),
                StockName = t1.StockName,
                WKN = t1.WKN,
                Exchange = t1.Exchange
            };

            merged.Data = new SortedDictionary<DateTime, Dictionary<string, double>>(
                          t1.Data.Concat(from entry in t2.Data
                                         where !t1.Data.ContainsKey(entry.Key)  // No duplicate keys
                                         select entry)
                          .ToDictionary(entry => entry.Key, entry => entry.Value));

            return merged;
        }
    }
}
