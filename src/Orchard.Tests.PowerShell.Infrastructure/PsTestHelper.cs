namespace Orchard.Tests.PowerShell.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public static class PsTestHelper
    {
        public static IList<string[]> ParseTable(string output)
        {
            var rows = new List<string[]>();

            if (string.IsNullOrEmpty(output))
            {
                return rows;
            }

            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 3)
            {
                return rows;
            }

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

            return rows;
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