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
        private const int NoCourseId = -1;

        private readonly StudentRepository _studentRepo = new StudentRepository();
        private readonly CourseRepository _courseRepo = new CourseRepository();

        private DataGridView _dgvStudents;
        private TextBox _txtSearch;
        private TextBox _txtStudentNumber;
        private TextBox _txtFirstName;
        private TextBox _txtLastName;
        private TextBox _txtEmail;
        private TextBox _txtPhone;
        private ComboBox _cmbCourse;

        private Button _btnAdd;
        private Button _btnUpdate;
        private Button _btnDelete;
        private Button _btnClear;

        private List<Student> _allStudents = new List<Student>();
        private bool _loading;

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

            // Search Bar & Refresh
            _txtSearch = new TextBox
            {
                Location = new Point(25, 68),
                Width = 280,
                Font = new Font("Segoe UI", 10F)
            };
            _txtSearch.TextChanged += (s, e) => FilterStudents();

            Label lblSearch = new Label
            {
                Text = "🔍 Search Student:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 70, 90),
                AutoSize = true,
                Location = new Point(25, 47)
            };
            container.Controls.Add(lblSearch);
            container.Controls.Add(_txtSearch);

            // DataGridView (Left side)
            _dgvStudents = new DataGridView
            {
                Location = new Point(25, 105),
                Width = 580,
                Height = 500,
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
            _dgvStudents.EnableHeadersVisualStyles = false;
            _dgvStudents.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 232, 250);
            _dgvStudents.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 40, 80);
            _dgvStudents.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _dgvStudents.ColumnHeadersHeight = 35;
            _dgvStudents.RowTemplate.Height = 32;

            _dgvStudents.Columns.Add("ID", "ID");
            _dgvStudents.Columns["ID"].Visible = false;
            _dgvStudents.Columns.Add("StudentNumber", "Student No.");
            _dgvStudents.Columns.Add("Name", "Student Name");
            _dgvStudents.Columns.Add("Course", "Course");
            _dgvStudents.Columns.Add("Email", "Email");

            _dgvStudents.SelectionChanged += DgvStudents_SelectionChanged;
            container.Controls.Add(_dgvStudents);

            // Details Form Card (Right side)
            GroupBox grpDetails = new GroupBox
            {
                Text = "Student Details",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 75),
                Location = new Point(625, 68),
                Width = 330,
                Height = 537,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int startY = 35;
            int gapY = 65;

            // Student Number
            grpDetails.Controls.Add(CreateLabel("Student Number *", 20, startY));
            _txtStudentNumber = CreateTextBox(20, startY + 22, 280);
            grpDetails.Controls.Add(_txtStudentNumber);

            // First Name
            grpDetails.Controls.Add(CreateLabel("First Name *", 20, startY + gapY));
            _txtFirstName = CreateTextBox(20, startY + gapY + 22, 280);
            grpDetails.Controls.Add(_txtFirstName);

            // Last Name
            grpDetails.Controls.Add(CreateLabel("Last Name *", 20, startY + gapY * 2));
            _txtLastName = CreateTextBox(20, startY + gapY * 2 + 22, 280);
            grpDetails.Controls.Add(_txtLastName);

            // Email
            grpDetails.Controls.Add(CreateLabel("Email", 20, startY + gapY * 3));
            _txtEmail = CreateTextBox(20, startY + gapY * 3 + 22, 280);
            grpDetails.Controls.Add(_txtEmail);

            // Phone
            grpDetails.Controls.Add(CreateLabel("Phone", 20, startY + gapY * 4));
            _txtPhone = CreateTextBox(20, startY + gapY * 4 + 22, 280);
            grpDetails.Controls.Add(_txtPhone);

            // Course Combo
            grpDetails.Controls.Add(CreateLabel("Course", 20, startY + gapY * 5));
            _cmbCourse = new ComboBox
            {
                Location = new Point(20, startY + gapY * 5 + 22),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            grpDetails.Controls.Add(_cmbCourse);

            // Action Buttons
            _btnAdd = CreateButton("Add", Color.FromArgb(13, 110, 253), 20, 425, 135);
            _btnAdd.Click += BtnAdd_Click;

            _btnUpdate = CreateButton("Update", Color.FromArgb(25, 135, 84), 165, 425, 135);
            _btnUpdate.Click += BtnUpdate_Click;

            _btnDelete = CreateButton("Delete", Color.FromArgb(220, 53, 69), 20, 470, 135);
            _btnDelete.Click += BtnDelete_Click;

            _btnClear = CreateButton("Clear", Color.FromArgb(108, 117, 125), 165, 470, 135);
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
                Size = new Size(width, 34),
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
                // Reload courses dropdown
                List<Course> courses = _courseRepo.GetAll();
                List<CourseComboItem> comboItems = new List<CourseComboItem>
                {
                    new CourseComboItem(NoCourseId, "-- None --")
                };
                foreach (var c in courses)
                {
                    comboItems.Add(new CourseComboItem(c.Id, string.Format("{0} - {1}", c.Code, c.Name)));
                }
                _cmbCourse.DataSource = comboItems;
                _cmbCourse.DisplayMember = "Display";
                _cmbCourse.ValueMember = "Id";

                // Reload students
                _allStudents = _studentRepo.GetAll();
                FilterStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loading = false;
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
                    (s.CourseName != null && s.CourseName.ToLower().Contains(query))).ToList();

            foreach (var s in filtered)
            {
                _dgvStudents.Rows.Add(s.Id, s.StudentNumber, s.FirstName + " " + s.LastName, s.CourseName ?? "N/A", s.Email ?? "");
            }

            ClearForm();
        }

        private void DgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading || _dgvStudents.SelectedRows.Count == 0) return;

            DataGridViewRow row = _dgvStudents.SelectedRows[0];
            int studentId = Convert.ToInt32(row.Cells["ID"].Value);
            Student student = _allStudents.FirstOrDefault(s => s.Id == studentId);

            if (student != null)
            {
                _txtStudentNumber.Text = student.StudentNumber;
                _txtFirstName.Text = student.FirstName;
                _txtLastName.Text = student.LastName;
                _txtEmail.Text = student.Email;
                _txtPhone.Text = student.Phone;
                _cmbCourse.SelectedValue = student.CourseId ?? NoCourseId;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            Student student = new Student
            {
                StudentNumber = _txtStudentNumber.Text.Trim(),
                FirstName = _txtFirstName.Text.Trim(),
                LastName = _txtLastName.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(_txtPhone.Text) ? null : _txtPhone.Text.Trim(),
                CourseId = (int)_cmbCourse.SelectedValue == NoCourseId ? (int?)null : (int)_cmbCourse.SelectedValue
            };

            try
            {
                _studentRepo.Create(student);
                MessageBox.Show("Student added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error adding student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            int studentId = Convert.ToInt32(_dgvStudents.SelectedRows[0].Cells["ID"].Value);
            Student student = new Student
            {
                Id = studentId,
                StudentNumber = _txtStudentNumber.Text.Trim(),
                FirstName = _txtFirstName.Text.Trim(),
                LastName = _txtLastName.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(_txtPhone.Text) ? null : _txtPhone.Text.Trim(),
                CourseId = (int)_cmbCourse.SelectedValue == NoCourseId ? (int?)null : (int)_cmbCourse.SelectedValue
            };

            try
            {
                _studentRepo.Update(student);
                MessageBox.Show("Student updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error updating student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = Convert.ToInt32(_dgvStudents.SelectedRows[0].Cells["ID"].Value);
            string studentName = _dgvStudents.SelectedRows[0].Cells["Name"].Value.ToString();

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

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            _dgvStudents.ClearSelection();
        }

        private void ClearForm()
        {
            _txtStudentNumber.Clear();
            _txtFirstName.Clear();
            _txtLastName.Clear();
            _txtEmail.Clear();
            _txtPhone.Clear();
            if (_cmbCourse.Items.Count > 0) _cmbCourse.SelectedIndex = 0;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(_txtStudentNumber.Text) ||
                string.IsNullOrWhiteSpace(_txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(_txtLastName.Text))
            {
                MessageBox.Show("Student Number, First Name, and Last Name are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private sealed class CourseComboItem
        {
            public int Id { get; }
            public string Display { get; }

            public CourseComboItem(int id, string display)
            {
                Id = id;
                Display = display;
            }
        }
    }
}
