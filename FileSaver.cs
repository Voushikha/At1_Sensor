using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AT1_Sensor
{
    public class FileSaver
    {
        public void Save(string filePath, DataView dataView)
        {
            if (dataView == null || dataView.Table.Rows.Count == 0)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write column headers
                    var columnNames = dataView.Table.Columns
                        .Cast<DataColumn>()
                        .Select(col => col.ColumnName);
                    writer.WriteLine(string.Join(",", columnNames));

                    // Write rows
                    foreach (DataRowView rowView in dataView)
                    {
                        var fields = rowView.Row.ItemArray
                            .Select(field => field?.ToString() ?? "");
                        writer.WriteLine(string.Join(",", fields));
                    }
                }

                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
        }
    }
}
