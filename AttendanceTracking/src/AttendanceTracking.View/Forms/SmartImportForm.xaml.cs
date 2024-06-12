using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for SmartImportForm.xaml
    /// </summary>
    public partial class SmartImportForm : Window
    {
        public SmartImportForm()
        {
            InitializeComponent();
            UpdateTables();
        }

        private string[] _workerHeaders = new[] { "Фамилия", "Имя", "Отчество" };
        private string[] _teacherHeaders = new[] { "Фамилия", "Имя", "Отчество" };
        private string[] _studentHeaders = new[] { "Фамилия", "Имя", "Отчество", "Группа" };

        private void UpdateTables()
        {
            var workers = Data.DataContext.GetInstance().QueryReturnTable(
                "SELECT p.last_name, p.first_name, p.patronomic " +
                "FROM secretaries " +
                "LEFT JOIN peoples AS p ON secretaries.id = p.id "
            );
            for (int i = 0; i < Math.Min(_workerHeaders.Length, workers.Columns.Count); i++)
                workers.Columns[i].ColumnName = _workerHeaders[i];
            Workers.ItemsSource = workers.DefaultView;

            var teachers = Data.DataContext.GetInstance().QueryReturnTable(
                "SELECT p.last_name, p.first_name, p.patronomic " +
                "FROM teachers " +
                "LEFT JOIN peoples AS p ON teachers.id = p.id "
            );
            for (int i = 0; i < Math.Min(_teacherHeaders.Length, teachers.Columns.Count); i++)
                teachers.Columns[i].ColumnName = _teacherHeaders[i];
            Teachers.ItemsSource = teachers.DefaultView;

            var students = Data.DataContext.GetInstance().QueryReturnTable(
                "SELECT p.last_name, p.first_name, p.patronomic, g.name " +
                "FROM students " +
                "LEFT JOIN peoples AS p ON students.id = p.id " +
                "LEFT JOIN groups AS g ON students.group_id = g.id "
            );
            for (int i = 0; i < Math.Min(_studentHeaders.Length, students.Columns.Count); i++)
                students.Columns[i].ColumnName = _studentHeaders[i];
            Students.ItemsSource = students.DefaultView;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = @"C:\Users\YourUsername\Documents";
                saveFileDialog.Filter = "Файлы *.csv | *.csv|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = ((TabItem)Tabs.SelectedItem).Header + ".csv";

                if (!saveFileDialog.ShowDialog().Value)
                    return;


                switch (Tabs.SelectedIndex)
                {
                    case 0: ExportWorkers(saveFileDialog.FileName); break;
                    case 1: ExportTeachers(saveFileDialog.FileName); break;
                    case 2: ExportStudents(saveFileDialog.FileName); break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{((TabItem)Tabs.SelectedItem).Header} экспортированы неверно!\n{ex.Message}", "При экспорте произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Файлы *.csv |*.csv";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == false)
                    return;

                string filePath = openFileDialog.FileName;
                var lines = File.ReadAllLines(filePath);

                var columns = lines[0].Split(';');
                var source = lines.Skip(1).Select(r => r.Split(';')).ToArray();

                switch (Tabs.SelectedIndex)
                {
                    case 0: ImportWorkers(columns, source); break;
                    case 1: ImportTeachers(columns, source); break;
                    case 2: ImportStudents(columns, source); break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{((TabItem)Tabs.SelectedItem).Header} импортированы неверно!\n{ex.Message}", "При импорте произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportWorkers(string pathFile)
        {
            var workers = Data.DataContext.GetInstance().QueryReturn(
                "SELECT p.last_name, p.first_name, p.patronomic " +
                "FROM secretaries " +
                "LEFT JOIN peoples AS p ON secretaries.id = p.id "
            );

            File.WriteAllLines(pathFile, new[] { _workerHeaders }.Concat(workers).Select(c => string.Join(";", c)), Encoding.UTF8);
        }

        private void ImportWorkers(string[] columns, params string[][] source)
        {
            foreach (var row in source)
                try
                {
                    var id = InsertPeople(
                        row[GetIndex(_workerHeaders[0], columns)],
                        row[GetIndex(_workerHeaders[1], columns)],
                        row[GetIndex(_workerHeaders[2], columns)]
                    );

                    Data.DataContext.GetInstance().QueryExecute(
                        "INSERT INTO secretaries (id) VALUES (@0)", id
                    );
                }
                catch
                {
                    throw new Exception("Неверная структура таблицы");
                }
            UpdateTables();
        }

        private void ExportTeachers(string pathFile)
        {
            var teachers = Data.DataContext.GetInstance().QueryReturn(
                "SELECT p.last_name, p.first_name, p.patronomic " +
                "FROM teachers " +
                "LEFT JOIN peoples AS p ON teachers.id = p.id "
            );

            File.WriteAllLines(pathFile, new[] { _teacherHeaders }.Concat(teachers).Select(c => string.Join(";", c)), Encoding.UTF8);
        }

        private void ImportTeachers(string[] columns, params string[][] source)
        {
            foreach (var row in source)
                try
                {
                    var id = InsertPeople(
                        row[GetIndex(_teacherHeaders[0], columns)],
                        row[GetIndex(_teacherHeaders[1], columns)],
                        row[GetIndex(_teacherHeaders[2], columns)]
                    );

                    Data.DataContext.GetInstance().QueryExecute(
                        "INSERT INTO teachers (id) VALUES (@0)", id    
                    );
                }
                catch
                {
                    throw new Exception("Неверная структура таблицы");
                }
            UpdateTables();
        }

        private int GetIndex(string origin, string[] currentColumns) =>
            currentColumns.ToList().IndexOf(origin);

        private void ExportStudents(string pathFile)
        {
            var students = Data.DataContext.GetInstance().QueryReturn(
                "SELECT p.last_name, p.first_name, p.patronomic, g.name " +
                "FROM students " +
                "LEFT JOIN peoples AS p ON students.id = p.id " +
                "LEFT JOIN groups AS g ON students.group_id = g.id "
            );

            File.WriteAllLines(pathFile, new[] { _studentHeaders }.Concat(students).Select(c => string.Join(";", c)), Encoding.UTF8);
        }

        private void ImportStudents(string[] columns, params string[][] source)
        {
            foreach (var row in source)
                try
                {
                    var id = InsertPeople(
                        row[GetIndex(_studentHeaders[0], columns)],
                        row[GetIndex(_studentHeaders[1], columns)],
                        row[GetIndex(_studentHeaders[2], columns)]
                    );

                    var groupId = int.Parse(
                            Data.DataContext.GetInstance().QueryReturn(
                            "SELECT id FROM groups WHERE name = @0",
                            row[GetIndex(_studentHeaders[3], columns)]
                        )?.SingleOrDefault()
                        ?.SingleOrDefault() ?? "-1"
                    );
                    if (groupId < 0) groupId = int.Parse(
                        Data.DataContext.GetInstance().QueryReturn(
                             "INSERT INTO groups (name) VALUES (@0); " +
                             "SELECT last_insert_id()",
                            row[GetIndex(_studentHeaders[3], columns)]
                        )?.SingleOrDefault()
                        ?.SingleOrDefault() ?? "-1"
                    );

                    Data.DataContext.GetInstance().QueryExecute(
                        "INSERT INTO students (id, group_id) VALUES (@0, @1)", id, groupId
                    );
                }
                catch
                {
                    throw new Exception("Неверная структура таблицы");
                }
            UpdateTables();
        }

        private int InsertPeople(string lastname, string firstname, string patronomic)
        {
            return int.Parse(
                    Data.DataContext.GetInstance().QueryReturn(
                    "INSERT " +
                    "INTO peoples (last_name, first_name, patronomic) " +
                    "VALUES (@0, @1, @2); " +
                    "SELECT last_insert_id()",
                    lastname, firstname, patronomic
                )?.SingleOrDefault()
                ?.SingleOrDefault()
            );
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var form = new ImportForm();
            form.Show();
            form.Closed += (s, ev) => Show();
            Hide();
        }
    }
}
