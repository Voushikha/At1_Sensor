using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace AT1_Sensor
{
    public static class colorIndicators
    {
        public static void ApplyColors(DataGrid dataGrid, double lower, double upper, double? highlightTarget)
        {
            dataGrid.UpdateLayout();
            foreach (var item in dataGrid.Items)
            {
                if (dataGrid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                {
                    foreach (DataGridCell cell in GetCells(row, dataGrid))
                    {
                        if (cell.Content is TextBlock textBlock && double.TryParse(textBlock.Text, out double value))
                        {
                            //Binary search value highlighted yellow 
                            if (highlightTarget.HasValue && value == highlightTarget.Value)
                                cell.Background = Brushes.Yellow;
                            else if (value < lower)
                                cell.Background = Brushes.Blue;
                            else if (value > upper)
                                cell.Background = Brushes.Red;
                            else
                                cell.Background = Brushes.Green;
                        }
                    }
                }
            }
        }

        private static IEnumerable<DataGridCell> GetCells(DataGridRow row, DataGrid grid)
        {
            var presenter = FindVisualChild<DataGridCellsPresenter>(row);
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                if (presenter.ItemContainerGenerator.ContainerFromIndex(i) is DataGridCell cell)
                    yield return cell;
            }
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T tChild) return tChild;
                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }
    }
}