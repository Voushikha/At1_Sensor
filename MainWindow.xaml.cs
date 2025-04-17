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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public static double[,] SensorArray; // 2d array implme

        //changed avg method 

        private void CalculateAverage()
        {
            double sum = 0;
            int count = 0;

            foreach (DataRowView rowView in DataGrid.ItemsSource)
            {
                foreach (var item in rowView.Row.ItemArray)
                {
                    if (int.TryParse(item.ToString(), out int value))
                    {
                        sum += value;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                double average = sum / count;
                Average_txtBox.Text = average.ToString("F2");
            }
            else
            {
                Average_txtBox.Text = "N/A";
            }
        }

        #region Load & Save 


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
                csvFiles = new List<string>(openFile.FileNames);
                currentFileIndex = 0;

                string filePath = csvFiles[currentFileIndex];
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                DataView dataView = null;
                if (extension == ".csv")
                {
                    dataView = LoadCsv(filePath);
                }
                else if (extension == ".bin")
                {
                    dataView = LoadBin(filePath);
                }

                if (dataView != null)
                {
                    DataGrid.ItemsSource = dataView;
                    CalculateAverage();
                    UpdateSampleLabel(filePath);
                }
            }
        }





        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentFileIndex < csvFiles.Count - 1)
            {
                currentFileIndex++;
                string filePath = csvFiles[currentFileIndex];
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                DataView dataView = null;
                if (extension == ".csv")
                {
                    dataView = LoadCsv(filePath);
                }
                else if (extension == ".bin")
                {
                    dataView = LoadBin(filePath);
                }

                if (dataView != null)
                {
                    DataGrid.ItemsSource = dataView;
                    CalculateAverage();
                    UpdateSampleLabel(filePath);
                }
            }
        }


        private void Btn_Previous_Click(object sender, RoutedEventArgs e)
{
            if (currentFileIndex > 0) // 
            {
                //currentFileIndex++;
                //var dataView = Load(csvFiles[currentFileIndex]); // Load next file
                currentFileIndex--;
                string filePath = csvFiles[currentFileIndex];
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                DataView dataView = null;
                if (extension == ".csv")
                {
                    dataView = LoadCsv(filePath);
                }
                else if (extension == ".bin")
                {
                    dataView = LoadBin(filePath);
                }

                if (dataView != null)
                {


                    DataGrid.ItemsSource = dataView;
                    CalculateAverage();
                    UpdateSampleLabel(filePath);
                }
            }
        }

        //LoaD FOR csv only
        public static DataView LoadCsv(string path)
        {
            DataTable dataTable = new DataTable();
            TextFieldParser parser = new TextFieldParser(path);
            parser.SetDelimiters(",");

            List<string[]> rows = new List<string[]>();

            if (!parser.EndOfData)
            {
                var columns = parser.ReadFields();
                foreach (var col in columns)
                    dataTable.Columns.Add(col);
            }

            while (!parser.EndOfData)
            {
                var row = parser.ReadFields();
                rows.Add(row);
                dataTable.Rows.Add(row);
            }

            // Convert to 2D array for logic
            int rowCount = rows.Count;
            int colCount = rows[0].Length;
            SensorArray = new double[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount; j++)
                    double.TryParse(rows[i][j], out SensorArray[i, j]);

            return dataTable.DefaultView;
        }

        //LoaD FOR bin only 
        private DataView LoadBin(string filePath)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    int rowCount = reader.ReadInt32();
                    int colCount = reader.ReadInt32();

                    SensorArray = new double[rowCount, colCount];
                    DataTable dataTable = new DataTable();

                    for (int col = 0; col < colCount; col++)
                        dataTable.Columns.Add("Sensor " + (col + 1));

                    for (int row = 0; row < rowCount; row++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        for (int col = 0; col < colCount; col++)
                        {
                            double value = reader.ReadDouble();
                            SensorArray[row, col] = value;
                            dataRow[col] = value;
                        }
                        dataTable.Rows.Add(dataRow);
                    }

                    return dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading binary: " + ex.Message);
                return null;
            }
        }


        //label sample no 
        //private void UpdateSampleLabel()
        //{
        //    if (csvFiles.Count > 0)
        //        SampleLabel.Content = $"Currently Viewing: {System.IO.Path.GetFileName(csvFiles[currentFileIndex])}";
        //    else
        //        SampleLabel.Content = "No file loaded";
        //}
        private void UpdateSampleLabel(string fileName)
        {
            SampleLabel.Content = $"Currently Viewing: {System.IO.Path.GetFileName(fileName)}";
        }




        //Save
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



        #endregion

        #region Binary Search 
        //Binary Search 
        private void SaveCSV(string filePath)
        {
            if (DataGrid.ItemsSource is DataView dataView)
            {
                using (var writer = new StreamWriter(filePath))
                {
                    var columnNames = dataView.Table.Columns
                        .Cast<DataColumn>()
                        .Select(col => col.ColumnName);
                    writer.WriteLine(string.Join(",", columnNames));

                    foreach (DataRowView rowView in dataView)
                    {
                        var fields = rowView.Row.ItemArray
                            .Select(field => field.ToString());
                        writer.WriteLine(string.Join(",", fields));
                    }
                }

                MessageBox.Show("CSV saved successfully.");
            }
            else
            {
                MessageBox.Show("No data to save.");
            }
        }
        private void SaveBin(string filePath)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {
                    int rowCount = SensorArray.GetLength(0);
                    int colCount = SensorArray.GetLength(1);

                    writer.Write(rowCount);
                    writer.Write(colCount);

                    for (int i = 0; i < rowCount; i++)
                        for (int j = 0; j < colCount; j++)
                            writer.Write(SensorArray[i, j]);
                }

                MessageBox.Show("Binary file saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving binary: " + ex.Message);
            }
        }


        #endregion


        private void Bounds_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update bounds from the textboxes
            double.TryParse(LB_txtBox.Text, out lowerBound);
            double.TryParse(UB_txtBox.Text, out upperBound);
        }


        // for the next and previous . let say i am currently on sample2
        //i should be able to see a label stating that i am on sample 2 , and same if i move to the other samples//





    }

}