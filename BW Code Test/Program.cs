using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BW_Code_Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var inputFilePath = args[0];
            var errorsFilePath = Path.Combine(Environment.CurrentDirectory, "Errors.txt");

            using (var fs = File.OpenWrite(errorsFilePath))
            {
                using (var writer = new StreamWriter(fs))
                {
                    // get the lines from the input file
                    var source = File.ReadAllLines(inputFilePath);

                    // get total number of lines in the file
                    var totalNumberOfLines = source.Count();

                    // project each line into a key value pair where the key is the line number and the value is the line read in from the file
                    var inputValuesWithLineNumbers = source.AsEnumerable()
                        .Select((inputValue, index) => new KeyValuePair<int, string>(index + 1, inputValue));

                    // filter out invalid values
                    var inputIntegersWithLineNumbers = inputValuesWithLineNumbers.FilterInvalidIntegers(totalNumberOfLines, writer).ToArray();

                    // using the remaining valid values, find the duplicates
                    var duplicateIntegers = from e in inputIntegersWithLineNumbers
                        group e by e.Value
                        into g
                        where g.Count() > 1
                        select g;

                    // flattening the duplicates
                    var flattenedDuplicateIntegers = from e in duplicateIntegers
                        from f in e.Select(g => g)
                        select f;

                    // write out errors
                    foreach (var e in flattenedDuplicateIntegers)
                        writer.WriteLine("Line #" + e.Key + ": Duplicate integer found " + e.Value);

                    // get missing integers
                    var missingIntegers = Enumerable.Range(1, totalNumberOfLines)
                        .Except(inputIntegersWithLineNumbers.Select(e => e.Value));

                    // write out errors
                    foreach (var missingInteger in missingIntegers)
                        writer.WriteLine("Missing Integer: {0}", missingInteger);
                }
            }
        }
    }

    public static class ExtensionMethods
    {
        public static IEnumerable<KeyValuePair<int, int>> FilterInvalidIntegers(this IEnumerable<KeyValuePair<int, string>> inputs, int lineCount, StreamWriter writer)
        {
            foreach (var e in inputs)
            {
                int integerValue;

                // try to parse the string to an integer
                if (int.TryParse(e.Value, out integerValue))
                {
                    // lower bounds check
                    if (integerValue < 1)
                    {
                        writer.WriteLine("Line #{0}: {1} is less than 1.", e.Key, integerValue);
                        continue;
                    }

                    // upper bounds check
                    if (integerValue > lineCount)
                    {
                        writer.WriteLine("Line #{0}: {1} is greater than {2}.", e.Key, integerValue, lineCount);
                        continue;
                    }

                    // project the result into a line number and integer value kvp
                    yield return new KeyValuePair<int, int>(e.Key, integerValue);
                }
                else
                {
                    writer.WriteLine("Line #{0}: \"{1}\" is not a valid integer.", e.Key, e.Value);
                }
            }
        }
    }
}