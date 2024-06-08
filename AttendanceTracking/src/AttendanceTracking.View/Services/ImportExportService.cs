using AttendanceTracking.View.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AttendanceTracking.View.Services
{
    class ImportExportService
    {
        public bool ImportTable(string table, string[] columns, string[][] allLines)
        {
            try
            {
                string query = $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES";

                foreach (var values in allLines)
                {
                    string row = "(";
                    foreach (string value in values)
                    {
                        if (Regex.Match(value, @"[a-zA-z]|[а-яА-я]").Success)
                            row += "'" + value + "'" + ", ";
                        else
                            row += value + ", ";
                    }
                    row = row.Substring(0, row.Length - 2) + "), ";
                    query += row;
                }
                query = query.Substring(0, query.Length - 2);

                DataContext.GetInstance().QueryExecute(query);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public string[][] ExportTable(string table)
        {
            var headersData = DataContext.GetInstance().QueryReturnTable(
                $"SHOW COLUMNS FROM {table}"
            ).Rows;

            var headers = new List<string>();
            for (int i = 0; i < headersData.Count; i++)
                headers.Add(headersData[i][0].ToString());
            

            var source = DataContext.GetInstance().QueryReturn(
                $"SELECT * FROM {table}"
            );

            var data = new List<string[]>();
            data.Add(headers.ToArray());
            data.AddRange(source);

            return data.ToArray();
        }

        public void ClearTable(string table)
        {
            DataContext.GetInstance().QueryExecute(
                $"DELETE FROM {table}"
            );
        }
    }
}
