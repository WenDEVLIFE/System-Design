using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using StudentSystem;

namespace System_Design
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Schema.EnsureAll();
            }
            catch (MySqlException)
            {
                MessageBox.Show(
                    "Unable to reach the database. Please make sure MySQL is running and the " +
                    "'student_management' database exists (import database.sql), then try again.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "An unexpected error occurred while initializing the database. Please try again.",
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Form1());
        }
    }
}
