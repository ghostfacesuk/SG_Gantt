using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using SG_Gantt.Models;
using SG_Gantt.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SG_Gantt.ViewModels
{
    public class GanttViewModel
    {
        // The PlotModel that the View will bind to
        public PlotModel GanttModel { get; private set; }

        // Constructor accepting a list of TaskItem objects
        public GanttViewModel(List<TaskItem> tasks)
        {
            // Initialize the PlotModel
            GanttModel = new PlotModel { Title = "Gantt Chart" };

            // Setup the axes
            SetupAxes(tasks);

            // Add the Gantt bars
            AddGanttSeries(tasks);

            // Add predecessor lines
            AddPredecessorAnnotations(tasks);
        }

        /// <summary>
        /// Configures the X and Y axes of the PlotModel.
        /// </summary>
        /// <param name="tasks">List of TaskItem objects.</param>
        private void SetupAxes(List<TaskItem> tasks)
        {
            // Define the date axis (X-axis)
            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "yyyy-MM-dd",
                Title = "Date",
                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days,
                IsZoomEnabled = true,
                IsPanEnabled = true,
                Minimum = DateTimeAxis.ToDouble(tasks.Min(t => t.Start).AddDays(-1)),
                Maximum = DateTimeAxis.ToDouble(tasks.Max(t => t.Finish).AddDays(1))
            };
            GanttModel.Axes.Add(dateAxis);

            // Define the category axis (Y-axis)
            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Left,
                Title = "Tasks",
                ItemsSource = tasks.OrderBy(t => t.OutlineLevel).ThenBy(t => t.ID),
                LabelField = "Name",
                GapWidth = 1
            };
            GanttModel.Axes.Add(categoryAxis);
        }

        /// <summary>
        /// Adds a RectangleBarSeries to represent tasks in the Gantt chart.
        /// </summary>
        /// <param name="tasks">List of TaskItem objects.</param>
        private void AddGanttSeries(List<TaskItem> tasks)
        {
            var ganttSeries = new RectangleBarSeries
            {
                StrokeColor = OxyColors.Black,
                StrokeThickness = 1,
                LabelPlacement = LabelPlacement.Inside,
                TrackerFormatString = "Task: {Label}\nStart: {X0:yyyy-MM-dd}\nFinish: {X1:yyyy-MM-dd}\n% Complete: {Label}"
            };

            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                var start = DateTimeAxis.ToDouble(task.Start);
                var end = DateTimeAxis.ToDouble(task.Finish);
                var y = i;

                ganttSeries.Items.Add(new RectangleBarItem(start, y - 0.4, end, y + 0.4)
                {
                    FillColor = GetOxyColor(task.ColorCode),
                    Label = $"{task.PercentComplete}%"
                });

                // Optionally, add a TextAnnotation for notes
                if (!string.IsNullOrWhiteSpace(task.Notes))
                {
                    var annotation = new TextAnnotation
                    {
                        Text = ConvertHtmlToPlainText(task.Notes),
                        Position = new DataPoint((start + end) / 2, y + 0.6), // Adjust position as needed
                        TextPosition = new DataPoint((start + end) / 2, y + 0.6),
                        Stroke = OxyColors.Transparent,
                        TextColor = OxyColors.Black,
                        FontSize = 12,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Background = OxyColors.White,
                        TextWrap = true,
                        MaximumWidth = 200,
                        // Optionally, set opacity or other styling
                    };

                    GanttModel.Annotations.Add(annotation);
                }
            }

            GanttModel.Series.Add(ganttSeries);
        }

        /// <summary>
        /// Adds LineAnnotations to represent task dependencies (predecessors).
        /// </summary>
        /// <param name="tasks">List of TaskItem objects.</param>
        private void AddPredecessorAnnotations(List<TaskItem> tasks)
        {
            foreach (var task in tasks)
            {
                foreach (var predId in task.PredecessorIDs)
                {
                    var predecessor = tasks.FirstOrDefault(t => t.ID == predId);
                    if (predecessor != null)
                    {
                        var x1 = DateTimeAxis.ToDouble(predecessor.Finish);
                        var y1 = tasks.IndexOf(predecessor);
                        var x2 = DateTimeAxis.ToDouble(task.Start);
                        var y2 = tasks.IndexOf(task);

                        var line = new LineAnnotation
                        {
                            Type = LineAnnotationType.Line,
                            X = x1,
                            Y = y1,
                            X2 = x2,
                            Y2 = y2,
                            Color = OxyColors.Gray,
                            StrokeThickness = 1,
                            Text = "FS",
                            TextOrientation = AnnotationTextOrientation.Horizontal
                        };

                        GanttModel.Annotations.Add(line);
                    }
                }
            }
        }

        /// <summary>
        /// Converts HTML content to plain text for tooltips or annotations.
        /// </summary>
        /// <param name="html">HTML string.</param>
        /// <returns>Plain text string.</returns>
        private string ConvertHtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Simple replacement; for more complex HTML, consider using HtmlAgilityPack or similar
            return html.Replace("<br>", "\n")
                       .Replace("<p>", "")
                       .Replace("</p>", "")
                       .Replace("&nbsp;", " ");
        }

        /// <summary>
        /// Parses a HEX color code and returns an OxyColor.
        /// Defaults to SkyBlue if parsing fails.
        /// </summary>
        /// <param name="colorCode">HEX color code string.</param>
        /// <returns>OxyColor object.</returns>
        private OxyColor GetOxyColor(string colorCode)
        {
            if (string.IsNullOrWhiteSpace(colorCode))
                return OxyColors.SkyBlue; // Default color

            try
            {
                return OxyColor.Parse(colorCode);
            }
            catch
            {
                return OxyColors.SkyBlue; // Fallback color
            }
        }
    }
}
