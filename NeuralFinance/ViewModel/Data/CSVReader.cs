using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeuralFinance.ViewModel.Data
{
    public abstract class CSVReader : StreamReader
    {
        public CSVReader(string file) : base(file)
        {
            File = file;
        }

        public CSVReader(string file, Encoding encoding) : base(file, encoding)
        {
            File = file;
        }

        public string File { get; set; }

        public ChartTable ReadTable()
        {
            ChartTable table = new ChartTable();
            bool headline = true;

            // Read the very first line
            if (!EndOfStream) ReadTableTitle(table, ReadLine());
            
            while (!EndOfStream)
            {
                var line = ReadLine();

                // Then it doesn't contain table content
                if (!line.Contains(";"))
                    continue;

                if (headline)
                {
                    ReadTableHeadline(table, line);
                    headline = false;
                }                    
                else
                    ReadTableRow(table, line);
            }

            return table;
        }

        protected abstract void ReadTableTitle(ChartTable table, string line);

        protected abstract void ReadTableHeadline(ChartTable table, string line);

        protected abstract void ReadTableRow(ChartTable table, string line);
    }
}
