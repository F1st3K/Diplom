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
    /// Interaction logic for AttendanceAccountingForm.xaml
    /// </summary>
    public partial class AttendanceAccountingForm : Window
    {
        private MonthTable MonthTable;

        private Func<DateTime, IEnumerable<Attendens>> _getHoursQuery;
        private Action<DateTime, Attendens> _editHoursCommand;

        public AttendanceAccountingForm(int groupId)
        {
            var studentsService = new GroupService();
            var attendencesService = new AttendensService();

            InitializeComponent();
            TextGroup.Text = studentsService.GetGroupName(groupId);
            StudentsTable.ItemsSource = studentsService.GetStudentsByGroup(groupId)
                .Select((s, i) => new { Id = i + 1, FullName = s.FullName });
            _getHoursQuery = m => attendencesService.GetAttendenses(m, groupId);
            _editHoursCommand = (m, a) => attendencesService.EditAttendens(m, a, groupId);
            MonthSwitcher.SelectedIndex = 0;
        }

        private void InitMonthDataGrid(DateTime date)
        {
            if (MonthTable != null)
            {
                MonthTable.ChangeHours -= MonthTable_ChangeHours;
                MonthDataGrid.Children.Clear();
            }
            MonthTable = MonthTable.Create(date, StudentsTable.Items.Count, _getHoursQuery?.Invoke(date).ToArray());
            MonthTable.ChangeHours += MonthTable_ChangeHours;
            MonthDataGrid.Children.Add(MonthTable);
        }

        private string[] russianMonths = new string[] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };

        public DateTime DateMonth;

        private void MonthTable_ChangeHours(object sender, Attendens e)
        {
            _editHoursCommand?.Invoke(DateMonth, e);
        }

        
        private void CurrentMonth_Selected(object sender, RoutedEventArgs e)
        {
            DateMonth = DateTime.Today;
            InitMonthDataGrid(DateMonth);
            TextMonth.Text = russianMonths[DateMonth.Month - 1];
            TextYear.Text = DateMonth.Year.ToString();
        }

        private void PrevMonth_Selected(object sender, RoutedEventArgs e)
        {
            DateMonth = DateTime.Today.AddMonths(-1);
            InitMonthDataGrid(DateMonth);
            TextMonth.Text = russianMonths[DateMonth.Month - 1];
            TextYear.Text = DateMonth.Year.ToString();
        }
    }
}
