using System;
using System.Collections.Generic;
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
using AttendanceTracking.View;
using AttendanceTracking.View.Services;
using Microsoft.Win32;

namespace AttendanceTracking.View.Forms
{
    /// <summary>
    /// Interaction logic for ImportForm.xaml
    /// </summary>
    public partial class ImportForm : Window
    {
        public ImportForm()
        {
            InitializeComponent();
            UpdateTables();
        }

        private ImportExportService _import = new ImportExportService();
        private string[] TableNames = new[]
        {
            "peoples",
            "administrators",
            "secretaries",
            "teachers",
            "groups",
            "group_curators",
            "students",
            "group_leaders",
            "passes",
        };

        private void UpdateTables()
        {
            Peoples.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM peoples").DefaultView;

            Administrators.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM administrators").DefaultView;

            Secretaries.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM secretaries").DefaultView;

            Teachers.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM teachers").DefaultView;

            Groups.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM groups").DefaultView;

            Curators.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM group_curators").DefaultView;

            Students.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM students").DefaultView;

            Leaders.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM group_leaders").DefaultView;

            Passes.ItemsSource = Data.DataContext.GetInstance()
                .QueryReturnTable("SELECT * FROM passes").DefaultView;
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
                saveFileDialog.FileName = TableNames[Tabs.SelectedIndex] + ".csv";

                if (saveFileDialog.ShowDialog().Value)
                {
                    string filePath = saveFileDialog.FileName;

                    var source = _import.ExportTable(TableNames[Tabs.SelectedIndex]).Select(r => string.Join(";", r));

                    File.WriteAllLines(filePath, source, Encoding.UTF8);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Файл не доступен для экспорта. Проверьте его доступность, и попробуйте еще раз.", "Ошибка файла", MessageBoxButton.OK, MessageBoxImage.Stop);
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
            
                if (_import.ImportTable(TableNames[Tabs.SelectedIndex], columns, source) == false)
                    MessageBox.Show("Неверный импорт, попробуйте еще раз.", "Ошибка пользователя", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (Exception)
            {
                MessageBox.Show("Файл не доступен для импорта. Проверьте его доступность и привильность, и попробуйте еще раз.", "Ошибка файла", MessageBoxButton.OK, MessageBoxImage.Stop);
            }

            UpdateTables();
        }

        private void DeleteTable_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы правда хотите затереть все записи из таблицы {TableNames[Tabs.SelectedIndex]} и с ними связаные?", "Уверены?", MessageBoxButton.YesNo)
                == MessageBoxResult.No)
                return;
            _import.ClearTable(TableNames[Tabs.SelectedIndex]);
            UpdateTables();
        }
    }
}
