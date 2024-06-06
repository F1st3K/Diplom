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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private Account _account;

        private RoleService _roles = new RoleService();

        public Menu(Account account)
        {
            _account = account;
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

        private void AttendenceButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new AttendanceAccountingForm(_roles.GetGroupIdByStudent(_account.Id));
            form.Show();
            form.Closed += (s, ev) => Show();
            Hide();
        }

        private void ViewMonthAttendanceButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new MonthAttendanceViewForm(_roles.GetGroupIdByCurator(_account.Id));
            form.Show();
            form.Closed += (s, ev) => Show();
            Hide();
        }

        private void ViewAttendanceButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new AttendanceViewForm(_roles.GetGroupIdByCurator(_account.Id));
            form.Show();
            form.Closed += (s, ev) => Show();
            Hide();
        }

        private void AppointLeaderButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new AppointmentLeaderForm(_roles.GetGroupIdByCurator(_account.Id));
            form.Show();
            form.Closed += (s, ev) => Show();
            Hide();
        }

        private void AppointCuratorsButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new AppointmentCuratorsForm();
            form.Show();
            form.Closed += (s, ev) => Show();
            Hide();
        }

        private void AccountsButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new AccountsViewForm();
            form.Show();
            form.Closed += (s, ev) => Show();
            Hide();
        }

        private void ImportTablesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
