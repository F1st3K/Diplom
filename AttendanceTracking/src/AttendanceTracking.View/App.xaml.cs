using AttendanceTracking.View.Data;
using AttendanceTracking.View.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AttendanceTracking.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string DbConnectionString = ConfigurationManager.AppSettings["DbConnectionString"];
        //dependensy for data base (mySql)
            _ = new DataContext(DbConnectionString,
                log => MessageBox.Show(log,
                    "Внимание, сбой в работе БД!", MessageBoxButton.OK, MessageBoxImage.Asterisk));

            DataContext.GetInstance().TestConnection();

            //make backup on leave the programm
            Current.Exit += (av, ev) => 
                DataContext.GetInstance()
                    .Backup($"{Environment.CurrentDirectory}\\Backups\\{DateTime.Now:[yyyy-MM-dd](HH-mm-ss)}.sql");
        }
    }
}
