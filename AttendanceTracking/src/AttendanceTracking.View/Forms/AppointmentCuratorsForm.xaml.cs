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
        public class Propod
        {
            public int Id;
            public string FullName;
            public Propod(int id, string fullName)
            {
                Id = id;
                FullName = fullName;
            }
        }
        public class Group
        {
            public int Id;
            public string Name;
            public int CuratorId;
            public Group(int id, int curatorId, string name)
            {
                Id = id;
                CuratorId = curatorId;
                Name = name;
            }
        }

        private IEnumerable<Group> _groups;
        private IEnumerable<Propod> _prepods;
        private Action<int, int> _editCurtorGroupCommand;

        public AppointmentCuratorsForm(IEnumerable<Group> groups, IEnumerable<Propod> prepods, Action<int, int> editCuratorGroup)
        {
            InitializeComponent();
            _groups = groups;
            _prepods = prepods;
            _editCurtorGroupCommand = editCuratorGroup;
            Groups.ItemsSource = _groups.Select(g => $"{g.Name}\t-  { _prepods.FirstOrDefault(c => c.Id == g.CuratorId)?.FullName ?? "нет"}");
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
    }
}
