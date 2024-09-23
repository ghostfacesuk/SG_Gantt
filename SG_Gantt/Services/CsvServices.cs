using CsvHelper;
using CsvHelper.Configuration;
using SG_Gantt.Models;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SG_Gantt.Services
{
    public class CsvServices
    {
        /// <summary>
        /// Loads tasks from a CSV file.
        /// </summary>
        /// <param name="filePath">Path to the CSV file.</param>
        /// <returns>List of TaskItem objects.</returns>
        public List<TaskItem> LoadTasks(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV file not found.", filePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null, // Ignore missing fields
                HeaderValidated = null,
                Delimiter = ","
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<TaskItem>().ToList();

                // Parse Predecessors
                foreach (var task in records)
                {
                    task.PredecessorIDs = ParsePredecessors(task.Predecessors);
                }

                return records;
            }
        }

        /// <summary>
        /// Parses the Predecessors string to extract task IDs.
        /// </summary>
        /// <param name="predecessors">Predecessors string from CSV.</param>
        /// <returns>List of predecessor task IDs.</returns>
        private List<int> ParsePredecessors(string predecessors)
        {
            var predecessorIDs = new List<int>();

            if (string.IsNullOrWhiteSpace(predecessors))
                return predecessorIDs;

            // Split by comma if multiple predecessors
            var preds = predecessors.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pred in preds)
            {
                // Assuming format like "1FS", "2SS", etc.
                var idStr = new string(pred.TakeWhile(char.IsDigit).ToArray());

                if (int.TryParse(idStr, out int id))
                {
                    predecessorIDs.Add(id);
                }
            }

            return predecessorIDs;
        }
    }
}
