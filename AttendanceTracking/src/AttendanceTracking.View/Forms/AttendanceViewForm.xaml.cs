using AttendanceTracking.View.Components;
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
        private Func<DateTime, IEnumerable<AditionMonthTable.Value>> _getAttendenceMonthQuery;

        public class AttendenceMonth
        {
            public DateTime Month { get; private set; }
            public IEnumerable<AditionMonthTable.Value> Attendence { get; private set; }

            public AttendenceMonth(DateTime month, IEnumerable<AditionMonthTable.Value> attendence)
            {
                Month = month;
                Attendence = attendence;
            }
        }
        private List<AttendenceMonth> Months = new List<AttendenceMonth>();

        public AttendanceViewForm(
            string groupName,
            IEnumerable<string> students,
            Func<DateTime, IEnumerable<AditionMonthTable.Value>> getAttendenceMonthQuery)
        {
            InitializeComponent();
            TextGroup.Text = groupName;
            StudentsTable.ItemsSource = students
                .Select((s, i) => new { Id = i + 1, FullName = s })
                .Concat(new object[] { new { FullName = "Всего пропусков:" } });
            _getAttendenceMonthQuery = getAttendenceMonthQuery;
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
                Months.Add(new AttendenceMonth(sep, _getAttendenceMonthQuery(sep).ToArray()));
                var oct = new DateTime(Years[ComboYear.SelectedIndex], 10, 1);
                Months.Add(new AttendenceMonth(oct, _getAttendenceMonthQuery(oct).ToArray()));
                var nov = new DateTime(Years[ComboYear.SelectedIndex], 11, 1);
                Months.Add(new AttendenceMonth(nov, _getAttendenceMonthQuery(nov).ToArray()));
                var dec = new DateTime(Years[ComboYear.SelectedIndex], 12, 1);
                Months.Add(new AttendenceMonth(dec, _getAttendenceMonthQuery(dec).ToArray()));
            }
            if (ComboSemestr.SelectedIndex == 0 || ComboSemestr.SelectedIndex == 2)
            {
                var yan = new DateTime(Years[ComboYear.SelectedIndex] + 1, 1, 1);
                Months.Add(new AttendenceMonth(yan, _getAttendenceMonthQuery(yan).ToArray()));
                var feb = new DateTime(Years[ComboYear.SelectedIndex] + 1, 2, 1);
                Months.Add(new AttendenceMonth(feb, _getAttendenceMonthQuery(feb).ToArray()));
                var mar = new DateTime(Years[ComboYear.SelectedIndex] + 1, 3, 1);
                Months.Add(new AttendenceMonth(mar, _getAttendenceMonthQuery(mar).ToArray()));
                var may = new DateTime(Years[ComboYear.SelectedIndex] + 1, 5, 1);
                Months.Add(new AttendenceMonth(may, _getAttendenceMonthQuery(may).ToArray()));
                var jun = new DateTime(Years[ComboYear.SelectedIndex] + 1, 6, 1);
                Months.Add(new AttendenceMonth(jun, _getAttendenceMonthQuery(jun).ToArray()));
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
                    return new AditionMonthTable.Value(excused, unexcused);
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


    }
}
