using System;
using System.Drawing;
using System.Windows.Forms;

namespace System_Design
{
    internal sealed class DashboardForm : Form
    {
        public DashboardForm()
        {
            Text = "Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(720, 480);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            User user = Session.CurrentUser;

            Label welcome = new Label
            {
                Text = string.Format("Welcome, {0}!", user != null ? user.Username : "User"),
                Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 40)
            };

            Label role = new Label
            {
                Text = string.Format("Signed in as: {0}  (role: {1})",
                    user != null ? user.Username : "-",
                    user != null ? user.Role : "user"),
                AutoSize = true,
                Location = new Point(40, 90)
            };

            Button logout = new Button
            {
                Text = "Logout",
                Location = new Point(40, 160),
                Size = new Size(120, 36)
            };
            logout.Click += (sender, args) =>
            {
                Session.Logout();
                Close();
            };

            Controls.Add(welcome);
            Controls.Add(role);
            Controls.Add(logout);
        }
    }
}