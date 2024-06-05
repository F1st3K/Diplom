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
    /// Interaction logic for AccountsViewForm.xaml
    /// </summary>
    public partial class AccountsViewForm : Window
    {

        private List<Account> _accounts;

        private Action<Account> _editCommand;
        private Action<Account> _deleteComand;
        private Action<Account> _createCommand;
        private Func<IEnumerable<People>> _getPeoplesQuery;

        private SearchService _searcher = new SearchService();

        public AccountsViewForm()
        {
            var accountService = new AccountService();

            _accounts = accountService.GetAccounts().ToList();
            _editCommand = a => accountService.EditAccount(a);
            _deleteComand = a => accountService.DeleteAccount(a);
            _createCommand = a => accountService.CreateAccount(a);
            _getPeoplesQuery = () => accountService.GetPeoples();

            InitializeComponent();

            AccountsTable.ItemsSource = _accounts;
            
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
                AccountsTable.ItemsSource = filtered(_accounts);
                AccountsTable.Focus();
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
                AccountsTable.ItemsSource = filtered(_accounts);

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
                _createCommand?.Invoke(account);

                _accounts.Insert(0, account);
                RoleBox.SelectedIndex = 0;
                AccountsTable.ItemsSource = _accounts;
                AccountsTable.SelectedIndex = 0;
                AccountsTable.Focus();
            };

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _accounts.Sort((x, y) =>
            {
                var distX = _searcher.GetLevenshteinDistance($"{x.FullName} {x.Login}", SearchBox.Text);
                var distY = _searcher.GetLevenshteinDistance($"{y.FullName} {y.Login}", SearchBox.Text);
                return distX.CompareTo(distY);
            });
            AccountsTable.ItemsSource = filtered(_accounts);
        }

        private Func<IEnumerable<Account>, IEnumerable<Account>> filtered;
        private void RoleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (RoleBox.SelectedIndex)
            {
                case 1:
                    filtered = acs => acs.Where(a => a.People.Roles.Contains(Roles.Role.Administrator)); break;
                case 2:
                    filtered = acs => acs.Where(a => a.People.Roles.Contains(Roles.Role.Secretary)); break;
                case 3:
                    filtered = acs => acs.Where(a => a.People.Roles.Contains(Roles.Role.Teacher)); break;
                case 4:
                    filtered = acs => acs.Where(a => a.People.Roles.Contains(Roles.Role.Curator)); break;
                case 5:
                    filtered = acs => acs.Where(a => a.People.Roles.Contains(Roles.Role.Student)); break;
                case 6:
                    filtered = acs => acs.Where(a => a.People.Roles.Contains(Roles.Role.Leader)); break;
                default:
                    filtered = acs => acs.Select(a => a); break;
            }
            if (AccountsTable != null)
                AccountsTable.ItemsSource = filtered(_accounts);
        }
    }

    
}
