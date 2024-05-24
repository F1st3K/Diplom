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
        public class Student
        {
            public int Id;
            public string FullName;
            public Student(int id, string fullName)
            {
                Id = id;
                FullName = fullName;
            }
        }

        private Action<int> _editLeaderIdCommand;
        private IEnumerable<Student> _students;

        public AppointmentLeaderForm(string group, IEnumerable<Student> students, int leaderId, Action<int> editLeaderId)
        {
            InitializeComponent();
            GroupText.Text = group;
            LeaderText.Text = students.FirstOrDefault(s => s.Id == leaderId)?.FullName ?? "нет";
            Students.ItemsSource = students.Select((s, i) => (i + 1) + ". " + s.FullName);
            _students = students;
            Students.SelectedIndex = leaderId;
            _editLeaderIdCommand = editLeaderId;
        }

        private void SetLeader_Click(object sender, RoutedEventArgs e)
        {
            _editLeaderIdCommand?.Invoke(_students.ElementAt(Students.SelectedIndex).Id);
        }
    }
}
