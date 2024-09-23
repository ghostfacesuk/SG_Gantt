using System;
using System.Collections.Generic;

namespace SG_Gantt.Models
{
    public class TaskItem
    {
        // Hierarchical level of the task (for outline purposes)
        public int OutlineLevel { get; set; }

        // Unique identifier for the task
        public int ID { get; set; }

        // Name or description of the task
        public string Name { get; set; } = string.Empty;

        // Start date of the task
        public DateTime Start { get; set; }

        // End date of the task
        public DateTime Finish { get; set; }

        // Duration of the task in days
        public TimeSpan Duration => Finish - Start;

        // Completion percentage of the task
        public double PercentComplete { get; set; }

        // Predecessor tasks (e.g., "1FS" meaning Finish-to-Start with Task ID 1)
        public string Predecessors { get; set; } = string.Empty;

        // Parsed list of predecessor IDs
        public List<int> PredecessorIDs { get; set; } = new List<int>();

        // Name of the resource/person assigned to the task
        public string Resource { get; set; } = string.Empty;

        // HEX color code for the task bar (e.g., "#FF5733")
        public string ColorCode { get; set; } = "#FF5733";

        // Notes associated with the task (stored in HTML format)
        public string Notes { get; set; } = string.Empty;
    }
}
