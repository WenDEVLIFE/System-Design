using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class StudentsViewControl : UserControl
    {
        private readonly StudentRepository _studentRepo = new StudentRepository();

        private DataGridView _dgvStudents;
        private TextBox _txtSearch;

        private Button _btnAddStudent;
        private Button _btnEditStudent;
        private Button _btnDeleteStudent;
        private Button _btnRefresh;

        private List<Student> _allStudents = new List<Student>();

        public StudentsViewControl()
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
                Text = "Students Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 30, 55),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            container.Controls.Add(lblTitle);

            // Top Bar: Search + Buttons
            Label lblSearch = new Label
            {
                Text = "🔍 Search:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 70, 90),
                AutoSize = true,
                Location = new Point(25, 68)
            };
            container.Controls.Add(lblSearch);

            _txtSearch = new TextBox
            {
                Location = new Point(95, 65),
                Width = 260,
                Font = new Font("Segoe UI", 10F)
            };
            _txtSearch.TextChanged += (s, e) => FilterStudents();
            container.Controls.Add(_txtSearch);

            // Action Buttons
            _btnAddStudent = CreateButton("+ Add New Student", Color.FromArgb(11, 94, 215), 380, 63, 160);
            _btnAddStudent.Click += BtnAddStudent_Click;

            _btnEditStudent = CreateButton("✏️ Edit Selected", Color.FromArgb(25, 135, 84), 550, 63, 140);
            _btnEditStudent.Click += BtnEditStudent_Click;

            _btnDeleteStudent = CreateButton("🗑️ Delete", Color.FromArgb(220, 53, 69), 700, 63, 100);
            _btnDeleteStudent.Click += BtnDeleteStudent_Click;

            _btnRefresh = CreateButton("🔄 Refresh", Color.FromArgb(108, 117, 125), 810, 63, 110);
            _btnRefresh.Click += (s, e) => LoadData();

            container.Controls.Add(_btnAddStudent);
            container.Controls.Add(_btnEditStudent);
            container.Controls.Add(_btnDeleteStudent);
            container.Controls.Add(_btnRefresh);

            // DataGridView
            _dgvStudents = new DataGridView
            {
                Location = new Point(25, 110),
                Width = 915,
                Height = 500,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
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
            _dgvStudents.EnableHeadersVisualStyles = false;
            _dgvStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 232, 250);
            _dgvStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 40, 80);
            _dgvStudents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _dgvStudents.ColumnHeadersHeight = 35;
            _dgvStudents.RowTemplate.Height = 34;

            _dgvStudents.Columns.Add("ID", "ID");
            _dgvStudents.Columns["ID"].Visible = false;
            _dgvStudents.Columns.Add("StudentNumber", "Student ID");
            _dgvStudents.Columns.Add("FullName", "Student Name");
            _dgvStudents.Columns.Add("Course", "Course");
            _dgvStudents.Columns.Add("YearLevel", "Year Level");
            _dgvStudents.Columns.Add("Gender", "Gender");
            _dgvStudents.Columns.Add("Status", "Status");
            _dgvStudents.Columns.Add("Email", "Email");

            _dgvStudents.CellDoubleClick += (s, e) => BtnEditStudent_Click(s, e);
            container.Controls.Add(_dgvStudents);
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
                Size = new Size(width, 35),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        public void LoadData()
        {
            try
            {
                _allStudents = _studentRepo.GetAll();
                FilterStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterStudents()
        {
            string query = _txtSearch.Text.Trim().ToLower();
            _dgvStudents.Rows.Clear();

            var filtered = string.IsNullOrEmpty(query)
                ? _allStudents
                : _allStudents.Where(s =>
                    (s.StudentNumber != null && s.StudentNumber.ToLower().Contains(query)) ||
                    (s.FirstName != null && s.FirstName.ToLower().Contains(query)) ||
                    (s.LastName != null && s.LastName.ToLower().Contains(query)) ||
                    (s.CourseName != null && s.CourseName.ToLower().Contains(query)) ||
                    (s.Status != null && s.Status.ToLower().Contains(query))).ToList();

            foreach (var s in filtered)
            {
                _dgvStudents.Rows.Add(
                    s.Id,
                    s.StudentNumber,
                    s.FullName,
                    s.CourseName ?? "N/A",
                    s.YearLevel ?? "N/A",
                    s.Gender ?? "N/A",
                    s.Status ?? "Active",
                    s.Email ?? ""
                );
            }
        }

        private void BtnAddStudent_Click(object sender, EventArgs e)
        {
            using (AddStudentForm form = new AddStudentForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void BtnEditStudent_Click(object sender, EventArgs e)
        {
            if (_dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = Convert.ToInt32(_dgvStudents.SelectedRows[0].Cells["ID"].Value);
            Student student = _allStudents.FirstOrDefault(s => s.Id == studentId);

            if (student != null)
            {
                using (AddStudentForm form = new AddStudentForm(student))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
        }

        private void BtnDeleteStudent_Click(object sender, EventArgs e)
        {
            if (_dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = Convert.ToInt32(_dgvStudents.SelectedRows[0].Cells["ID"].Value);
            string studentName = _dgvStudents.SelectedRows[0].Cells["FullName"].Value.ToString();

            if (MessageBox.Show(string.Format("Are you sure you want to delete {0}?", studentName), "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _studentRepo.Delete(studentId);
                    MessageBox.Show("Student deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error deleting student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
