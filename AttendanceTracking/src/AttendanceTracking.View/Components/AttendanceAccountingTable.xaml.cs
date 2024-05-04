using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AttendanceTracking.View.Components
{
    /// <summary>
    /// Interaction logic for AttendanceAccountingTable.xaml
    /// </summary>
    public partial class AttendanceAccountingTable : UserControl
    {
        private MonthTable MonthTable;

        public Func<DateTime, IEnumerable<MonthTable.Value>> GetHoursQuery = 
            date => Enumerable.Empty<MonthTable.Value>();
        public Action<MonthTable.Value> EditHoursCommand = 
            value => { };

        public AttendanceAccountingTable()
        {
            InitializeComponent();
            GetHoursQuery = d => new MonthTable.Value[] { new MonthTable.Value(0, 8, 8, true)};
            EditHoursCommand = e => MessageBox.Show($"({e.RowIndex},{e.Day}): {e.Hours} ->{e.IsExcused}");
            InitMonthDataGrid(DateTime.Now);
        }

        private void InitMonthDataGrid(DateTime date)
        {
            if (MonthTable != null)
            {
                MonthTable.ChangeHours -= MonthTable_ChangeHours;
                MonthDataGrid.Children.Clear();
            }
            MonthTable = MonthTable.Create(date, 25, GetHoursQuery?.Invoke(date).ToArray());
            MonthTable.ChangeHours += MonthTable_ChangeHours;
            MonthDataGrid.Children.Add(MonthTable);
        }

        private void MonthTable_ChangeHours(object sender, MonthTable.Value e)
        {
            EditHoursCommand?.Invoke(e);
        }
    }
}
