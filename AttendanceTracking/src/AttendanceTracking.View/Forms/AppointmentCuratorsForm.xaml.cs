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
    /// Interaction logic for AppointmentCurators.xaml
    /// </summary>
    public partial class AppointmentCuratorsForm : Window
    {
        
        

        private IEnumerable<Group> _groups;
        private IEnumerable<Prepod> _prepods;
        private Action<int, int> _editCurtorGroupCommand;

        private SearchService _searcher = new SearchService();

        public AppointmentCuratorsForm()
        {
            var studentsService = new GroupService();

            InitializeComponent();
            _groups = studentsService.GetAllGroups();
            _prepods = studentsService.GetAllPrepods();
            _editCurtorGroupCommand = (gi, pi) => studentsService.EditCuratorGroup(gi, pi);
            Groups.ItemsSource = _groups.Select(toStr);
            Prepods.ItemsSource = _prepods.Select((p, i) => $"{i+1}. {p.FullName}");
        }

        private void SetLeader_Click(object sender, RoutedEventArgs e)
        {
            if (Groups.SelectedIndex < 0
                || Groups.SelectedIndex >= _groups.Count()
                || Prepods.SelectedIndex < 0
                || Prepods.SelectedIndex >= _prepods.Count())
                return;
            var group = _groups.ElementAt(Groups.SelectedIndex);
            var curator = _prepods.ElementAt(Prepods.SelectedIndex);
            group.CuratorId = curator.Id;
            Groups.ItemsSource = Groups.ItemsSource.Cast<string>().ToArray();
            GroupText.Text = group.Name;
            LeaderText.Text = curator.FullName;
            
            _editCurtorGroupCommand.Invoke(group.Id, group.CuratorId);
        }

        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Groups.SelectedIndex < 0
                || Groups.SelectedIndex >= _groups.Count())
                return;
            var group = _groups.ElementAt(Groups.SelectedIndex);
            Prepods.SelectedIndex = _prepods.ToList().FindIndex(p => p.Id == group.CuratorId);
            var curator = _prepods.ElementAt(Prepods.SelectedIndex);
            GroupText.Text = group.Name;
            LeaderText.Text = curator.FullName;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var groups = _groups.ToList();
            groups.Sort((x, y) => 
            {
                var distX = _searcher.GetLevenshteinDistance(toStr(x), SearchBox.Text);
                var distY = _searcher.GetLevenshteinDistance(toStr(y), SearchBox.Text);
                return distX.CompareTo(distY);
            });
            _groups = groups;
            Groups.ItemsSource = _groups.Select(toStr);
        }
        string toStr(Group g) =>
                    $"{g.Name}\t-  { _prepods.FirstOrDefault(c => c.Id == g.CuratorId)?.FullName ?? "нет"}";
    }
}
