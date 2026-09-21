using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class SubjectViewControl : UserControl
    {
        private readonly CourseRepository _courseRepo = new CourseRepository();

        private DataGridView _dgvCourses;
        private TextBox _txtCode;
        private TextBox _txtName;
        private TextBox _txtUnits;

        private Button _btnAdd;
        private Button _btnUpdate;
        private Button _btnDelete;
        private Button _btnClear;

        private List<Course> _allCourses = new List<Course>();
        private bool _loading;

        public SubjectViewControl()
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

            // Title
            Label lblTitle = new Label
            {
                Text = "Subject / Course Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 30, 55),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            container.Controls.Add(lblTitle);

            // DataGridView (Left side)
            _dgvCourses = new DataGridView
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
            _dgvCourses.EnableHeadersVisualStyles = false;
            _dgvCourses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 232, 250);
            _dgvCourses.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 40, 80);
            _dgvCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _dgvCourses.ColumnHeadersHeight = 35;
            _dgvCourses.RowTemplate.Height = 32;

            _dgvCourses.Columns.Add("ID", "ID");
            _dgvCourses.Columns["ID"].Visible = false;
            _dgvCourses.Columns.Add("Code", "Subject Code");
            _dgvCourses.Columns.Add("Name", "Subject Name");
            _dgvCourses.Columns.Add("Units", "Units");

            _dgvCourses.SelectionChanged += DgvCourses_SelectionChanged;
            container.Controls.Add(_dgvCourses);

            // Action Card (Right side)
            GroupBox grpDetails = new GroupBox
            {
                Text = "Subject Details",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 75),
                Location = new Point(625, 68),
                Width = 330,
                Height = 537,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int startY = 40;
            int gapY = 80;

            // Code
            grpDetails.Controls.Add(CreateLabel("Subject Code *", 20, startY));
            _txtCode = CreateTextBox(20, startY + 22, 280);
            grpDetails.Controls.Add(_txtCode);

            // Name
            grpDetails.Controls.Add(CreateLabel("Subject Name *", 20, startY + gapY));
            _txtName = CreateTextBox(20, startY + gapY + 22, 280);
            grpDetails.Controls.Add(_txtName);

            // Units
            grpDetails.Controls.Add(CreateLabel("Units *", 20, startY + gapY * 2));
            _txtUnits = CreateTextBox(20, startY + gapY * 2 + 22, 280);
            _txtUnits.Text = "3";
            grpDetails.Controls.Add(_txtUnits);

            // Buttons
            _btnAdd = CreateButton("Add Subject", Color.FromArgb(13, 110, 253), 20, 330, 135);
            _btnAdd.Click += BtnAdd_Click;

            _btnUpdate = CreateButton("Update", Color.FromArgb(25, 135, 84), 165, 330, 135);
            _btnUpdate.Click += BtnUpdate_Click;

            _btnDelete = CreateButton("Delete", Color.FromArgb(220, 53, 69), 20, 380, 135);
            _btnDelete.Click += BtnDelete_Click;

            _btnClear = CreateButton("Clear", Color.FromArgb(108, 117, 125), 165, 380, 135);
            _btnClear.Click += BtnClear_Click;

            grpDetails.Controls.Add(_btnAdd);
            grpDetails.Controls.Add(_btnUpdate);
            grpDetails.Controls.Add(_btnDelete);
            grpDetails.Controls.Add(_btnClear);

            container.Controls.Add(grpDetails);
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

        private static TextBox CreateTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Segoe UI", 10F)
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
            _loading = true;
            try
            {
                _allCourses = _courseRepo.GetAll();
                _dgvCourses.Rows.Clear();

                foreach (var c in _allCourses)
                {
                    _dgvCourses.Rows.Add(c.Id, c.Code, c.Name, c.Units);
                }

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading subjects: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loading = false;
            }
        }

        private void DgvCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading || _dgvCourses.SelectedRows.Count == 0) return;

            DataGridViewRow row = _dgvCourses.SelectedRows[0];
            int courseId = Convert.ToInt32(row.Cells["ID"].Value);
            Course c = _allCourses.FirstOrDefault(x => x.Id == courseId);

            if (c != null)
            {
                _txtCode.Text = c.Code;
                _txtName.Text = c.Name;
                _txtUnits.Text = c.Units.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out byte units)) return;

            Course c = new Course
            {
                Code = _txtCode.Text.Trim(),
                Name = _txtName.Text.Trim(),
                Units = units
            };

            try
            {
                _courseRepo.Create(c);
                MessageBox.Show("Subject added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error adding subject: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a subject to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs(out byte units)) return;

            int courseId = Convert.ToInt32(_dgvCourses.SelectedRows[0].Cells["ID"].Value);
            Course c = new Course
            {
                Id = courseId,
                Code = _txtCode.Text.Trim(),
                Name = _txtName.Text.Trim(),
                Units = units
            };

            try
            {
                _courseRepo.Update(c);
                MessageBox.Show("Subject updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error updating subject: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a subject to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int courseId = Convert.ToInt32(_dgvCourses.SelectedRows[0].Cells["ID"].Value);
            string code = _dgvCourses.SelectedRows[0].Cells["Code"].Value.ToString();

            if (MessageBox.Show(string.Format("Are you sure you want to delete {0}?", code), "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _courseRepo.Delete(courseId);
                    MessageBox.Show("Subject deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error deleting subject: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            _dgvCourses.ClearSelection();
        }

        private void ClearForm()
        {
            _txtCode.Clear();
            _txtName.Clear();
            _txtUnits.Text = "3";
        }

        private bool ValidateInputs(out byte units)
        {
            units = 3;
            if (string.IsNullOrWhiteSpace(_txtCode.Text) || string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Subject Code and Subject Name are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!byte.TryParse(_txtUnits.Text.Trim(), out units) || units == 0)
            {
                MessageBox.Show("Please enter a valid positive number for units.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}
