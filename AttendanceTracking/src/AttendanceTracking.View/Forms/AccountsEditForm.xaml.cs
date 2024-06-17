using AttendanceTracking.View.Entities;
using AttendanceTracking.View.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AccountsEditForm.xaml
    /// </summary>
    public partial class AccountsEditForm : Window
    {
        public delegate void EditHandler(Account account);
        public event EditHandler Edited;

        private HashService _hasher = new HashService();

        private Account _account;

        public AccountsEditForm(Account account)
        {
            _account = account;
            InitializeComponent();
            Edited += p => Close();

            Fullname.Text = account.FullName;
            TextRoles.Text = account.TextRoles;
            Login.Text = account.Login;
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            _account.Login = Login.Text;
            if (Password.Password != string.Empty)
            {
                _account.Hash = _hasher.Hash(Password.Password);
                _account.IsActive = false;
            }
            Edited?.Invoke(_account);
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Login_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlockCreate();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            BlockCreate();
        }
        private void BlockCreate()
        {
            string loginPattern = @"^[a-zA-Z0-9_]{4,20}$";

            Create.IsEnabled = Regex.IsMatch(Login.Text, loginPattern) &&
                Password.Password.Length >= 4;
        }
    }
}
