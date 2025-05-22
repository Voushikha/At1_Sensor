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
            double average = Singleton.Instance.CalculateAverage(currentFileIndex);
            Average_txtBox.Text = average > 0 ? average.ToString("F2") : "N/A";
        }



        #region Load & Save 
        // USE SINGLETON

        //Load method
        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "CSV and Binary Files|*.csv;*.bin",
                Multiselect = true
            };

            if (openFile.ShowDialog() == true)
            {
                csvFiles.Clear();
                csvFiles.AddRange(openFile.FileNames);

                if (csvFiles.Count > 0)
                {
                    currentFileIndex = 0;
                    LoadFileAtIndex(currentFileIndex);
                }
            }
        }
        //. Separate File Loading Logic
        //Move the file loading logic into a dedicated method to handle CSV and binary file loading.
        // This method loads a file (either CSV or BIN format) into a 2D array
        // and returns a DataView which is used to display the data in the DataGrid.
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

            // If the file type is not supported, show an error
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

                // Update the average value shown in the text box
                CalculateAverage();

                // Update the label to show which file is currently viewed
                UpdateSampleLabel(filePath);

                // Color the cells based on the sensor value (green/red/blue)
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





        //private List<string> loadedFiles = new List<string>(); // List to store loaded file paths

        //        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        //        {
        //            OpenFileDialog openFile = new OpenFileDialog
        //            {
        //                Filter = "CSV and Binary Files|*.csv;*.bin",
        //                Multiselect = true
        //            };

        //            if (openFile.ShowDialog() == true)
        //            {
        //                csvFiles = new List<string>(openFile.FileNames);
        //                currentFileIndex = 0;

        //                string filePath = csvFiles[currentFileIndex];
        //                string extension = System.IO.Path.GetExtension(filePath).ToLower();

        //                DataView dataView = null;
        //                double[,] sensorArray = Singleton.Instance.SensorArray ?? new double[0, 0]; // Create a local variable  

        //                if (extension == ".csv")
        //                {
        //                    dataView = new FileLoader().LoadCsv(filePath, ref sensorArray);
        //                }
        //                else if (extension == ".bin")
        //                {
        //                    dataView = new FileLoader().LoadBin(filePath, ref sensorArray);
        //                }

        //                Singleton.Instance.SensorArray = sensorArray; // Update the Singleton property  

        //                if (dataView != null)
        //                {
        //                    DataGrid.ItemsSource = dataView;
        //                    CalculateAverage();
        //                    UpdateSampleLabel(filePath);
        //                    ApplyCellColoring();

        //                    // Add the loaded file to the list
        //                    if (!loadedFiles.Contains(filePath))
        //                    {
        //                        loadedFiles.Add(filePath);
        //                    }
        //                }
        //            }
        //        }


        //   private void Btn_Load_Click(object sender, RoutedEventArgs e)
        //   {
        //       OpenFileDialog openFile = new OpenFileDialog
        //       {
        //           Filter = "CSV and Binary Files|*.csv;*.bin",
        //           Multiselect = true // Allow multiple file selection
        //       };

        //       if (openFile.ShowDialog() == true)
        //       {
        //           // Pass the csvFiles collection by reference to update it
        //           LoadFiles(openFile.FileNames, ref csvFiles);

        //           // Load the first file in the collection (if any)
        //           if (csvFiles.Count > 0)
        //           {
        //               currentFileIndex = 0;
        //               LoadFileAtIndex(currentFileIndex);
        //           }
        //       }
        //   }
        ////  Singleton.instances.Sensors.Clear();
        //   private void LoadFiles(string[] filePaths, ref List<string> fileCollection)
        //   {
        //       // Clear the existing collection and add the new file paths
        //       fileCollection.Clear();
        //       fileCollection.AddRange(filePaths);
        //   }



        // too many function in one 
        // simplify it 
        // make sure that it snot overrideing the old data 
        // the load data is too complicated, diiferent function on one thing 

        //try separationg file loading logic 
        //private void LoadFileAtIndex(int index)
        //{
        //    if (index >= 0 && index < csvFiles.Count)
        //    {
        //        string filePath = csvFiles[index];
        //        string extension = System.IO.Path.GetExtension(filePath).ToLower();

        //        DataView dataView = null;
        //        double[,] sensorArray = new double[0, 0]; // Just initialize a new local array// New array for each file


        //        if (extension == ".csv")
        //        {
        //            dataView = new FileLoader().LoadCsv(filePath, ref sensorArray);
        //        }
        //        else if (extension == ".bin")
        //        {
        //            dataView = new FileLoader().LoadBin(filePath, ref sensorArray);
        //        }

        //        var sensor = new Sensor
        //        {
        //            //Label = System.IO.Path.GetFileName(filePath),
        //            Label = System.IO.Path.GetFileName(filePath).Trim().ToLowerInvariant(),

        //            Data = sensorArray
        //        };

        //        if (Singleton.Instance.Sensors.Count > index)
        //        {
        //            Singleton.Instance.Sensors[index] = sensor;// Overwrites existing data // wrong!

        //        }
        //        else
        //        {
        //            Singleton.Instance.Sensors.Add(sensor);// Adds new data if index is out of bounds

        //        }

        //        if (dataView != null)
        //        {
        //            DataGrid.ItemsSource = dataView;
        //            CalculateAverage();
        //            UpdateSampleLabel(filePath);
        //            ApplyCellColoring();
        //        }
        //    }
        //}

        #region Try separating file loading logic, to avoid overriding the old data



        #endregion


        #region Load csv bin
        //LoaD FOR csv only
        //private DataView LoadCsv(string filePath)
        //{
        //    // Implement the CSV loading logic here  
        //    // Example:  
        //    DataTable dataTable = new DataTable();
        //    using (var reader = new StreamReader(filePath))
        //    {
        //        string[] headers = reader.ReadLine().Split(',');
        //        foreach (string header in headers)
        //        {
        //            dataTable.Columns.Add(header);
        //        }

        //        while (!reader.EndOfStream)
        //        {
        //            string[] rows = reader.ReadLine().Split(',');
        //            dataTable.Rows.Add(rows);
        //        }
        //    }
        //    return dataTable.DefaultView;
        //}

        //private DataView LoadBin(string filePath)
        //{
        //    // Implement the binary file loading logic here  
        //    // Example:  
        //    DataTable dataTable = new DataTable();
        //    using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        //    {
        //        int rowCount = reader.ReadInt32();
        //        int colCount = reader.ReadInt32();

        //        for (int i = 0; i < colCount; i++)
        //        {
        //            dataTable.Columns.Add($"Column{i + 1}");
        //        }

        //        for (int i = 0; i < rowCount; i++)
        //        {
        //            object[] row = new object[colCount];
        //            for (int j = 0; j < colCount; j++)
        //            {
        //                row[j] = reader.ReadDouble();
        //            }
        //            dataTable.Rows.Add(row);
        //        }
        //    }
        //    return dataTable.DefaultView;
        //}

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
                        writer.WriteLine(string.Join(",", fields));
                    }
                }

                MessageBox.Show("Data saved successfully.");
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
            // Check if we are not already at the last file
            if (currentFileIndex < csvFiles.Count - 1)
            {
                // Move to the next file by increasing the index
                currentFileIndex++;

                // Load the file at the new index and update the view
                LoadFileAtIndex(currentFileIndex);
            }
            else
            {
                // If we're already at the last file, show a message to the user
                MessageBox.Show("You are viewing the last file.");
            }
        }

        
        private void Btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            // Check if we are not already at the first file
            if (currentFileIndex > 0)
            {
                // Move to the previous file by decreasing the index
                currentFileIndex--;

                // Load the file at the new index and update the view
                LoadFileAtIndex(currentFileIndex);
            }
            else
            {
                // If we're already at the first file, show a message to the user
                MessageBox.Show("You are viewing the first file.");
            }
        }


        #endregion
        #region Binary Search 
        //Binary Search 
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

        //private void BinarySearch_Btn_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!double.TryParse(BinarySearch_txtBox.Text, out double target))
        //    {
        //        MessageBox.Show("Please enter a valid number to search.");
        //        return;
        //    }

        //    bool matchFound = false;

        //    DataGrid.UpdateLayout(); // Refresh 

        //    foreach (var item in DataGrid.Items)
        //    {
        //        if (DataGrid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
        //        {
        //            foreach (DataGridCell cell in GetCells(row))
        //            {
        //                if (cell.Content is TextBlock textBlock && double.TryParse(textBlock.Text, out double value))
        //                {
        //                    if (value == target)
        //                    {
        //                        cell.Background = Brushes.Yellow;
        //                        matchFound = true;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (!matchFound)
        //    {
        //        MessageBox.Show($"Value {target} not found.");
        //    }
        //}
        private void BinarySearch_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(BinarySearch_txtBox.Text, out double target))
            {
                MessageBox.Show("Please enter a valid number to search.");
                return;
            }

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

            // Sort the list before performing binary search  
            sortedList = sortedList.OrderBy(x => x.Load).ToList();

            // Perform binary search  
            int foundIndex = BinarySearch(sortedList, target);

            if (foundIndex != -1)
            {
                // Highlight the matching cell in the DataGrid  
                   DataGrid.UpdateLayout(); // Refresh  

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
            string targetLabel = SearchLabelTextBox?.Text?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(targetLabel))
            {
                MessageBox.Show("Please enter a label to search.");
                return;
            }

            // Extract labels from sensors and sort them  
            var sensors = Singleton.Instance.Sensors;
            var sortedSensors = sensors.OrderBy(s => s.Label).ToList();

            // Perform binary search on sorted labels  
            int foundIndex = BinarySearchByLabel(sortedSensors, targetLabel);

            if (foundIndex >= 0 && foundIndex < sortedSensors.Count)
            {
                currentFileIndex = sensors.IndexOf(sortedSensors[foundIndex]);
                LoadFileAtIndex(currentFileIndex);
            }
            else
            {
                MessageBox.Show($"No dataset found with label: {targetLabel}");
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


        //Save
        #region Traffic LIghts
        private void Bounds_TextChanged(object sender, TextChangedEventArgs e)
        {
            double.TryParse(LB_txtBox.Text, out lowerBound);
            double.TryParse(UB_txtBox.Text, out upperBound);

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
        // should be able to search the dataset as well on the same textbox that is used to search for valuees

        //make the color choice it part of the cell ,
        //try adding a button





    }
}
