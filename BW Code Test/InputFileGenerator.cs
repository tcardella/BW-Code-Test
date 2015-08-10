using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BW_Code_Test
{
    public class InputFileGenerator
    {
        public string GenerateInputFile(int count)
        {
            var tempFilePath = Path.GetTempFileName();
            var availableValues = Enumerable.Range(0, (int) (count + (count*.05))).ToList();

            var random = new Random(DateTime.UtcNow.Millisecond);

            var contents = new List<string>();

            for (var i = 0; i < count; i++)
            {
                var j = random.Next(availableValues.Count);
                contents.Add(availableValues[j].ToString());
                availableValues.RemoveAt(j);
            }

            File.WriteAllLines(tempFilePath, contents);

            return tempFilePath;
        }
    }
}