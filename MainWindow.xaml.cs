using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AT1_Sensor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // try use traffic ilight system , for the files 
        //can use files or csv to load data



        public MainWindow()
        {
            InitializeComponent();
            // Average();
            // Example grid data

            ////replace the following with data. that will be loaded from load button 
            ////below is just for texting average
        //    var loadData = new List<GridData>
        //    {
        //        new GridData { Grid = "A", Load = 100 },
        //        new GridData { Grid = "B", Load = 150 },
        //        new GridData { Grid = "C", Load = 200 },
        //        new GridData { Grid = "D", Load = 250 }
        //};

        //    DataGrid.ItemsSource = loadData; // Bind data to DataGrid
        //    CalculateAverage();
        }


        // Calculated Average of loaded dta 
        private void CalculateAverage()
        {
            var gridData = (List<GridData>)DataGrid.ItemsSource;
            if (gridData != null && gridData.Any())
            {
                double averageLoad = gridData.Average(x => x.Load);
                Average_txtBox.Text = averageLoad.ToString();

            }
            else
            {
                MessageBox.Show("No data available to calculate the average.", "Error");
            }
         }



            // Model class
        public class GridData
        {
            public string Grid { get; set; }
            public int Load { get; set; }
        }



        //Load method
        //private void Load()
        //{
        //    // Method here
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //   dilg.Filter = "Excel Sheet(.xlsx)||"


        //    //then add the average // not sure if average method need to be added 
        //    //after loading data or ...
        //}
        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
            CalculateAverage();
            var gridData = (List<GridData>)DataGrid.ItemsSource;


            // Open file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                Load(filePath);
            }
          
        }

        private void Load(string filePath)
        {
            var dataTable = new DataTable();

            using (var reader = new StreamReader(filePath))
            {
                // Read header
                string[] headers = reader.ReadLine().Split(',');
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Trim());
                }

                // Read rows
                while (!reader.EndOfStream)
                {
                    string[] rows = reader.ReadLine().Split(',');
                    dataTable.Rows.Add(rows);
                }
            }

            // Bind to DataGrid
            DataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void Save()
        {
        }
        private void Sort()
        {

        }
        // Calculate average loaded data
        private void Average()
        {

        }
        //Binary Search 
        private void BinarySearch_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

      
    }
}