using System;
using System.Drawing;
using System.Windows.Forms;

namespace System_Design
{
    internal sealed class SettingsViewControl : UserControl
    {
        public SettingsViewControl()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Panel container = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                AutoScroll = true
            };
            Controls.Add(container);

            Label lblTitle = new Label
            {
                Text = "System Settings & Configuration",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 30, 55),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            container.Controls.Add(lblTitle);

            GroupBox grpDb = new GroupBox
            {
                Text = "Database Connection Status",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 75),
                Location = new Point(25, 70),
                Width = 930,
                Height = 160
            };

            Label lblServer = new Label
            {
                Text = "MySQL Database: Connected (student_management @ localhost:3306)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 135, 84),
                Location = new Point(20, 40),
                AutoSize = true
            };

            Button btnTestDb = new Button
            {
                Text = "Test Database Connection",
                BackColor = Color.FromArgb(13, 110, 253),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(20, 85),
                Size = new Size(220, 36),
                Cursor = Cursors.Hand
            };
            btnTestDb.FlatAppearance.BorderSize = 0;
            btnTestDb.Click += (s, e) =>
            {
                try
                {
                    using (var conn = Database.OpenConnection())
                    {
                        MessageBox.Show("MySQL Connection Successful!", "Database Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection Failed: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            grpDb.Controls.Add(lblServer);
            grpDb.Controls.Add(btnTestDb);
            container.Controls.Add(grpDb);

            // System Info Card
            GroupBox grpApp = new GroupBox
            {
                Text = "Application Information",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 75),
                Location = new Point(25, 250),
                Width = 930,
                Height = 220
            };

            Label lblInfo = new Label
            {
                Text = "System Design - Academic Student Management System\n" +
                       "Version: 1.0.0 (WinForms .NET 10.0-windows)\n" +
                       "Architecture: Tiered Architecture (Repositories, Data Access Layer, Modern Controls UI)\n" +
                       "Theme: Deep Blue Gradient Dashboard Design System",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(40, 50, 70),
                Location = new Point(20, 40),
                AutoSize = true
            };

            grpApp.Controls.Add(lblInfo);
            container.Controls.Add(grpApp);
        }
    }
}
