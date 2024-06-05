using AttendanceTracking.View.Entities;
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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu(Account account)
        {
            InitializeComponent();

            FullName.Text = account.FullName;
            TextRoles.Text = account.TextRoles;

            if (account.People.Roles.Contains(Roles.Role.Administrator))
            {
                AccountsButton.Visibility = Visibility.Visible;
                ImportTablesButton.Visibility = Visibility.Visible;
                BackupButton.Visibility = Visibility.Visible;
            }
            if (account.People.Roles.Contains(Roles.Role.Secretary))
            {
                AppointCuratorsButton.Visibility = Visibility.Visible;
            }
            if (account.People.Roles.Contains(Roles.Role.Curator))
            {
                AppointLeaderButton.Visibility = Visibility.Visible;
                ViewAttendanceButton.Visibility = Visibility.Visible;
                ViewMonthAttendanceButton.Visibility = Visibility.Visible;
            }
            if (account.People.Roles.Contains(Roles.Role.Leader))
            {
                AttendenceButton.Visibility = Visibility.Visible;
            }

        }
    }
}
