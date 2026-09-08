using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal static class Database
    {
        public static string ConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["StudentDb"];
                if (settings == null)
                {
                    throw new InvalidOperationException("The 'StudentDb' connection string was not found in App.config.");
                }

                return settings.ConnectionString;
            }
        }

        public static MySqlConnection OpenConnection()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}