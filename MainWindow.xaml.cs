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


namespace AT1_Sensor
{
    /// <summary>
    /// Main window for the AT1 Sensor application. Handles file loading, navigation, search, and UI updates.
    /// </summary>
    public partial class MainWindow : Window
    {

        // Lower and upper bounds for cell coloring in the DataGrid.
        private double lowerBound = 0;
        private double upperBound = 0;

        // List of loaded CSV file paths.
        private List<string> csvFiles = new List<string>(); 
        // Index of the current CSV file
        private int currentFileIndex = 0;



        // The value to highlight in the DataGrid, if any.
        private double? highlightTarget = null;
        /// <summary>
        /// Initializes the main window and sets up event handlers.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
           
           Trace.WriteLine("MainWindow initialized.");
            // Initialize the DataGrid and other UI components here
            DataGrid.ItemsSource = null; // Set to null initially
            DataGrid.CanUserSortColumns = false; // Disable sorting on columns
                                               
          // Use Loaded event to attach ScrollChanged listener
            DataGrid.Loaded += (s, e) =>
            {
                if (VisualTreeHelper.GetChild(DataGrid, 0) is Decorator border &&
                    border.Child is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollChanged += DataGrid_ScrollChanged;
                }
            };
        }


        public static double[,]? SensorArray;
       


        /// <summary>
        /// Calculates the average of the current file's sensor data and updates the Average_txtBox.
        /// </summary>
        private void CalculateAverage()
        {
            Trace.WriteLine(" Average Calculated");
            double average = Singleton.Instance.CalculateAverage(currentFileIndex);
            Average_txtBox.Text = average > 0 ? average.ToString("F2") : "N/A";
        }
        /// <summary>
        /// Handles the Load button click event. Opens a file dialog, loads selected files, and updates the UI.
        /// </summary>
        #region Load & Save      
        /// <summary>
        /// Handles the Load button click event. Opens a file dialog, loads selected files, and updates the UI.
        /// </summary>
        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "CSV and Binary Files|*.csv;*.bin",
                Multiselect = true
            };

            if (openFile.ShowDialog() == true)
            {
                Trace.WriteLine("File dialog opened");

                csvFiles.Clear();
                csvFiles.AddRange(openFile.FileNames);
                Singleton.Instance.Sensors.Clear(); // Clear existing for fresh load

                if (csvFiles.Count > 0)
                {
                    currentFileIndex = 0;

                    //  Load all selected files into Singleton for label searching
                    for (int i = 0; i < csvFiles.Count; i++)
                    {
                        string filePath = csvFiles[i];
                        string fileName = System.IO.Path.GetFileName(filePath).Trim().ToLowerInvariant();

                        // Check if this file has already been added to the sensor list (avoid duplicates)
                        if (!Singleton.Instance.Sensors.Any(s => s.Label == fileName))
                        {
                            // Initialize a new 2D array to hold sensor data for this file
                            double[,] sensorArray = new double[0, 0];

                            // Load the sensor data from the file into the array
                            LoadFile(filePath, ref sensorArray);

                            // Add a new Sensor object to the Singleton list with the label and data
                            Singleton.Instance.Sensors.Add(new Sensor
                            {
                                Label = fileName,
                                Data = sensorArray
                            });
                        }
                    }
                    // Sort sensor list after loading
                    Singleton.Instance.SortSensorsByLabel();
                    LoadFileAtIndex(0);
                    Trace.WriteLine("Files loaded successfully");
                }
            }
        }

        /// <summary>
        /// Loads sensor data from a file into a 2D array and returns a DataView for display.
        /// </summary>
        private DataView LoadFile(string filePath, ref double[,] sensorArray)
        {
            return new FileLoader().Load(filePath, ref sensorArray);
        }


        /// <summary>
        /// Loads a file at the specified index and updates the DataGrid and related UI.
        /// </summary>
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
    
                // Apply color indicators right after file is loaded
                colorIndicators.ApplyColors(DataGrid, lowerBound, upperBound, highlightTarget);

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


        /// <summary>
        /// Save method
        /// Handles the Save button click event. Saves the current DataGrid data to a CSV file.
        /// </summary>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "output.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                if (DataGrid.ItemsSource is DataView dataView)
                {
                    new FileSaver().Save(saveFileDialog.FileName, dataView);
                }
            }

        }

        /// <summary>
        /// Updates the sample label to show the currently viewed file.
        /// </summary>
        private void UpdateSampleLabel(string fileName)
        {
            //$label - "Currently Viewing: {fileName}"
            SampleLabel.Content = $" {System.IO.Path.GetFileName(fileName)}";
        }


        #endregion
        #region Navigation
        /// <summary>
        /// Handles the Next button click event. Navigates to the next file in the list.
        /// </summary>
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

        /// <summary>
        /// Handles the Previous button click event. Navigates to the previous file in the list.
        /// </summary>
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
        /// <summary>
        /// Handles the Binary Search button click event. Searches for a value in the DataGrid.
        /// </summary>
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
            //int foundIndex = BinarySearch(sortedList, target);
            int foundIndex = Singleton.Instance.BinarySearch(sortedList, target);

            Trace.WriteLine(foundIndex != -1
                ? $"Target value {target} found at index {foundIndex}."
                : $"Target value {target} not found.");

            if (foundIndex != -1)
            {
                // Highlight the matching cell in the DataGrid
                //
                //highlightTarget = target;
                HighlightAllMatches(target);
                DataGrid.UpdateLayout();

               
            }
            else
            {
                highlightTarget = null;
                MessageBox.Show($"Value {target} not found.");
            }
        }



        //Search label
        /// <summary>
        /// Handles the Search Label button click event. Searches for a dataset by label.
        /// </summary>
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

            int foundIndex = Singleton.Instance.SearchByLabel(targetLabel);

            if (foundIndex >= 0 && foundIndex < Singleton.Instance.Sensors.Count)
            {
                currentFileIndex = foundIndex;
                Trace.WriteLine($"Label '{targetLabel}' found at index {foundIndex}.");
                LoadFileAtIndex(currentFileIndex);
            }
            else
            {
                MessageBox.Show($"No dataset found with label: {targetLabel}");
                Trace.WriteLine($"SearchLabelButton_Click: Label '{targetLabel}' not found.");
            }
        }


        #endregion
        #region Traffic LIghts

        //this will enabe to color big dataset 
        /// <summary>
        /// Handles DataGrid scroll events to re-apply cell coloring and highlighting.
        /// </summary>
        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //ApplyCellColoring();
            //highlight search value even when scrolled down
            if (highlightTarget.HasValue)
            {
                HighlightAllMatches(highlightTarget.Value);
            }
        }
        /// <summary>
        /// Handles changes to the lower and upper bound text boxes and updates cell coloring.
        /// </summary>
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

            //ApplyCellColoring(); // refresh colors manually  
            colorIndicators.ApplyColors(DataGrid, lowerBound, upperBound, highlightTarget);
        }




        #endregion
        /// <summary>
        /// Highlights all cells in the DataGrid that match the target value.
        /// </summary>
        //highlight search value 
        private void HighlightAllMatches(double target)
        {
            highlightTarget = target;
            colorIndicators.ApplyColors(DataGrid, lowerBound, upperBound, highlightTarget);
        }



    }

}

