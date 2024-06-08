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
using System.Windows.Threading;

namespace AttendanceTracking.View.Forms
{
    /// <summary>
    /// Interaction logic for AuthForm.xaml
    /// </summary>
    public partial class AuthForm : Window
    {
        private HashService _hasher = new HashService();
        private AccountService _accounts = new AccountService();

        private int _badTrying = 0;
        private int _maxBadTrying = 3;

        private int secondsStoped = 15;

        public int BadTrying { get => _badTrying; 
            set {
                _badTrying = value;
                CaptchaActive = _badTrying >= _maxBadTrying;
            } 
        }

        private bool _captchaActive = false;
        public bool CaptchaActive { get => _captchaActive; 
            set { 
                if (_captchaActive != value)
                {
                    _captchaActive = value;
                    if (_captchaActive) ActivateCaptcha();
                    else DeActivateCaptcha();
                }
            }
        }

        private bool _succsessCaptcha = true;

        private void DeActivateCaptcha()
        {
            TextCaptcha.Text = string.Empty;
            _succsessCaptcha = true;
            Captcha.Visibility = Visibility.Collapsed;
        }

        private void ActivateCaptcha()
        {
            _succsessCaptcha = false;
            Captcha.Visibility = Visibility.Visible;
            ViewCaptcha.Text = string.Join(string.Empty, Guid.NewGuid().ToString("N").Take(6));
        }
        private void TextCaptcha_TextChanged(object sender, TextChangedEventArgs e)
        {
            _succsessCaptcha = TextCaptcha.Text == ViewCaptcha.Text;
        }

        private void Stopped()
        {
            IsEnabled = false;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            var sec = secondsStoped;
            timer.Tick += (s, ev) =>
            {
                Title.Text = $"Блокировка формы {sec}";
                sec--;
                if (sec <= 0)
                {
                    timer.Stop();
                    Title.Text = "Авторизация";
                    IsEnabled = true;
                    BadTrying = 0;
                }
            };
            timer.Start();
        }

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
            var isSuccsess = ac != null && ac.Hash == _hasher.Hash(PasswordBox.Password) && _succsessCaptcha;

            PasswordBox.Password = string.Empty;
            LoginBox.Text = string.Empty;

            if (isSuccsess == false)
            {
                MessageBox.Show("Неверный логин или пароль!" + (CaptchaActive ? "\nНезабудте про каптчу" : ""), "Ошибка входа");
                if (CaptchaActive)
                    Stopped();
                BadTrying++;
                return;
            }
            BadTrying = 0;

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
