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
    /// Interaction logic for AttendanceAccountingForm.xaml
    /// </summary>
    public partial class AttendanceAccountingForm : Window
    {
        private MonthTable MonthTable;

        private Func<DateTime, IEnumerable<MonthTable.Value>> _getHoursQuery;
        private Action<MonthTable.Value> _editHoursCommand;

        public AttendanceAccountingForm(
            string groupName,
            IEnumerable<string> students,
            Func<DateTime, IEnumerable<MonthTable.Value>> getHoursQuery,
            Action<MonthTable.Value> editHoursCommand)
        {
            InitializeComponent();
            TextGroup.Text = groupName;
            StudentsTable.ItemsSource = students.Select((s, i) => new { Id = i + 1, FullName = s });
            _getHoursQuery = getHoursQuery;
            _editHoursCommand = editHoursCommand;
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

        private void MonthTable_ChangeHours(object sender, MonthTable.Value e)
        {
            _editHoursCommand?.Invoke(e);
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
