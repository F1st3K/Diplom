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
    /// Interaction logic for PasswordForm.xaml
    /// </summary>
    public partial class PasswordForm : Window
    {
        private HashService _hasher = new HashService();
        private AccountService _accounts = new AccountService();

        private Account _account;

        public PasswordForm(Account account)
        {
            _account = account;
            InitializeComponent();


        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            _account.Hash = _hasher.Hash(Pwd.Password);
            _account.IsActive = true;
            _accounts.EditAccount(_account);

            Close();
        }

        private void Pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Enter.IsEnabled = Pwd.Password != string.Empty && Pwd.Password == RepeatPwd.Password;
        }
    }
}
