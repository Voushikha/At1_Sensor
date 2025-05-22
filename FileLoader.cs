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
        public DataView LoadCsv(string path, ref double[,] sensorArray)
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
            sensorArray = new double[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount; j++)
                    double.TryParse(rows[i][j], out sensorArray[i, j]);

            return dataTable.DefaultView;
        }

        public DataView LoadBin(string filePath, ref double[,] sensorArray)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    int rowCount = reader.ReadInt32();
                    int colCount = reader.ReadInt32();

                    sensorArray = new double[rowCount, colCount];
                    DataTable dataTable = new DataTable();

                    for (int col = 0; col < colCount; col++)
                        dataTable.Columns.Add("Sensor " + (col + 1));

                    for (int row = 0; row < rowCount; row++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        for (int col = 0; col < colCount; col++)
                        {
                            double value = reader.ReadDouble();
                            sensorArray[row, col] = value;
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
    }
}
