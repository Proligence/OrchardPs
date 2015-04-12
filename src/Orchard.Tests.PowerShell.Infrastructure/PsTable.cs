namespace Orchard.Tests.PowerShell.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Parses tables printed to the console by PowerShell grid view.
    /// </summary>
    /// <remarks>
    /// The purpose of this class is to provide an easy way to parse and validate output returned by the PowerShell
    /// engine in unit test code.
    /// </remarks>
    public class PsTable
    {
        public string[] Header { get; private set; }
        public IList<string[]> Rows { get; private set; }

        public string this[int rowIndex, int columnIndex]
        {
            get
            {
                if ((rowIndex >= 0) && (rowIndex < this.Rows.Count))
                {
                    if ((columnIndex >= 0) && (columnIndex < this.Header.Length))
                    {
                        return this.Rows[rowIndex][columnIndex];
                    }
                }

                return null;
            }
        }

        public string this[int rowIndex, string columnName]
        {
            get
            {
                int columnIndex = Array.IndexOf(this.Header, columnName);
                if (columnIndex >= 0)
                {
                    return this[rowIndex, columnIndex];
                }

                return null;
            }
        }

        public static PsTable Parse(string output)
        {
            if (string.IsNullOrEmpty(output))
            {
                return null;
            }

            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 3)
            {
                return null;
            }

            var rows = new List<string[]>();
            var colWidths = GetTableColumnWidths(lines[1]);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 1)
                {
                    continue;
                }

                var row = new string[colWidths.Length];
                for (int j = 0; j < colWidths.Length; j++)
                {
                    if (j == colWidths.Length - 1)
                    {
                        row[j] = lines[i].Substring(colWidths[j]).Trim();
                    }
                    else
                    {
                        row[j] = lines[i].Substring(colWidths[j], colWidths[j + 1] - colWidths[j]).Trim();
                    }
                }

                rows.Add(row);
            }

            return new PsTable
            {
                Header = rows[0],
                Rows = rows.Skip(1).ToList()
            };
        }

        private static int[] GetTableColumnWidths(string headerLine)
        {
            bool newColumn = true;

            var cols = new List<int>();
            for (int i = 0; i < headerLine.Length; i++)
            {
                if (newColumn && (headerLine[i] == '-'))
                {
                    cols.Add(i);
                    newColumn = false;
                }
                else if (char.IsWhiteSpace(headerLine[i]))
                {
                    newColumn = true;
                }
            }

            return cols.ToArray();
        }
    }
}