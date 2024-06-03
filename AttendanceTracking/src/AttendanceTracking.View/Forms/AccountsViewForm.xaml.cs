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
    /// Interaction logic for AccountsViewForm.xaml
    /// </summary>
    public partial class AccountsViewForm : Window
    {
        public class People
        {
            public int Id { get; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Patronomic { get; set; }
            public string FullName => $"{LastName} {FirstName} {Patronomic}";


            public IEnumerable<string> Roles { get; }
            public string TextRoles => string.Join(", ", Roles);


            public People(int id, string firstName, string lastName, string patronomic, IEnumerable<string> roles)
            {
                Id = id;

                FirstName = firstName;
                LastName = lastName;
                Patronomic = patronomic;

                Roles = roles;
            }
        }

        public class Account
        {
            public string Login { get; set; }

            public bool IsActive { get; set; }
            public string Status => IsActive ? "Активирована" : "Не активирована";

            public string Hash { get; set; }

            public People People { get; }
            public int Id => People.Id;
            public string FullName => People.FullName;
            public string TextRoles => People.TextRoles;



            public Account(People people, string login, string hash, bool isActive)
            {
                People = people;

                Login = login;
                IsActive = isActive;
                Hash = hash;
            }
        }

        

        private List<Account> _accounts;

        private Action<Account> _editCommand;
        private Action<Account> _deleteComand;
        private Func<Account, Account> _createCommand;
        private Func<IEnumerable<People>> _getPeoplesQuery;

        public AccountsViewForm(
            IEnumerable<Account> accounts,
            Action<Account> editAccount,
            Action<Account> deleteAccount,
            Func<Account, Account> createAccount,
            Func<IEnumerable<People>> getPeoples
            )
        {
            _accounts = accounts.ToList();
            _editCommand = editAccount;
            _deleteComand = deleteAccount;
            _createCommand = createAccount;
            _getPeoplesQuery = getPeoples;

            InitializeComponent();

            StudentsTable.ItemsSource = _accounts;
            
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var account = (Account)s.DataContext;

            var editForm = new AccountsEditForm(account);
            Hide();
            editForm.Show();

            editForm.Closed += (se, ev) => Show();
            editForm.Edited += a =>
            {
                _editCommand?.Invoke(a);
                StudentsTable.ItemsSource = _accounts.Select(p => p);
                StudentsTable.Focus();
            };
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var account = (Account)s.DataContext;

            var result = MessageBox.Show($"Вы точно хотите удалить учетную запись для {account.FullName}?", "Внимание", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                _accounts.Remove(account);
                StudentsTable.ItemsSource = _accounts.Select(p => p);

                _deleteComand?.Invoke(account);
            }
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            var createForm = new AccountsCreateForm(_getPeoplesQuery?.Invoke()
                .Where(p => _accounts.FindAll(a => a.People.Id == p.Id).Count == 0));
            Hide();
            createForm.Show();

            createForm.Closed += (s, ev) => Show();
            createForm.Created += account =>
            {
                account = _createCommand?.Invoke(account);

                _accounts.Insert(0, account);
                StudentsTable.ItemsSource = _accounts.Select(p => p);
                StudentsTable.SelectedIndex = 0;
                StudentsTable.Focus();
            };

        }
    }

    
}
