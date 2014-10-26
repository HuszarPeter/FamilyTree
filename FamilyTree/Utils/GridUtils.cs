using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FamilyTree.Utils
{
    public class GridUtils
    {
        public static string GetRows(DependencyObject obj)
        {
            return (string)obj.GetValue(RowsProperty);
        }

        public static void SetRows(DependencyObject obj, string value)
        {
            obj.SetValue(RowsProperty, value);
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached("Rows", typeof(string), typeof(GridUtils), new PropertyMetadata("", OnRowsPropertyChanged));

        private static void OnRowsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is Grid))
            {
                throw new Exception("Must attach to a Grid");
            }

            var theGrid = dependencyObject as Grid;

            var newValue = dependencyPropertyChangedEventArgs.NewValue;
            if (!(newValue is string)) return;
            var str = (string)newValue;
            var cols = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            theGrid.RowDefinitions.Clear();
            cols.Select(row => new RowDefinition(){Height = ParseGridLength(row)})
                .ToList().ForEach(def => theGrid.RowDefinitions.Add(def));
        }


        public static string GetColumns(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnsProperty, value);
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns", typeof(string), typeof(GridUtils), new PropertyMetadata("", OnColumnsPropertyChanged ));

        private static void OnColumnsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if(!(dependencyObject is Grid))
            {
                throw new Exception("Must attach to a Grid");
            }

            var theGrid = dependencyObject as Grid;
            
            var newValue = dependencyPropertyChangedEventArgs.NewValue;
            if (!(newValue is string)) return;
            
            var str = (string) newValue;
            var cols = str.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            theGrid.ColumnDefinitions.Clear();
            cols.Select(col => new ColumnDefinition() {Width = ParseGridLength(col)})
                .ToList().ForEach(def => theGrid.ColumnDefinitions.Add(def));
        }

        private static GridLength ParseGridLength(string stringValue)
        {
            GridLength gridLength;

            var starMatch = new Regex(@"([0-9]*)\*$").Match(stringValue);
            var valueMatch = new Regex(@"([0-9])+$").Match(stringValue);
            var autoMatch = new Regex(@"(auto)$", RegexOptions.IgnoreCase).Match(stringValue);
            if (starMatch.Success)
            {
                int defSize;
                if (!int.TryParse(starMatch.Groups[1].Value, out defSize))
                    defSize = 1;
                gridLength = new GridLength(defSize, GridUnitType.Star);
            }
            else if (valueMatch.Success)
            {
                int size;
                if (!int.TryParse(valueMatch.Groups[1].Value, out size))
                    size = 0;
                gridLength = new GridLength(size);
            }
            else if (autoMatch.Success)
            {
                gridLength = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                throw new Exception(string.Format("Unknown term : {0}", stringValue));
            }

            return gridLength;
        }
    }
}
