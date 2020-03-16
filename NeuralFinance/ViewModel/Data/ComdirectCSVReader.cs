using System;
using System.Text;
using System.Collections.Generic;

namespace NeuralFinance.ViewModel.Data
{
    public class ComdirectCSVReader : CSVReader
    {
        public ComdirectCSVReader(string file) : base(file, Encoding.UTF7)
        { }

        protected override void ReadTableRow(ChartTable table, string line)
        {
            line = line.Replace("\"", ""); // Remove all quotes

            var content = line.Split(';');
            var row = new Dictionary<string, double>();
            string date = null;
            string time = null;

            // content.Length <= columnNames.Length
            for (int i = 0; i < content.Length; i++)
            {
                switch (table.ColumnNames[i])
                {
                    case "Datum":
                        date = content[i];
                        break;
                    case "Zeit":
                        time = content[i];
                        break;
                    default:
                        double value;

                        try
                        {
                            value = double.Parse(content[i]);
                        }
                        catch (FormatException)
                        {
                            value = 0;
                        }
                        
                        row.Add(table.ColumnNames[i], value);
                        break;
                }
            }

            var dateTime = ReadGermanDateTime(date, time);

            try
            {
                table.Data.Add(dateTime, row);
            }
            catch (ArgumentException)
            { /* No double entries */ }            
        }

        protected override void ReadTableHeadline(ChartTable table, string line)
        {
            // Remove all quotes
            line = line.Replace("\"", "");

            table.ColumnNames = new List<string>();
            table.ValueColumnNames = new List<string>();
            var columnNames = line.Split(';');

            foreach (var columnName in columnNames)
            {
                table.ColumnNames.Add(columnName);

                if (columnName != "Datum" && columnName != "Zeit")
                {
                    table.ValueColumnNames.Add(columnName);
                }
            }
        }

        protected override void ReadTableTitle(ChartTable table, string line)
        {
            // Remove all quotes
            line = line.Replace("\"", "");

            // Format: "Name (WKN: xxx Börse: xxx)"
            int paranthese = line.IndexOf('(');
            int wkn = line.IndexOf("WKN:");
            int boerse = line.IndexOf("Börse:");

            table.StockName = line.Substring(0, paranthese).Trim();
            table.WKN = line.Substring(wkn + 4, boerse - wkn - 4).Trim();
            table.Exchange = line.Substring(boerse + 6).Replace(")", "").Trim();
        }

        private DateTime ReadGermanDateTime(string date, string time)
        {
            // For readability: Introduce new variables
            int year, month, day;
            int hour = 0, minute = 0, second = 0;

            ///////////////////////////////////////////
            // Date for example 13.01.2020
            var dateParts = new List<int>();

            foreach (string s in date.Split('.'))
                dateParts.Add(int.Parse(s));

            year = dateParts[2];
            month = dateParts[1];
            day = dateParts[0];

            ///////////////////////////////////////////
            if (time != null)
            {
                // Time for example 12:30:10 OR 12:30
                var timeParts = new List<int>();

                foreach (string s in time.Split(':'))
                    timeParts.Add(int.Parse(s));

                hour = timeParts[0];
                minute = timeParts[1];
                second = (timeParts.Count == 3) ? timeParts[2] : 0;
            }

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
