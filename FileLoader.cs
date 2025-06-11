using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AT1_Sensor

{
    public class FileLoader
    {
        public DataView Load(string filePath, ref double[,] sensorArray)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".csv" => LoadCsv(filePath, ref sensorArray),
                ".bin" => LoadBin(filePath, ref sensorArray),
                _ => throw new NotSupportedException($"File type '{extension}' is not supported.")
            };
        }

        public DataView LoadCsv(string filePath, ref double[,] sensorArray)
        {
            var table = new DataTable();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                throw new Exception("CSV file is empty.");

            // Create columns based on the first row
            var headers = lines[0].Split(',');
            foreach (var header in headers)
                table.Columns.Add(header);

            // Create data rows
            sensorArray = new double[lines.Length - 1, headers.Length];
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                table.Rows.Add(values);

                for (int j = 0; j < headers.Length; j++)
                    sensorArray[i - 1, j] = double.Parse(values[j]);
            }

            return table.DefaultView;
        }

        public DataView LoadBin(string filePath, ref double[,] sensorArray)
        {
            using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));

            int rows = reader.ReadInt32();
            int cols = reader.ReadInt32();

            sensorArray = new double[rows, cols];
            var table = new DataTable();

            for (int i = 0; i < cols; i++)
                table.Columns.Add($"Col{i + 1}");

            for (int i = 0; i < rows; i++)
            {
                var row = table.NewRow();
                for (int j = 0; j < cols; j++)
                {
                    double value = reader.ReadDouble();
                    sensorArray[i, j] = value;
                    row[j] = value;
                }
                table.Rows.Add(row);
            }

            return table.DefaultView;
        }
    }
}