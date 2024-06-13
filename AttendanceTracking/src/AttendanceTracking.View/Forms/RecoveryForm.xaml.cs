using AttendanceTracking.View.Data;
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

namespace AttendanceTracking.View.Forms
{
    /// <summary>
    /// Interaction logic for RecoveryForm.xaml
    /// </summary>
    public partial class RecoveryForm : Window
    {
        private readonly string BackupsPath = $"{Environment.CurrentDirectory}\\Backups";

        public RecoveryForm()
        {
            InitializeComponent();
            UpdateBackupsList();
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            Data.DataContext.GetInstance().Backup($"{BackupsPath}\\{DateTime.Now:[yyyy-MM-dd](HH-mm-ss)}.sql");
            UpdateBackupsList();
            MessageBox.Show("Резервная коппия создана успешно!");
        }

        private void UpdateBackupsList()
        {
            try
            {
                var list = Directory.GetFiles(BackupsPath, "*.sql");
                for (int i = 0; i < list.Length; i++)
                    list[i] = list[i].Replace($"{BackupsPath}\\", string.Empty);
                BackupsList.ItemsSource = list;
                BackupsList.SelectedIndex = BackupsList.Items.Count - 1;
            }
            catch (Exception)
            {

            }

        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            var no = MessageBox.Show("Вы точно хотите востановить базу: " + BackupsList.SelectedItem, "Внимание", MessageBoxButton.YesNo);
            if (no == MessageBoxResult.No)
                return;
            Data.DataContext.GetInstance().Restore($"{BackupsPath}\\{BackupsList.Text}");
            UpdateBackupsList();
            MessageBox.Show("База востановленна успешно!");
        }
    }
}
