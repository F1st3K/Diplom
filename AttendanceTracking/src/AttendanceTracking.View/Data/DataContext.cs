using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace AttendanceTracking.View.Data
{
    public class DataContext
    {
        private static DataContext _instance;
        private string _connectionString;
        private MySqlConnection _dbConection;

        public Action<string> Logger;

        public DataContext(string connectionString, Action<string> logger)
        {
            _instance = this;
            _connectionString = connectionString;
            _dbConection = new MySqlConnection(_connectionString);
            Logger = logger;
        }

        public static DataContext GetInstance()
        {
            if (_instance == null)
                throw new ArgumentNullException("Instace is null, create him.");
            return _instance;
        }

        public MySqlConnection Connection
        {
            get { return _dbConection; }
        }

        public void TestConnection()
        {
            try
            {
                _dbConection.Open();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
            _dbConection.Close();
        }

        public string[][] QueryReturn(string query, params object[] parameters)
        {
            var table = new List<List<string>>();
            try
            {
                _dbConection.Open();
                MySqlCommand command = new MySqlCommand(query, _dbConection);
                for (int i = 0; i < parameters.Length; i++)
                    command.Parameters.AddWithValue("@" + i, parameters[i]);
                var reader = command.ExecuteReader();
                int numCols = reader.FieldCount;
                for (int i = 0; reader.Read(); i++)
                {
                    var row = new List<string>();
                    for (int j = 0; j < numCols; j++)
                        row.Add(reader.GetString(j));
                    table.Add(row);
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message + "\n" + query);
            }
            _dbConection.Close();
            return table.ConvertAll(row => row.ToArray()).ToArray();
        }

        public void QueryExecute(string query, params object[] parameters)
        {
            try
            {
                _dbConection.Open();
                MySqlCommand command = new MySqlCommand(query, _dbConection);
                for (int i = 0; i < parameters.Length; i++)
                    command.Parameters.AddWithValue("@" + i, parameters[i]);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger(ex.Message + "\n" + query);
            }
            _dbConection.Close();
        }
    }
}