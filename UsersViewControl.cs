using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class UsersViewControl : UserControl
    {
        private readonly UserRepository _userRepo = new UserRepository();

        private DataGridView _dgvUsers;
        private TextBox _txtUsername;
        private TextBox _txtPassword;
        private ComboBox _cmbRole;

        private Button _btnAddUser;
        private Button _btnResetPassword;

        public UsersViewControl()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            InitializeComponents();
            LoadData();
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
                Text = "User Accounts & Role Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 30, 55),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            container.Controls.Add(lblTitle);

            // DataGridView (Left)
            _dgvUsers = new DataGridView
            {
                Location = new Point(25, 68),
                Width = 580,
                Height = 537,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 235, 245),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvUsers.EnableHeadersVisualStyles = false;
            _dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 232, 250);
            _dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 40, 80);
            _dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _dgvUsers.ColumnHeadersHeight = 35;
            _dgvUsers.RowTemplate.Height = 32;

            _dgvUsers.Columns.Add("ID", "ID");
            _dgvUsers.Columns["ID"].Visible = false;
            _dgvUsers.Columns.Add("Username", "Username");
            _dgvUsers.Columns.Add("Role", "Role / Authority");

            _dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            container.Controls.Add(_dgvUsers);

            // Add/Edit Card (Right)
            GroupBox grpUser = new GroupBox
            {
                Text = "Add / Manage User Account",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 75),
                Location = new Point(625, 68),
                Width = 330,
                Height = 537,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int y = 40;

            grpUser.Controls.Add(CreateLabel("Username *", 20, y));
            _txtUsername = new TextBox
            {
                Location = new Point(20, y + 22),
                Width = 280,
                Font = new Font("Segoe UI", 10F)
            };
            grpUser.Controls.Add(_txtUsername);

            y += 75;

            grpUser.Controls.Add(CreateLabel("Password *", 20, y));
            _txtPassword = new TextBox
            {
                Location = new Point(20, y + 22),
                Width = 280,
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 10F)
            };
            grpUser.Controls.Add(_txtPassword);

            y += 75;

            grpUser.Controls.Add(CreateLabel("Role", 20, y));
            _cmbRole = new ComboBox
            {
                Location = new Point(20, y + 22),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            _cmbRole.Items.Add("admin");
            _cmbRole.Items.Add("user");
            _cmbRole.SelectedIndex = 1;
            grpUser.Controls.Add(_cmbRole);

            _btnAddUser = CreateButton("Create User Account", Color.FromArgb(13, 110, 253), 20, 310, 280);
            _btnAddUser.Click += BtnAddUser_Click;

            _btnResetPassword = CreateButton("Reset Password for Selected User", Color.FromArgb(25, 135, 84), 20, 365, 280);
            _btnResetPassword.Click += BtnResetPassword_Click;

            grpUser.Controls.Add(_btnAddUser);
            grpUser.Controls.Add(_btnResetPassword);

            container.Controls.Add(grpUser);
        }

        private static Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 70, 85),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        private static Button CreateButton(string text, Color bg, int x, int y, int width)
        {
            Button btn = new Button
            {
                Text = text,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(x, y),
                Size = new Size(width, 38),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        public void LoadData()
        {
            try
            {
                List<User> users = _userRepo.GetAll();
                _dgvUsers.Rows.Clear();

                foreach (var u in users)
                {
                    _dgvUsers.Rows.Add(u.Id, u.Username, u.Role);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (_dgvUsers.SelectedRows.Count == 0) return;

            DataGridViewRow row = _dgvUsers.SelectedRows[0];
            _txtUsername.Text = row.Cells["Username"].Value.ToString();
            string role = row.Cells["Role"].Value.ToString();
            _cmbRole.SelectedItem = role;
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            string username = _txtUsername.Text.Trim();
            string password = _txtPassword.Text;
            string role = _cmbRole.SelectedItem != null ? _cmbRole.SelectedItem.ToString() : "user";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username and Password are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User created = _userRepo.CreateUser(username, password, role);
                if (created != null)
                {
                    MessageBox.Show("User account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _txtUsername.Clear();
                    _txtPassword.Clear();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Username already exists.", "Duplicate User", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error creating user: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            string username = _txtUsername.Text.Trim();
            string newPassword = _txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Please enter the Username and the New Password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success = _userRepo.UpdatePassword(username, newPassword);
                if (success)
                {
                    MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _txtPassword.Clear();
                }
                else
                {
                    MessageBox.Show("User account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error resetting password: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
