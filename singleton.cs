using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace AT1_Sensor
{
    public class Singleton
    {
        private static readonly Singleton instance = new Singleton();
        public static Singleton Instance => instance;

        public double[,]? SensorArray { get; private set; } = new double[0, 0]; 

        private Singleton() { }

        public DataView LoadCsv(string path)
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

            int rowCount = rows.Count;
            int colCount = rows[0].Length;
            SensorArray = new double[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount; j++)
                    double.TryParse(rows[i][j], out SensorArray[i, j]);

            return dataTable.DefaultView;
        }

        public DataView LoadBin(string filePath)
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
                throw new Exception("Binary file load failed: " + ex.Message);
            }
        }

        public double CalculateAverage()
        {
            if (SensorArray == null)
                return 0;

            double sum = 0;
            int count = 0;

            for (int i = 0; i < SensorArray.GetLength(0); i++)
            {
                for (int j = 0; j < SensorArray.GetLength(1); j++)
                {
                    sum += SensorArray[i, j];
                    count++;
                }
            }

            return count > 0 ? sum / count : 0;
        }

    }
}
