using Microsoft.Win32;
using System.Data;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;
using System.Diagnostics;
//using static System.Net.WebRequestMethods;


namespace AT1_Sensor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // try use traffic ilight system , for the files 
        //can use files or csv to load data

        // cOLUMNS DISABLE SORT //

        private double lowerBound = 0;
        private double upperBound = 0;

        /// <summary>
        private List<string> csvFiles = new List<string>(); // List to store paths of CSV files
        private int currentFileIndex = 0; // Index of the current CSV file

        /// </summary>


        public MainWindow()
        {
            InitializeComponent();
           
           Trace.WriteLine("MainWindow initialized.");
            // Initialize the DataGrid and other UI components here
            DataGrid.ItemsSource = null; // Set to null initially
            DataGrid.CanUserSortColumns = false; // Disable sorting on columns
          //  DataGrid.ScrollChanged += DataGrid_ScrollChanged; // Attach scroll event handler
        }



        // Model class
        public class GridData
        {
            //public string Grid { get; set; }
            public int Load { get; set; }
        }

        public static double[,]? SensorArray;
        // 2d array implme

        //changed avg method 

        //private void CalculateAverage()
        //{
        //    double sum = 0;
        //    int count = 0;

        //    foreach (DataRowView rowView in DataGrid.ItemsSource)
        //    {
        //        foreach (var item in rowView.Row.ItemArray)
        //        {
        //            if (int.TryParse(item.ToString(), out int value))
        //            {
        //                sum += value;
        //                count++;
        //            }
        //        }
        //    }

        //    if (count > 0)
        //    {
        //        double average = sum / count;
        //        Average_txtBox.Text = average.ToString("F2");
        //    }
        //    else
        //    {
        //        Average_txtBox.Text = "N/A";
        //    }
        //}
        private void CalculateAverage()
        {
            Trace.WriteLine(" Average Calculated");
            double average = Singleton.Instance.CalculateAverage(currentFileIndex);
            Average_txtBox.Text = average > 0 ? average.ToString("F2") : "N/A";
        }



        #region Load & Save 
        // USE SINGLETON

        //Load method
        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
           
            // Open a file dialog to select CSV files
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "CSV and Binary Files|*.csv;*.bin",
                Multiselect = true
            };
          
            if (openFile.ShowDialog() == true)
                Trace.WriteLine("File dialog opened");
            {
                csvFiles.Clear();
                csvFiles.AddRange(openFile.FileNames);

                if (csvFiles.Count > 0)
                {
                    currentFileIndex = 0;
                    LoadFileAtIndex(currentFileIndex);
                }
                Trace.WriteLine("Files loaded successfully");
            }
        }
        //. Separate File Loading Logic
        //Move the file loading logic into a dedicated method to handle CSV and binary file loading.
     
        private DataView LoadFile(string filePath, ref double[,] sensorArray)
        {
            // Get the file extension (e.g., ".csv" or ".bin")
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            // If the file is a CSV, use the FileLoader class to load it
            if (extension == ".csv")
                return new FileLoader().LoadCsv(filePath, ref sensorArray);

            // If the file is a BIN file, use the FileLoader class to load it
            else if (extension == ".bin")
                return new FileLoader().LoadBin(filePath, ref sensorArray);
 
            throw new NotSupportedException($"File type '{extension}' is not supported.");
        }

        // This method loads a file from the list based on its index and updates the UI with its data.
        private void LoadFileAtIndex(int index)
        {
            // Make sure the index is within the valid range of the file list
            if (index >= 0 && index < csvFiles.Count)
            {
                // Get the file path from the list
                string filePath = csvFiles[index];

                // Extract just the file name and convert it to lowercase (used as a unique label)
                string fileName = System.IO.Path.GetFileName(filePath).Trim().ToLowerInvariant();

                // Create a 2D array to hold the sensor data
                double[,] sensorArray = new double[0, 0];

                // Load the file and get the data view for the DataGrid
                DataView dataView = LoadFile(filePath, ref sensorArray);

                // Display the data in the DataGrid
                DataGrid.ItemsSource = dataView;

               

                UpdateSampleLabel(filePath);
                CalculateAverage();

                ApplyCellColoring();

                // If this file hasn’t already been added to the Singleton sensor list
                if (!Singleton.Instance.Sensors.Any(s => s.Label == fileName))
                {
                    // Add the sensor data to the Singleton list
                    Singleton.Instance.Sensors.Add(new Sensor
                    {
                        Label = fileName,
                        Data = sensorArray
                    });

                    // Sort the list of sensors by label for easier searching
                    Singleton.Instance.SortSensorsByLabel();
                }
               
            }
        }





  

        #region Try separating file loading logic, to avoid overriding the old data



        #endregion


     


        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "output.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                Save(saveFileDialog.FileName);
            }
           
        }


        private void Save(string filePath)
        {
            if (DataGrid.ItemsSource is DataView dataView)
            {
                using (var writer = new StreamWriter(filePath))
                {
                    // Write column headers
                    var columnNames = dataView.Table.Columns
                        .Cast<DataColumn>()
                        .Select(col => col.ColumnName);
                    writer.WriteLine(string.Join(",", columnNames));

                    // Write each row
                    foreach (DataRowView rowView in dataView)
                    {
                        var fields = rowView.Row.ItemArray
                            .Select(field => field.ToString());

                        Trace.WriteLine("data written to file.");
                        writer.WriteLine(string.Join(",", fields));
                        
                    }
                }

                MessageBox.Show("Data saved successfully.");
                Trace.WriteLine("File successfully saved");
            }
            else
            {
                MessageBox.Show("No data to save.");
            }
        }

        private void UpdateSampleLabel(string fileName)
        {
            //$label - "Currently Viewing: {fileName}"
            SampleLabel.Content = $" {System.IO.Path.GetFileName(fileName)}";
        }




        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Btn_Next_Click invoked.");

            // Check if we are not already at the last file  
            if (currentFileIndex < csvFiles.Count - 1)
            {
                // Move to the next file by increasing the index  
                currentFileIndex++;

                // Load the file at the new index and update the view  
                LoadFileAtIndex(currentFileIndex);
                Trace.WriteLine($"Moved to next file. Current index: {currentFileIndex}");
            }
            else
            {
                // If we're already at the last file, show a message to the user  
                MessageBox.Show("You are viewing the last file.");
                Trace.WriteLine("Already at the last file.");
            }
        }
   
        
