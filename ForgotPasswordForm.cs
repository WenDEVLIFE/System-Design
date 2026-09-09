using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class ForgotPasswordForm : Form
    {
        private readonly TextBox txtUsername;
        private readonly TextBox txtNewPassword;
        private readonly TextBox txtConfirm;

        public ForgotPasswordForm()
        {
            Text = "Reset Password";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(400, 260);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            Font = new Font("Microsoft YaHei UI", 9F);

            Label title = new Label
            {
                Text = "Reset your password",
                Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            Label lblUsername = new Label { Text = "Username", Location = new Point(24, 62), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(120, 58), Size = new Size(250, 24) };

            Label lblNewPassword = new Label { Text = "New password", Location = new Point(24, 96), AutoSize = true };
            txtNewPassword = new TextBox { Location = new Point(120, 92), Size = new Size(250, 24), UseSystemPasswordChar = true };

            Label lblConfirm = new Label { Text = "Confirm", Location = new Point(24, 130), AutoSize = true };
            txtConfirm = new TextBox { Location = new Point(120, 126), Size = new Size(250, 24), UseSystemPasswordChar = true };

            Button btnReset = new Button
            {
                Text = "Reset password",
                Location = new Point(120, 170),
                Size = new Size(130, 34)
            };
            btnReset.Click += btnReset_Click;

            Button btnCancel = new Button
            {
                Text = "Back to login",
                Location = new Point(260, 170),
                Size = new Size(110, 34)
            };
            btnCancel.Click += (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.Add(title);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblNewPassword);
            Controls.Add(txtNewPassword);
            Controls.Add(lblConfirm);
            Controls.Add(txtConfirm);
            Controls.Add(btnReset);
            Controls.Add(btnCancel);

            AcceptButton = btnReset;
            CancelButton = btnCancel;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string newPassword = txtNewPassword.Text;
            string confirm = txtConfirm.Text;

            if (username.Length == 0 || newPassword.Length == 0 || confirm.Length == 0)
            {
                MessageBox.Show(this, "Please fill in all fields.", "Reset Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirm)
            {
                MessageBox.Show(this, "Passwords do not match.", "Reset Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool updated = new UserRepository().UpdatePassword(username, newPassword);

                if (!updated)
                {
                    MessageBox.Show(this, "No account was found with that username.",
                        "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (MySqlException)
            {
                MessageBox.Show(this, "Unable to reach the database. Please make sure MySQL is running.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "An unexpected error occurred. Please try again.",
                    "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}