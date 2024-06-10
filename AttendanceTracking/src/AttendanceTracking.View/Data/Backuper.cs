using MySql.Data.MySqlClient;
using System;

namespace AttendanceTracking.View.Data
{
    static class Backuper
    {
        public static void Backup(this DataContext context, string pathFile)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = context.Connection;
                        context.Connection.Open();
                        mb.ExportToFile(pathFile);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger(ex.Message);
            }
            context.Connection.Close();
        }

        public static void Restore(this DataContext context, string pathFile)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = context.Connection;
                        context.Connection.Open();
                        mb.ImportFromFile(pathFile);
                        context.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger(ex.Message);
            }
            context.Connection.Close();
        }
    }
}
