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
                .Select((s, i) => new { Id = i + 1, FullName = s });
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
    }
}
