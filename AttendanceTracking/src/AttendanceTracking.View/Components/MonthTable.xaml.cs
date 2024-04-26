using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AttendanceTracking.View.Components
{
    /// <summary>
    /// Interaction logic for MonthTable.xaml
    /// </summary>
    public partial class MonthTable : UserControl
    {
        public class Value
        {
            public int RowIndex { get; private set; }
            public int Day { get; private set; }
            public int Hours { get; private set; }
            public Value(int rowIndex, int day, int hours)
            {
                if (rowIndex < 0)
                    throw new ArgumentException("rowIndex must be more zero");
                if (day < 0)
                    throw new ArgumentException("day must be more zero");
                if (hours < 0)
                    throw new ArgumentException("hours must be more zero");
                RowIndex = rowIndex;
                Day = day;
                Hours = hours;
            }
        }

        public delegate void ChangeHoursHandler(object sender, Value e);

        public event ChangeHoursHandler ChangeHours;

        private int[] _holidays;

        public static MonthTable Create(DateTime date, int rows, params Value[] values)
        {
            var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            var holidays = new List<int>();
            for (int i = 1; i <= daysInMonth; i++)
                if (new DateTime(date.Year, date.Month, i).DayOfWeek == DayOfWeek.Sunday)
                    holidays.Add(i);

            var source = new List<List<string>>();
            for (int i = 0; i < rows; i++)
            {
                var row = new List<string>();
                for (int day = 1; day <= daysInMonth; day++)
                    row.Add(string.Empty);
                source.Add(row);
            }

            foreach (var v in values)
                source[v.RowIndex][v.Day - 1] = v.Hours.ToString();

            return new MonthTable(source, holidays.ToArray());
        }

        public MonthTable(List<List<string>> source, params int[] holidays)
        {
            InitializeComponent();
            InitializeMonthTable(source);
            _holidays = holidays;
        }

        private void InitializeMonthTable(List<List<string>> source)
        {
            var table = new DataTable();
            for (int i = 1; i <= source.First().Count; i++)
                table.Columns.Add(new DataColumn(i.ToString()));
            source.ForEach(r => table.Rows.Add(r.ToArray()));

            Table.ItemsSource = table.DefaultView;
            Table.CellEditEnding += Table_CellEditEnding;
            Table.BeginningEdit += Table_BeginningEdit;
        }

        private string _cellText = string.Empty;

        private void Table_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                foreach (DataGridCellInfo cellInfo in dataGrid.SelectedCells)
                {
                    DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item) as DataGridRow;
                    if (row != null && cellInfo.Column.DisplayIndex == e.Column.DisplayIndex)
                    {
                        int columnIndex = cellInfo.Column.DisplayIndex;

                        if (row.Item is DataRowView dataRow)
                        {
                            object cellValue = dataRow.Row.ItemArray[columnIndex];
                            _cellText = cellValue?.ToString();
                        }
                    }
                }
            }
        }

        private void Table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var row = e.Row.GetIndex();
            var column = e.Column.DisplayIndex;
            string newValue = (e.EditingElement as TextBox).Text;

            if (!int.TryParse(newValue, out var number) && newValue != string.Empty)
            {
                (e.Row.Item as DataRowView).Row[column] = _cellText;
                return;
            }

            if (number > 8)
                number = 8;
            (e.Row.Item as DataRowView).Row[column] = number;

            if (number <= 0)
            {
                number = 0;
                (e.Row.Item as DataRowView).Row[column] = string.Empty;
            }

            ChangeHours.Invoke(sender, new Value(row, column + 1, number));
        }

       

        private void Table_Loaded(object sender, RoutedEventArgs e)
        {           

            foreach (var day in _holidays)
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                buttonColumn.CellTemplate = new DataTemplate(typeof(Button));
                buttonColumn.Header = day;

                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                buttonFactory.SetValue(Button.ContentProperty, "");
                buttonColumn.CellTemplate.VisualTree = buttonFactory;
                Table.Columns[day - 1] = buttonColumn;
            }
        }
    }
}
