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
                excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, MonthTable.Table.Columns.Count + StudentsTable.Columns.Count]].Merge();

                for (int i = 1; i <= StudentsTable.Columns.Count; i++)
                {
                    excelSheet.Cells[2, i] = StudentsTable.Columns[i - 1].Header;
                }
                for (int i = 1; i <= MonthTable.Table.Columns.Count; i++)
                {
                    excelSheet.Cells[2, i + StudentsTable.Columns.Count] = MonthTable.Table.Columns[i - 1].Header;
                }

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

                excelSheet.Columns.AutoFit();
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("При формировании отчета произошла ошибка");
            }
            
        }
    }
}