private void Btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Btn_Previous_Click invoked."); 

            // Check if we are not already at the first file  
            if (currentFileIndex > 0)
            {
                // Move to the previous file by decreasing the index  
                currentFileIndex--;

                // Load the file at the new index and update the view  
                LoadFileAtIndex(currentFileIndex);
                Trace.WriteLine($"Moved to previous file. Current index: {currentFileIndex}"); 
            }
            else
            {
                // If we're already at the first file, show a message to the user  
                MessageBox.Show("You are viewing the first file.");
                Trace.WriteLine("Already at the first file."); 
            }
        }


        #endregion
        #region Binary Search 

        // Binary search method
        private int BinarySearch(List<GridData> sortedList, double target)
        {
            int low = 0;
            int high = sortedList.Count - 1;


            while (low <= high)

            {
                int mid = low + (high - low) / 2;
                if (sortedList[mid].Load == target)
                {
                    return mid; // Target found

                }
                else if (sortedList[mid].Load < target)



                {
                    low = mid + 1;


                }
                else


                {
                    high = mid - 1;


                }
            }

            return -1; // Target not found



        }
        private void BinarySearch_Btn_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("BinarySearch_Btn_Click invoked.");

            if (!double.TryParse(BinarySearch_txtBox.Text, out double target))
            {
                MessageBox.Show("Please enter a valid number to search.");
                Trace.WriteLine("Invalid input for binary search.");
                return;
            }

            Trace.WriteLine($"Targeted binary search value: {target}");

            // Extract data from DataGrid into a sorted list  
            var sortedList = new List<GridData>();
            if (DataGrid.ItemsSource is DataView dataView)
            {
                foreach (DataRowView rowView in dataView)
                {
                    foreach (var item in rowView.Row.ItemArray)
                    {
                        if (int.TryParse(item.ToString(), out int value))
                        {
                            sortedList.Add(new GridData { Load = value });
                        }
                    }
                }
            }

            Trace.WriteLine($"Extracted {sortedList.Count} items from DataGrid.");

            // Sort the list before performing binary search  
            sortedList = sortedList.OrderBy(x => x.Load).ToList();
            Trace.WriteLine("Sorted the list for binary search.");

            // Perform binary search  
            int foundIndex = BinarySearch(sortedList, target);
            Trace.WriteLine(foundIndex != -1
                ? $"Target value {target} found at index {foundIndex}."
                : $"Target value {target} not found.");

            if (foundIndex != -1)
            {
                // Highlight the matching cell in the DataGrid  
                DataGrid.UpdateLayout(); // Refresh  
                Trace.WriteLine("Highlighting matching cell in DataGrid.");

                foreach (var item in DataGrid.Items)
                {
                    if (DataGrid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                    {
                        foreach (DataGridCell cell in GetCells(row))
                        {
                            if (cell.Content is TextBlock textBlock && double.TryParse(textBlock.Text, out double value))
                            {
                                if (value == target)
                                {
                                    cell.Background = Brushes.Yellow;
                                    Trace.WriteLine($"Cell with value {value} highlighted.");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show($"Value {target} not found.");
            }
        }


//Search label
        private void SearchLabelButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("SearchLabelButton_Click invoked.");

            string targetLabel = SearchLabelTextBox?.Text?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(targetLabel))
            {
                MessageBox.Show("Please enter a label to search.");
                Trace.WriteLine("SearchLabelButton_Click: No label entered.");
                return;
            }

            Trace.WriteLine($"SearchLabelButton_Click: Searching for label '{targetLabel}'.");

            // Extract labels from sensors and sort them  
            var sensors = Singleton.Instance.Sensors;
            var sortedSensors = sensors.OrderBy(s => s.Label).ToList();

            // Perform binary search on sorted labels  
            int foundIndex = BinarySearchByLabel(sortedSensors, targetLabel);

            if (foundIndex >= 0 && foundIndex < sortedSensors.Count)
            {
                currentFileIndex = sensors.IndexOf(sortedSensors[foundIndex]);
                Trace.WriteLine($"SearchLabelButton_Click: Label '{targetLabel}' found at index {foundIndex}.");
                LoadFileAtIndex(currentFileIndex);
            }
            else
            {
                MessageBox.Show($"No dataset found with label: {targetLabel}");
                Trace.WriteLine($"SearchLabelButton_Click: Label '{targetLabel}' not found.");
            }
        }
        // sorting the data before performing a binary search
        private int BinarySearchByLabel(List<Sensor> sortedSensors, string targetLabel)
        {
            int low = 0;
            int high = sortedSensors.Count - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                int comparison = string.Compare(sortedSensors[mid].Label, targetLabel, StringComparison.OrdinalIgnoreCase);

                if (comparison == 0)
                {
                    return mid; // Target found  
                }
                else if (comparison < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }

            return -1; // Target not found  
        }




        #endregion

        //this will enabe to color big dataset 
        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ApplyCellColoring();
        }

        #region Traffic LIghts

        //Save
        private void Bounds_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(LB_txtBox.Text, out lowerBound))
            {
                Trace.WriteLine($"Lower bound set to: {lowerBound}");
            }

            if (double.TryParse(UB_txtBox.Text, out upperBound))
            {
                Trace.WriteLine($"Upper bound set to: {upperBound}");
            }

            ApplyCellColoring(); // refresh colors manually  
        }


        private IEnumerable<DataGridCell> GetCells(DataGridRow row)
        {
            var presenter = FindVisualChild<DataGridCellsPresenter>(row);

            if (presenter == null)
            {
                // Force the row to generate its visual tree
                row.ApplyTemplate();
                presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter == null)
                    yield break; // Still null, safely exit
            }

            for (int i = 0; i < DataGrid.Columns.Count; i++)
            {
                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);
                if (cell != null)
                    yield return cell;
            }
        }


        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject

        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T tChild)
                    return tChild;

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
        private void ApplyCellColoring()
        {
            if (DataGrid.ItemsSource is DataView dataView)
            {
                DataGrid.UpdateLayout(); // Ensures rows and cells are generated

                foreach (var item in DataGrid.Items)
                {
                    if (DataGrid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                    {
                        foreach (DataGridCell cell in GetCells(row))
                        {
                            if (cell.Content is TextBlock textBlock && double.TryParse(textBlock.Text, out double value))
                            {
                                if (value < lowerBound)
                                    cell.Background = Brushes.Blue;
                                else if (value > upperBound)
                                    cell.Background = Brushes.Red;
                                else
                                    cell.Background = Brushes.Green;

                                //    else if (value >= lowerBound && value <= upperBound)
                                //        cell.Background = Brushes.LightGreen;
                                //}
                            }
                        }
                    }
                }
            }

            #endregion

        }
        //sort dtaset based on names, then search, 
      
        //make the color choice it part of the cell ,
        //try adding a button





    }
}
