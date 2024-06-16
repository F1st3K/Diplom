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
    /// Interaction logic for AccountsCreateForm.xaml
    /// </summary>
    public partial class AccountsCreateForm : Window
    {
        public delegate void CreateHandler(Account people);
        public event CreateHandler Created;

        private IEnumerable<People> _peoples;
        private People _selectedPeople;

        private HashService _hasher = new HashService();
        private RoleService _service = new RoleService();

        private SearchService _searcher = new SearchService();

        public AccountsCreateForm(IEnumerable<People> peoples)
        {
            _peoples = peoples;

            InitializeComponent();
            Created += p => Close();
            People.ItemsSource = ActualFilter(_peoples).Select(p => toStr(p));
        }

        private Func<IEnumerable<People>, IEnumerable<People>> ActualFilter;

        private void People_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (People.SelectedIndex < 0
                || People.SelectedIndex >= ActualFilter(_peoples).Count())
                return;
            _selectedPeople = ActualFilter(_peoples).ElementAt(People.SelectedIndex);
            Fullname.Text = _selectedPeople.FullName;
            TextRoles.Text = _selectedPeople.TextRoles;

            BlockCreate();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var peoples = _peoples.ToList();
            peoples.Sort((x, y) =>
            {
                var distX = _searcher.GetLevenshteinDistance(toStr(x), SearchBox.Text);
                var distY = _searcher.GetLevenshteinDistance(toStr(y), SearchBox.Text);
                return distX.CompareTo(distY);
            });
            _peoples = peoples;
            People.ItemsSource = ActualFilter(_peoples).Select(p => toStr(p));
        }
        string toStr(People p) => $"{p.FullName} ({p.TextRoles})";

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            var roles = _selectedPeople.Roles.ToList();
            if (AddToAdmin.IsChecked.Value &&
                roles.Contains(Roles.Role.Administrator) == false &&
                MessageBox.Show("Вы точно хотите добавить учетную запись в группу Администраторов?", "Уверены?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            { 
                _service.AddToAdministrators(_selectedPeople.Id);
                roles.Add(Roles.Role.Administrator);
                _selectedPeople.Roles = roles;
            }
            if (AddToSecretar.IsChecked.Value &&
                roles.Contains(Roles.Role.Secretary) == false &&
                MessageBox.Show("Вы точно хотите добавить учетную запись в группу Работники учебной части?", "Уверены?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _service.AddToSecretary(_selectedPeople.Id);
                roles.Add(Roles.Role.Secretary);
                _selectedPeople.Roles = roles;
            }
            Created?.Invoke(new Account(_selectedPeople, Login.Text, _hasher.Hash(Password.Password), false));
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Workers_Selected(object sender, RoutedEventArgs e)
        {
            ActualFilter = ps => ps.Where(p => !p.Roles.Contains(Roles.Role.Student));
            if (AddToAdmin != null && AddToSecretar != null && People != null)
            {
                AddToAdmin.Visibility = Visibility.Visible;
                AddToAdmin.IsChecked = false;
                AddToSecretar.Visibility = Visibility.Visible;
                AddToSecretar.IsChecked = false;
                People.ItemsSource = ActualFilter(_peoples).Select(p => toStr(p));
            }
            
        }

        private void Students_Selected(object sender, RoutedEventArgs e)
        {
            ActualFilter = ps => ps.Where(p => p.Roles.Contains(Roles.Role.Student));
            if (AddToAdmin != null && AddToSecretar != null && People != null)
            {
                AddToAdmin.Visibility = Visibility.Collapsed;
                AddToAdmin.IsChecked = false;
                AddToSecretar.Visibility = Visibility.Collapsed;
                AddToSecretar.IsChecked = false;
                People.ItemsSource = ActualFilter(_peoples).Select(p => toStr(p));
            }
            
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

            Create.IsEnabled = _selectedPeople != null &&
                Regex.IsMatch(Login.Text, loginPattern) &&
                Password.Password.Length >= 8;
        }
    }
}
