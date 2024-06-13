using AttendanceTracking.View.Data;
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
            string rootPath = @"C:\repos\Diplom\AttendanceTracking\src\AttendanceTracking.View";
            var header = "***********************************" + Environment.NewLine;

            var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

            var result = files.Where(f => !f.EndsWith(".Designer.cs"))
                              .Where(f => !f.EndsWith(".xaml.cs"))
                              .Where(f => !f.EndsWith(".g.i.cs"))
                              .Where(f => !f.EndsWith(".g.cs"))
                              .Where(f => !f.Contains(@"\Properties\"))
                              .Where(f => !f.Contains(@"\Resources\"))
                .Select(path => new { Name = Path.GetFileName(path), Contents = File.ReadAllText(path) })
                .Select(info =>
                    header
                + "Filename: " + info.Name + Environment.NewLine
                + header
                + info.Contents);


            var singleStr = string.Join(Environment.NewLine, result);
            Console.WriteLine(singleStr);
            File.WriteAllText(@"C:\output.txt", singleStr, Encoding.UTF8);

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
