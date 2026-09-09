using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class RegisterForm : Form
    {
        private readonly TextBox txtUsername;
        private readonly TextBox txtPassword;
        private readonly TextBox txtConfirm;

        public RegisterForm()
        {
            Text = "Create Account";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(400, 260);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            Font = new Font("Microsoft YaHei UI", 9F);

            Label title = new Label
            {
                Text = "Create a new account",
                Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(24, 20)
            };

            Label lblUsername = new Label { Text = "Username", Location = new Point(24, 62), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(120, 58), Size = new Size(250, 24) };

            Label lblPassword = new Label { Text = "Password", Location = new Point(24, 96), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(120, 92), Size = new Size(250, 24), UseSystemPasswordChar = true };

            Label lblConfirm = new Label { Text = "Confirm", Location = new Point(24, 130), AutoSize = true };
            txtConfirm = new TextBox { Location = new Point(120, 126), Size = new Size(250, 24), UseSystemPasswordChar = true };

            Button btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(120, 170),
                Size = new Size(110, 34)
            };
            btnRegister.Click += btnRegister_Click;

            Button btnCancel = new Button
            {
                Text = "Back to login",
                Location = new Point(250, 170),
                Size = new Size(120, 34)
            };
            btnCancel.Click += (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.Add(title);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(lblConfirm);
            Controls.Add(txtConfirm);
            Controls.Add(btnRegister);
            Controls.Add(btnCancel);

            AcceptButton = btnRegister;
            CancelButton = btnCancel;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            if (username.Length == 0 || password.Length == 0 || confirm.Length == 0)
            {
                MessageBox.Show(this, "Please fill in all fields.", "Register",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show(this, "Passwords do not match.", "Register",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User created = new UserRepository().CreateUser(username, password, "user");

                if (created == null)
                {
                    MessageBox.Show(this, "That username is already taken. Please choose another.",
                        "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}