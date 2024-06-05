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
    /// Interaction logic for AuthForm.xaml
    /// </summary>
    public partial class AuthForm : Window
    {
        private HashService _hasher = new HashService();
        private AccountService _accounts = new AccountService();

        public AuthForm()
        {
            InitializeComponent();
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var ac = _accounts.GetAccounts().FirstOrDefault(a => a.Login == LoginBox.Text);

            if (ac == null || ac.Hash != _hasher.Hash(PasswordBox.Password))
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка входа");
                return;
            }
            PasswordBox.Password = string.Empty;
            LoginBox.Text = string.Empty;

            if (ac.IsActive == false)
            {
                var pwd = new PasswordForm(ac);
                pwd.Show();
                pwd.Closed += (a, b) => Show();
                Hide();
                return;
            }

            var menu = new Menu(ac);
            menu.Show();
            menu.Closed += (a, b) => Show();
            Hide();
        }
    }
}
