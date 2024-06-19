using AttendanceTracking.View.Components;
using AttendanceTracking.View.Entities;
using AttendanceTracking.View.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

            string forNullEmpty(int n) => n == 0 ? string.Empty : n.ToString();
            MonthTable.ChangeHours += (o, a) => 
            {
                var values = _getHoursQuery?.Invoke(date);
                AditionalTable.ItemsSource = Enumerable.Range(0, StudentsTable.Items.Count)
                .Select(i => new
                {
                    Excused = forNullEmpty(values.Sum(v => v.StudentIndex == i && v.IsExcused ? v.Hours : 0)),
                    Unexcused = forNullEmpty(values.Sum(v => v.StudentIndex == i && !v.IsExcused ? v.Hours : 0)),
                })
                .Concat(new[] { new {
                    Excused = forNullEmpty(values.Sum(v => v.IsExcused ? v.Hours : 0)),
                    Unexcused = forNullEmpty(values.Sum(v => !v.IsExcused ? v.Hours : 0)),
                }});
                var t = new DispatcherTimer();
                t.Interval = TimeSpan.FromMilliseconds(10);
                t.Tick += (ov, v) => { ColorAditinal(); t.Stop(); };
                t.Start();
            };

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
