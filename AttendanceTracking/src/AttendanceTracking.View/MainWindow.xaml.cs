using AttendanceTracking.View.Components;
using AttendanceTracking.View.Entities;
using AttendanceTracking.View.Forms;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Menu = AttendanceTracking.View.Forms.Menu;

namespace AttendanceTracking.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var form = new AttendanceAccountingForm(1);
            form.Show();
            var formView = new MonthAttendanceViewForm(1);
            formView.Show();
            var formLeader = new AppointmentLeaderForm(1);
            formLeader.Show();
            var formCurators = new AppointmentCuratorsForm();
            formCurators.Show();
            new AuthForm().Show();
            var attView = new AttendanceViewForm(1);
            attView.Show();
            var av = new AccountsViewForm();
            av.Show();
        }
    }
}
