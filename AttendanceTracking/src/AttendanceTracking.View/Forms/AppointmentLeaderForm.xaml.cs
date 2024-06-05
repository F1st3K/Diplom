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
    /// Interaction logic for AppointmentLeaderForm.xaml
    /// </summary>
    public partial class AppointmentLeaderForm : Window
    {
        

        private Action<int> _editLeaderIdCommand;
        private IEnumerable<Student> _students;

        public AppointmentLeaderForm(int groupId)
        {
            var studentsService = new GroupService();

            InitializeComponent();
            GroupText.Text = studentsService.GetGroupName(groupId);
            LeaderText.Text = studentsService.GetStudentsByGroup(groupId)
                .FirstOrDefault(s => s.Id == studentsService.GetLeaderIdByGroup(groupId))?.FullName ?? "нет";
            _students = studentsService.GetStudentsByGroup(groupId);
            Students.ItemsSource = _students.Select((s, i) => (i + 1) + ". " + s.FullName);
            Students.SelectedIndex = studentsService.GetLeaderIndexInGroup(groupId);
            _editLeaderIdCommand = li => studentsService.EditLeaderIdByGroup(li, groupId);
        }

        private void SetLeader_Click(object sender, RoutedEventArgs e)
        {
            var leader = _students.ElementAt(Students.SelectedIndex);
            LeaderText.Text = leader.FullName;
            _editLeaderIdCommand?.Invoke(leader.Id);
        }
    }
}
