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
    /// Interaction logic for AppointmentLeaderForm.xaml
    /// </summary>
    public partial class AppointmentLeaderForm : Window
    {

        public AppointmentLeaderForm(string group, string[] students, int leaderId)
        {
            InitializeComponent();
            GroupText.Text = group;
            LeaderText.Text = students[leaderId];
            Students.ItemsSource = students.Select((s, i) => (i + 1) + ". " + s);
            Students.SelectedIndex = leaderId;
        }
    }
}
