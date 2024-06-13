using AttendanceTracking.View.Components;
using AttendanceTracking.View.Entities;
using AttendanceTracking.View.Services;
using System;
using System.Collections.Generic;
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
    public partial class AttendanceViewForm : Window
    {
        private int[] Years = new int[] {
                (DateTime.Now.Month > 8 ? DateTime.Now.Year : DateTime.Now.Year - 1),
                ((DateTime.Now.Month > 8 ? DateTime.Now.Year : DateTime.Now.Year - 1) - 1),
                ((DateTime.Now.Month > 8 ? DateTime.Now.Year : DateTime.Now.Year - 1) - 2),
                ((DateTime.Now.Month > 8 ? DateTime.Now.Year : DateTime.Now.Year - 1) - 3)
            };
        private string[] russianMonths = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
        private Func<DateTime, IEnumerable<AttendensesOnMonth>> _getAttendenceMonthQuery;

        public class AttendenceMonth
        {
            public DateTime Month { get; private set; }
            public IEnumerable<AttendensesOnMonth> Attendence { get; private set; }

            public AttendenceMonth(DateTime month, IEnumerable<AttendensesOnMonth> attendence)
            {
                Month = month;
                Attendence = attendence;
            }
        }
        private List<AttendenceMonth> Months = new List<AttendenceMonth>();

        public AttendanceViewForm(int groupId)
        {
            var studentsService = new GroupService();
            var attendencesService = new AttendensService();

            InitializeComponent();
            TextGroup.Text = studentsService.GetGroupName(groupId);
            StudentsTable.ItemsSource = studentsService.GetStudentsByGroup(groupId)
                .Select((s, i) => new { Id = i + 1, FullName = s.FullName })
                .Concat(new object[] { new { FullName = "Всего пропусков:" } });
            _getAttendenceMonthQuery = m => attendencesService.GetAttendensesOnMonth(m, groupId);
            ComboYear.ItemsSource = Years.Select(y => $"{y} - {y + 1}");
            ComboYear.SelectedIndex = 0;
            ComboSemestr.SelectionChanged += InitMonthTables;
            ComboYear.SelectionChanged += InitMonthTables;
            InitMonthTables(this, null);
        }

        private void InitMonthTables(object sender, RoutedEventArgs e)
        {
            Months.Clear();
            if (ComboSemestr.SelectedIndex == 0 || ComboSemestr.SelectedIndex == 1)
            {
                var sep = new DateTime(Years[ComboYear.SelectedIndex], 9, 1);
                Months.Add(new AttendenceMonth(sep, _getAttendenceMonthQuery(sep)));
                var oct = new DateTime(Years[ComboYear.SelectedIndex], 10, 1);
                Months.Add(new AttendenceMonth(oct, _getAttendenceMonthQuery(oct)));
                var nov = new DateTime(Years[ComboYear.SelectedIndex], 11, 1);
                Months.Add(new AttendenceMonth(nov, _getAttendenceMonthQuery(nov)));
                var dec = new DateTime(Years[ComboYear.SelectedIndex], 12, 1);
                Months.Add(new AttendenceMonth(dec, _getAttendenceMonthQuery(dec)));
            }
            if (ComboSemestr.SelectedIndex == 0 || ComboSemestr.SelectedIndex == 2)
            {
                var yan = new DateTime(Years[ComboYear.SelectedIndex] + 1, 1, 1);
                Months.Add(new AttendenceMonth(yan, _getAttendenceMonthQuery(yan)));
                var feb = new DateTime(Years[ComboYear.SelectedIndex] + 1, 2, 1);
                Months.Add(new AttendenceMonth(feb, _getAttendenceMonthQuery(feb)));
                var mar = new DateTime(Years[ComboYear.SelectedIndex] + 1, 3, 1);
                Months.Add(new AttendenceMonth(mar, _getAttendenceMonthQuery(mar)));
                var may = new DateTime(Years[ComboYear.SelectedIndex] + 1, 5, 1);
                Months.Add(new AttendenceMonth(may, _getAttendenceMonthQuery(may)));
                var jun = new DateTime(Years[ComboYear.SelectedIndex] + 1, 6, 1);
                Months.Add(new AttendenceMonth(jun, _getAttendenceMonthQuery(jun)));
            }

            var enumerableAttendence = Months.First().Attendence
                .Select((_, i) =>
                {
                    var excused = 0;
                    var unexcused = 0;
                    foreach (var m in Months.Select(m => m.Attendence.ToArray()[i]))
                    {
                        excused += m.Excused;
                        unexcused += m.Unexcused;
                    }
                    return new AttendensesOnMonth(excused, unexcused);
                });

            AditionalTable.ItemsSource = enumerableAttendence
                .Select(a => new
                {
                    Sum = a.Excused + a.Unexcused,
                    Excused = a.Excused != 0 ? a.Excused.ToString() : "",
                    Unexcused = a.Unexcused != 0 ? a.Unexcused.ToString() : "",
                })
                .Concat(new object[] { new
                    {
                        Sum = enumerableAttendence.Sum(s => s.Excused) + enumerableAttendence.Sum(s => s.Unexcused),
                        Excused = enumerableAttendence.Sum(s => s.Excused),
                        Unexcused = enumerableAttendence.Sum(s => s.Unexcused)
                    }
                });

            MonthDataGrid.Children.Clear();
            foreach (var month in Months)
                MonthDataGrid.Children.Add(new AditionMonthTable(russianMonths[month.Month.Month - 1], month.Attendence));

        }

        private void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var excelBook = excelApp.Workbooks.Add();
                var excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets[1];
                var title = "Посещаемость группы " + TextGroup.Text + " за " + ComboSemestr.Text + " " + ComboYear.Text + " года";
                excelApp.DefaultFilePath = title;

                excelSheet.Cells[1, 1] = title;
                excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, AditionalTable.Columns.Count + StudentsTable.Columns.Count + Months.Count()*2]].Merge();

                for (int i = 1; i <= StudentsTable.Columns.Count; i++)
                {
                    excelSheet.Cells[2, i] = StudentsTable.Columns[i - 1].Header;
                    excelSheet.Range[excelSheet.Cells[2, i], excelSheet.Cells[3, i]].Merge();
                }
                for (int i = 1; i <= Months.Count()*2; i+=2)
                {
                    var n = (i - 1) / 2;
                    excelSheet.Cells[2, i + StudentsTable.Columns.Count] = russianMonths[Months[n].Month.Month - 1];
                    excelSheet.Range[excelSheet.Cells[2, i + StudentsTable.Columns.Count], excelSheet.Cells[2, i+1 + StudentsTable.Columns.Count]].Merge();

                    excelSheet.Cells[3, i + StudentsTable.Columns.Count] = "уваж.";
                    excelSheet.Cells[3, i + 1 + StudentsTable.Columns.Count] = "не уваж.";
                }
                excelSheet.Cells[2, 1 + StudentsTable.Columns.Count + Months.Count() * 2] = "Итого:";
                excelSheet.Range[excelSheet.Cells[2, 1 + StudentsTable.Columns.Count + Months.Count() * 2], excelSheet.Cells[2, StudentsTable.Columns.Count + Months.Count() * 2 + 3]].Merge();
                for (int i = 1; i <= AditionalTable.Columns.Count; i++)
                {
                    excelSheet.Cells[3, i + StudentsTable.Columns.Count + Months.Count()*2] = AditionalTable.Columns[i - 1].Header;
                }

                for (int i = 0; i < StudentsTable.Items.Count; i++)
                {
                    var rowView = StudentsTable.Items[i];

                    excelSheet.Cells[i + 4, 1] = rowView.GetType().GetProperty("Id")?.GetValue(rowView) ?? string.Empty;
                    excelSheet.Cells[i + 4, 2] = rowView.GetType().GetProperty("FullName").GetValue(rowView);
                }

                Microsoft.Office.Interop.Excel.Range sumRange = excelSheet.Range[
                    excelSheet.Cells[4, StudentsTable.Columns.Count + Months.Count() * 2 + 1],
                    excelSheet.Cells[AditionalTable.Items.Count + 3, StudentsTable.Columns.Count + Months.Count() * 2 + 1]];
                sumRange.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbBlueViolet;
                sumRange.Font.Bold = true;
                Microsoft.Office.Interop.Excel.Range excusedRange = excelSheet.Range[
                    excelSheet.Cells[4, StudentsTable.Columns.Count + Months.Count() * 2 + 2],
                    excelSheet.Cells[AditionalTable.Items.Count + 3, StudentsTable.Columns.Count + Months.Count() * 2 + 2]];
                excusedRange.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbDarkGreen;
                excusedRange.Font.Bold = true;
                Microsoft.Office.Interop.Excel.Range unexcusedRange = excelSheet.Range[
                    excelSheet.Cells[4, StudentsTable.Columns.Count + Months.Count() * 2 + 3],
                    excelSheet.Cells[AditionalTable.Items.Count + 3, StudentsTable.Columns.Count + Months.Count() * 2 + 3]];
                unexcusedRange.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbDarkRed;
                unexcusedRange.Font.Bold = true;
                for (int i = 0; i < AditionalTable.Items.Count; i++)
                {
                    var rowView = AditionalTable.Items[i];

                    excelSheet.Cells[i + 4, StudentsTable.Columns.Count + Months.Count() * 2 + 1] = rowView.GetType().GetProperty("Sum").GetValue(rowView);
                    excelSheet.Cells[i + 4, StudentsTable.Columns.Count + Months.Count() * 2 + 2] = rowView.GetType().GetProperty("Excused").GetValue(rowView);
                    excelSheet.Cells[i + 4, StudentsTable.Columns.Count + Months.Count() * 2 + 3] = rowView.GetType().GetProperty("Unexcused").GetValue(rowView);
                }

                for (int i = 0; i < Months.Count(); i++)
                {
                    var m = Months[i];
                    for (int j = 0; j < Months[i].Attendence.Count(); j++)
                    {
                        var s = m.Attendence.ToArray()[j];
                        if (s.Excused != 0)
                        {
                            excelSheet.Cells[j + 4, i * 2 + 3] =  s.Excused;
                            Microsoft.Office.Interop.Excel.Range ex = excelSheet.Range[excelSheet.Cells[j + 4, i * 2 + 3], excelSheet.Cells[j + 4, i * 2 + 3]];
                            ex.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbCadetBlue;
                            ex.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                        }

                        if (s.Unexcused != 0)
                        {
                            excelSheet.Cells[j + 4, i * 2 + 4] = s.Unexcused;
                            Microsoft.Office.Interop.Excel.Range uex = excelSheet.Range[excelSheet.Cells[j + 4, i * 2 + 4], excelSheet.Cells[j + 4, i * 2 + 4]];
                            uex.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbPaleVioletRed;
                            uex.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                        }
                    }
                    if (m.Attendence.Sum(s => s.Excused) != 0)
                    {
                        excelSheet.Cells[Months[i].Attendence.Count() + 4, i * 2 + 3] = m.Attendence.Sum(s => s.Excused);
                        Microsoft.Office.Interop.Excel.Range ex = excelSheet.Range[
                            excelSheet.Cells[Months[i].Attendence.Count() + 4, i * 2 + 3],
                            excelSheet.Cells[Months[i].Attendence.Count() + 4, i * 2 + 3]];
                        ex.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbGreen;
                        ex.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                    }

                    if (m.Attendence.Sum(s => s.Unexcused) != 0)
                    {
                        excelSheet.Cells[Months[i].Attendence.Count() + 4, i * 2 + 4] = m.Attendence.Sum(s => s.Unexcused);
                        Microsoft.Office.Interop.Excel.Range uex = excelSheet.Range[
                            excelSheet.Cells[Months[i].Attendence.Count() + 4, i * 2 + 4],
                            excelSheet.Cells[Months[i].Attendence.Count() + 4, i * 2 + 4]];
                        uex.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbRed;
                        uex.Font.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbWhite;
                    }
                }

                excelSheet.Columns.AutoFit();
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("При формировании отчета произошла ошибка:\n"+ex.Message);
            }

        }
    }
}
