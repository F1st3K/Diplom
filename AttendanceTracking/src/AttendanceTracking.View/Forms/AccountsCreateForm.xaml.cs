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
    /// Interaction logic for AccountsCreateForm.xaml
    /// </summary>
    public partial class AccountsCreateForm : Window
    {
        public delegate void CreateHandler(Account people);
        public event CreateHandler Created;

        private IEnumerable<People> _peoples;
        private People _selectedPeople;

        private HashService _hasher = new HashService();

        private SearchService _searcher = new SearchService();

        public AccountsCreateForm(IEnumerable<People> peoples)
        {
            _peoples = peoples;

            InitializeComponent();
            Created += p => Close();
            People.ItemsSource = _peoples.Select(p => toStr(p));
        }

        private void People_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (People.SelectedIndex < 0
                || People.SelectedIndex >= _peoples.Count())
                return;
            _selectedPeople = _peoples.ElementAt(People.SelectedIndex);
            Fullname.Text = _selectedPeople.FullName;
            TextRoles.Text = _selectedPeople.TextRoles;
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
            People.ItemsSource = _peoples.Select(p => toStr(p));
        }
        string toStr(People p) => $"{p.FullName} ({p.TextRoles})";

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            Created?.Invoke(new Account(_selectedPeople, Login.Text, _hasher.Hash(Password.Text), false));
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
