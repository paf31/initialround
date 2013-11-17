using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InitialRound.Common.Helpers
{
    public static class CSVReader
    {
        public static IEnumerable<string[]> Read(string csv)
        {
            // Any extra blank lines at the end of the file should be removed
            csv = csv.TrimEnd('\n', '\r');

            bool insideQuote = false;

            IList<string> currentRow = new List<string>();
            StringBuilder currentCell = new StringBuilder();

            for (int i = 0; i < csv.Length; i++)
            {
                char c = csv[i];

                switch (c)
                {
                    case '"':
                        if (insideQuote)
                        {
                            if (i <= csv.Length - 2 && csv[i + 1] == '"')
                            {
                                i++;
                                currentCell.Append('"');
                            }
                            else
                            {
                                insideQuote = false;
                            }
                        }
                        else
                        {
                            insideQuote = true;
                        }
                        break;
                    case ',':
                        if (insideQuote)
                        {
                            currentCell.Append(',');
                        }
                        else
                        {
                            currentRow.Add(currentCell.ToString());
                            currentCell.Clear();
                        }
                        break;
                    case '\r':
                        if (insideQuote)
                        {
                            currentCell.Append(c);
                        }
                        break;
                    case '\n':
                        if (insideQuote)
                        {
                            currentCell.Append(c);
                        }
                        else
                        {
                            currentRow.Add(currentCell.ToString());
                            currentCell.Clear();

                            yield return currentRow.ToArray();
                            currentRow.Clear();
                        }
                        break;
                    default:
                        currentCell.Append(c);
                        break;
                }
            }

            currentRow.Add(currentCell.ToString());
            currentCell.Clear();

            yield return currentRow.ToArray();
        }
    }
}
