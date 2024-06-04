using AttendanceTracking.View.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
            //dependensy for data base (mySql)
            _ = new DataContext(
                "host='localhost';" +
                "database='technical_college';" +
                "uid='root';" +
                "pwd='root';" +
                "charset=utf8;",
            log => MessageBox.Show(log,
                "Внимание, сбой в работе БД!", MessageBoxButton.OK, MessageBoxImage.Asterisk));

            DataContext.GetInstance().TestConnection();
        }
    }
}
