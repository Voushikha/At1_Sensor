using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.VisualBasic.FileIO;

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


    }
}

      
        //public double CalculateAverage()
        //{
        //    if (SensorArray == null)
        //        return 0;

        //    double sum = 0;
        //    int count = 0;

        //    for (int i = 0; i < SensorArray.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < SensorArray.GetLength(1); j++)
        //        {
        //            sum += SensorArray[i, j];
        //            count++;
        //        }
        //    }

        //    return count > 0 ? sum / count : 0;
        //}

//    }
//}
//// 