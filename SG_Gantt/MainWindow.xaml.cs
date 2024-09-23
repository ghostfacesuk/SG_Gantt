using Microsoft.Win32;
using SG_Gantt.Models;
using SG_Gantt.Services;
using SG_Gantt.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SG_Gantt
{
    public partial class MainWindow : Window
    {
        private CsvServices csvService;

        public MainWindow()
        {
            InitializeComponent();
            csvService = new CsvServices();
        }

        /// <summary>
        /// Event handler for opening a CSV file.
        /// </summary>
        private void OpenCsv_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select the CSV file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Select a CSV File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load tasks from the selected CSV file
                    List<TaskItem> tasks = csvService.LoadTasks(openFileDialog.FileName);

                    // Initialize the ViewModel with the loaded tasks
                    GanttViewModel viewModel = new GanttViewModel(tasks);

                    // Set the DataContext to the ViewModel
                    this.DataContext = viewModel;

                    MessageBox.Show("CSV file loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading CSV file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event handler for exiting the application.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
