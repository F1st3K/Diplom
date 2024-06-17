using AttendanceTracking.View.Components;
using AttendanceTracking.View.Entities;
using AttendanceTracking.View.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AttendanceTracking.View.Forms
{
    /// <summary>
    /// Interaction logic for AttendanceViewForm.xaml
    /// </summary>
    public partial class MonthAttendanceViewForm : Window
    {
        private MonthTable MonthTable;

        private Func<DateTime, IEnumerable<Attendens>> _getHoursQuery;

        public MonthAttendanceViewForm(int groupId)
        {
            var studentsService = new GroupService();
            var attendencesService = new AttendensService();

            InitializeComponent();
            TextGroup.Text = studentsService.GetGroupName(groupId);
            StudentsTable.ItemsSource = studentsService.GetStudentsByGroup(groupId)
                .Select((s, i) => new { Id = i + 1, FullName = s.FullName });
            _getHoursQuery = m => attendencesService.GetAttendenses(m, groupId);
            MonthSwitcher.SelectedIndex = 0;
        }

        private void InitMonthDataGrid(DateTime date)
        {
            if (MonthTable != null)
                MonthDataGrid.Children.Clear();

            MonthTable = MonthTable.Create(date, StudentsTable.Items.Count, _getHoursQuery?.Invoke(date).ToArray());
            MonthTable.Table.IsReadOnly = true;
            MonthTable.Context.Visibility = Visibility.Collapsed;
            MonthDataGrid.Children.Add(MonthTable);
            
            AditionalTable.ItemsSource = Enumerable.Range(0, StudentsTable.Items.Count)
                .Select(i => new
                {
                    Excused = forNullEmpty(MonthTable.Values.Sum(v => v.StudentIndex == i && v.IsExcused ? v.Hours : 0)),
                    Unexcused = forNullEmpty(MonthTable.Values.Sum(v => v.StudentIndex == i && !v.IsExcused ? v.Hours : 0)),
                })
                .Concat(new[] { new {
                    Excused = forNullEmpty(MonthTable.Values.Sum(v => v.IsExcused ? v.Hours : 0)),
                    Unexcused = forNullEmpty(MonthTable.Values.Sum(v => !v.IsExcused ? v.Hours : 0)),
                }});
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += (ov, v) => { ColorAditinal(); timer.Stop(); };
            timer.Start();
        }
        string forNullEmpty(int n) => n == 0 ? string.Empty : n.ToString();

        private void ColorAditinal()
        {
            for (int i = 0; i < AditionalTable.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)AditionalTable.ItemContainerGenerator.ContainerFromIndex(i);
                {
                    DataGridCell cell = AditionalTable.Columns[0].GetCellContent(row).Parent as DataGridCell;
                    if (((TextBlock)cell.Content).Text != string.Empty)
                    {
                        Style style = new Style(typeof(DataGridCell));
                        style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.CadetBlue));
                        cell.Style = style;
                    }
                }
                {
                    DataGridCell cell = AditionalTable.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    if (((TextBlock)cell.Content).Text != string.Empty)
                    {
                        Style style = new Style(typeof(DataGridCell));
                        style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.PaleVioletRed));
                        cell.Style = style;
                    }
                }
                if (i == AditionalTable.Items.Count - 1)
                {
                    {
                        DataGridCell cell = AditionalTable.Columns[0].GetCellContent(row).Parent as DataGridCell;
                        if (((TextBlock)cell.Content).Text != string.Empty)
                        {
                            Style style = new Style(typeof(DataGridCell));
                            style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Green));
                            cell.Style = style;
                        }
                    }
                    {
                        DataGridCell cell = AditionalTable.Columns[1].GetCellContent(row).Parent as DataGridCell;
                        if (((TextBlock)cell.Content).Text != string.Empty)
                        {
                            Style style = new Style(typeof(DataGridCell));
                            style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Red));
                            cell.Style = style;
                        }
                    }
                }
            }
        }

        private string[] russianMonths = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
        private void CurrentMonth_Selected(object sender, RoutedEventArgs e)
        {
            var currentDay = DateTime.Today;
            InitMonthDataGrid(currentDay);
            TextMonth.Text = russianMonths[currentDay.Month - 1];
            TextYear.Text = currentDay.Year.ToString();
        }

        private void PrevMonth_Selected(object sender, RoutedEventArgs e)
        {
            var prevMonthDay = DateTime.Today.AddMonths(-1);
            InitMonthDataGrid(prevMonthDay);
            TextMonth.Text = russianMonths[prevMonthDay.Month - 1];
            TextYear.Text = prevMonthDay.Year.ToString();
        }

        private void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var excelBook = excelApp.Workbooks.Add();
                var excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets[1];

                excelSheet.Cells[1, 1] = "Учет посещаемости группы " + TextGroup.Text + " за " + TextMonth.Text + " " + TextYear.Text + "г.";
                excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, MonthTable.Table.Columns.Count + StudentsTable.Columns.Count + 2]].Merge();

                for (int i = 1; i <= StudentsTable.Columns.Count; i++)
                {
                    excelSheet.Cells[2, i] = StudentsTable.Columns[i - 1].Header;
                }
                for (int i = 1; i <= MonthTable.Table.Columns.Count; i++)
                {
                    excelSheet.Cells[2, i + StudentsTable.Columns.Count] = "'" + MonthTable.Table.Columns[i - 1].Header;

                }
                excelSheet.Cells[2, 1 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count] = "Всего уваж.";
                excelSheet.Cells[2, 2 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count] = "Всего не уваж.";

                for (int i = 0; i < StudentsTable.Items.Count; i++)
                {
                    var rowView = StudentsTable.Items[i];

                    excelSheet.Cells[i + 3, 1] = rowView.GetType().GetProperty("Id").GetValue(rowView);
                    excelSheet.Cells[i + 3, 2] = rowView.GetType().GetProperty("FullName").GetValue(rowView);
                }

                foreach (var v in MonthTable.Values)
                {
                    excelSheet.Cells[v.StudentIndex + 3, v.Day + StudentsTable.Columns.Count] = v.Hours;
                    Microsoft.Office.Interop.Excel.Range c = excelSheet.Range[
                        excelSheet.Cells[v.StudentIndex + 3, v.Day + StudentsTable.Columns.Count],
                        excelSheet.Cells[v.StudentIndex + 3, v.Day + StudentsTable.Columns.Count]];
                    c.Interior.Color = v.IsExcused 
                        ? Microsoft.Office.Interop.Excel.XlRgbColor.rgbGreen 
                        : Microsoft.Office.Interop.Excel.XlRgbColor.rgbRed;
                    c.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                }
                foreach (var h in MonthTable.Holidays)
                {
                    Microsoft.Office.Interop.Excel.Range r = excelSheet.Range[
                        excelSheet.Cells[3, h + StudentsTable.Columns.Count],
                        excelSheet.Cells[StudentsTable.Items.Count + 2, h + StudentsTable.Columns.Count]];
                    r.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightGray;
                }
                for (int i = 0; i < StudentsTable.Items.Count; i++)
                {
                    var exv = MonthTable.Values.Sum(v => v.StudentIndex == i && v.IsExcused ? v.Hours : 0);
                    if (exv != 0)
                    {
                        excelSheet.Cells[i + 3, 1 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count] = exv;
                        Microsoft.Office.Interop.Excel.Range ce = excelSheet.Range[
                            excelSheet.Cells[i + 3, 1 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count],
                            excelSheet.Cells[i + 3, 1 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count]];
                        ce.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbCadetBlue;
                        ce.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                    }
                    var uxv = MonthTable.Values.Sum(v => v.StudentIndex == i && !v.IsExcused ? v.Hours : 0);
                    if (uxv != 0)
                    {
                        excelSheet.Cells[i + 3, 2 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count] = uxv;

                        Microsoft.Office.Interop.Excel.Range cu = excelSheet.Range[
                                excelSheet.Cells[i + 3, 2 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count],
                                excelSheet.Cells[i + 3, 2 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count]];
                        cu.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbPaleVioletRed;
                        cu.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                    }

                    
                    //forNullEmpty(MonthTable.Values.Sum(v => v.IsExcused ? v.Hours : 0))
                }

                var sexv = MonthTable.Values.Sum(v => v.IsExcused ? v.Hours : 0);
                    excelSheet.Cells[StudentsTable.Items.Count + 3, 1 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count] = sexv;
                    Microsoft.Office.Interop.Excel.Range sce = excelSheet.Range[
                        excelSheet.Cells[StudentsTable.Items.Count + 3, 1 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count],
                        excelSheet.Cells[StudentsTable.Items.Count + 3, 1 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count]];
                    sce.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbGreen;
                    sce.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                
                var suxv = MonthTable.Values.Sum(v => !v.IsExcused ? v.Hours : 0);
                    excelSheet.Cells[StudentsTable.Items.Count + 3, 2 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count] = suxv;

                    Microsoft.Office.Interop.Excel.Range scu = excelSheet.Range[
                            excelSheet.Cells[StudentsTable.Items.Count + 3, 2 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count],
                            excelSheet.Cells[StudentsTable.Items.Count + 3, 2 + MonthTable.Table.Columns.Count + StudentsTable.Columns.Count]];
                    scu.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbRed;
                    scu.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                

                excelSheet.Columns.AutoFit();
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("При формировании отчета произошла ошибка");
            }
            
        }

        private void AditionalTable_Loaded(object sender, RoutedEventArgs e)
        {
            ColorAditinal();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
