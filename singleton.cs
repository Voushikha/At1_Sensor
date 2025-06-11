using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using static AT1_Sensor.MainWindow;

namespace AT1_Sensor
{
    // Define the Sensor class
    public class Sensor
    {
        public string Label { get; set; } = string.Empty;
        public double[,] Data { get; set; } = new double[0, 0];

        public double Average
        {
            get
            {
                double sum = 0;
                int count = 0;

                for (int i = 0; i < Data.GetLength(0); i++)
                {
                    for (int j = 0; j < Data.GetLength(1); j++)
                    {
                        sum += Data[i, j];
                        count++;
                    }
                }

                return count > 0 ? sum / count : 0;
            }
        }
    }

    public class Singleton
    {
        private static readonly Singleton instance = new Singleton();
        internal double[,] SensorArray;

        public static Singleton Instance => instance;

        // Replacing the old SensorArray with a list of Sensor objects
        public List<Sensor> Sensors { get; set; } = new List<Sensor>();

        private Singleton() { }

        // Returns the average of the first sensor (you can customize this)
        public double CalculateAverage(int index)
        {
            if (Sensors.Count == 0 || index < 0 || index >= Sensors.Count)
                return 0;

            return Sensors[index].Average;
        }
        public void SortSensorsByLabel()
        {
            Sensors = Sensors.OrderBy(s => s.Label).ToList();
        }

        //encap in singleton here
        // Binary search method
        public int BinarySearch(List<GridData> sortedList, double target)
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

        // sorting the data before performing a binary search
        public int SearchByLabel(string targetLabel)
        {
            var sortedSensors = Sensors.OrderBy(s => s.Label).ToList();
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
    }
}

      
       