using AttendanceTracking.View.Entities;
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
        

        public delegate void ChangeHoursHandler(object sender, Attendens e);

        public event ChangeHoursHandler ChangeHours;

        private int[] _holidays;
        private Attendens[] _values;
        private DataTable _source;

        public static MonthTable Create(DateTime date, int rows, params Attendens[] values)
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

            var str = new string[0];
            return new MonthTable(daysInMonth, source, values, holidays.ToArray());
        }

        public MonthTable(int days, List<List<string>> source, Attendens[] values = null, params int[] holidays)
        {
            _holidays = holidays;
            _values = values ?? new Attendens[0];
            _source = new DataTable();
            for (int i = 1; i <= days; i++)
                _source.Columns.Add(new DataColumn(i.ToString()));
            source.ForEach(r => _source.Rows.Add(r.ToArray()));
            InitializeComponent();
            InitializeMonthTable();
        }

        private void InitializeMonthTable()
        {
            Table.ItemsSource = _source.DefaultView;
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
            DataGridRow rowTable = (DataGridRow)Table.ItemContainerGenerator.ContainerFromIndex(row);
            DataGridCell cell = Table.Columns[column].GetCellContent(rowTable).Parent as DataGridCell;

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

            if ((e.Row.Item as DataRowView).Row[column].ToString() != _cellText)
            {
                if (cell.Background == Brushes.White)
                {
                    cell.Background = Brushes.Red;
                    cell.Foreground = Brushes.White;
                }
                if (number == 0)
                {
                    cell.Background = Brushes.White;
                    cell.Foreground = Brushes.Black;
                }

                ChangeHours?.Invoke(sender, new Attendens(row, column + 1, number, cell.Background == Brushes.Green));
            }
        }



        private void Table_Loaded(object sender, RoutedEventArgs e)
        {
            Style styleCells = new Style(typeof(DataGridCell));
            styleCells.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.White));
            styleCells.Setters.Add(new Setter(DataGridCell.ForegroundProperty, Brushes.Black));
            Table.CellStyle = styleCells;
            foreach (var day in _holidays)
            {
                DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
                buttonColumn.CellTemplate = new DataTemplate(typeof(Button));
                buttonColumn.Header = day;

                FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
                
                buttonColumn.CellTemplate.VisualTree = buttonFactory;
                Table.Columns[day - 1] = buttonColumn;
            }

            foreach (var v in _values)
            {
                _source.Rows[v.RowIndex][v.Day - 1] = v.Hours.ToString();
                Brush newBackgroundBrush = v.IsExcused ? Brushes.Green : Brushes.Red;

                DataGridRow row = (DataGridRow)Table.ItemContainerGenerator.ContainerFromIndex(v.RowIndex);
                DataGridCell cell = Table.Columns[v.Day - 1].GetCellContent(row).Parent as DataGridCell;

                Style style = new Style(typeof(DataGridCell));
                style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, newBackgroundBrush));
                style.Setters.Add(new Setter(DataGridCell.ForegroundProperty, Brushes.White));
                cell.Style = style;
            }
        }

        private void Table_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cell = GetCellFromCellInfo(Table.CurrentCell);
            if (cell != null)
            {
                ContextEditToExcused.Visibility = cell.Background == Brushes.Red 
                    ? Visibility.Visible : Visibility.Collapsed;
                ContextEditToUnexcused.Visibility = cell.Background == Brushes.Green
                    ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private DataGridCell GetCellFromCellInfo(DataGridCellInfo cellInfo)
        {
            if (cellInfo.IsValid)
            {
                DataGridCell cell = cellInfo.Column.GetCellContent(cellInfo.Item).Parent as DataGridCell;
                return cell;
            }

            return null;
        }

        private void ContextEditToExcused_Click(object sender, RoutedEventArgs e)
        {
            var cell = GetCellFromCellInfo(Table.CurrentCell);
            cell.Background = Brushes.Green;

            int rowIndex = Table.Items.IndexOf(cell.DataContext);

            int columnIndex = cell.Column.DisplayIndex;

            int number = int.Parse(((TextBlock)cell.Content).Text);

            ChangeHours?.Invoke(sender, new Attendens(rowIndex, columnIndex + 1, number, cell.Background == Brushes.Green));
        }

        private void ContextEditToUnexcused_Click(object sender, RoutedEventArgs e)
        {
            var cell = GetCellFromCellInfo(Table.CurrentCell);
            cell.Background = Brushes.Red;

            int rowIndex = Table.Items.IndexOf(cell.DataContext);

            int columnIndex = cell.Column.DisplayIndex;

            int number = int.Parse(((TextBlock)cell.Content).Text);

            ChangeHours?.Invoke(sender, new Attendens(rowIndex, columnIndex + 1, number, cell.Background == Brushes.Green)); ;
        }
    }
}
