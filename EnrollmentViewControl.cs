using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class EnrollmentViewControl : UserControl
    {
        private readonly EnrollmentRepository _enrollmentRepo = new EnrollmentRepository();
        private readonly StudentRepository _studentRepo = new StudentRepository();
        private readonly CourseRepository _courseRepo = new CourseRepository();

        private DataGridView _dgvEnrollments;
        private ComboBox _cmbStudent;
        private ComboBox _cmbCourse;
        private TextBox _txtGrade;
        private Button _btnEnroll;
        private Button _btnUnenroll;
        private Button _btnSetGrade;

        private List<Enrollment> _allEnrollments = new List<Enrollment>();
        private bool _loading;

        public EnrollmentViewControl()
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
                Text = "Enrollment Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 30, 55),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            container.Controls.Add(lblTitle);

            // DataGridView (Left side)
            _dgvEnrollments = new DataGridView
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
            _dgvEnrollments.EnableHeadersVisualStyles = false;
            _dgvEnrollments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 232, 250);
            _dgvEnrollments.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 40, 80);
            _dgvEnrollments.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _dgvEnrollments.ColumnHeadersHeight = 35;
            _dgvEnrollments.RowTemplate.Height = 32;

            _dgvEnrollments.Columns.Add("StudentId", "StudentId");
            _dgvEnrollments.Columns["StudentId"].Visible = false;
            _dgvEnrollments.Columns.Add("CourseId", "CourseId");
            _dgvEnrollments.Columns["CourseId"].Visible = false;
            _dgvEnrollments.Columns.Add("StudentNumber", "Student No.");
            _dgvEnrollments.Columns.Add("StudentName", "Student Name");
            _dgvEnrollments.Columns.Add("Course", "Subject / Course");
            _dgvEnrollments.Columns.Add("Grade", "Grade");
            _dgvEnrollments.Columns.Add("Date", "Date Enrolled");

            _dgvEnrollments.SelectionChanged += DgvEnrollments_SelectionChanged;
            container.Controls.Add(_dgvEnrollments);

            // Action Card (Right side)
            GroupBox grpEnroll = new GroupBox
            {
                Text = "Enroll / Update Subject",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 40, 75),
                Location = new Point(625, 68),
                Width = 330,
                Height = 537,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int y = 40;

            // Student Combo
            grpEnroll.Controls.Add(CreateLabel("Select Student *", 20, y));
            _cmbStudent = new ComboBox
            {
                Location = new Point(20, y + 22),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            grpEnroll.Controls.Add(_cmbStudent);

            y += 75;

            // Course Combo
            grpEnroll.Controls.Add(CreateLabel("Select Subject / Course *", 20, y));
            _cmbCourse = new ComboBox
            {
                Location = new Point(20, y + 22),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            grpEnroll.Controls.Add(_cmbCourse);

            y += 75;

            // Grade Input
            grpEnroll.Controls.Add(CreateLabel("Grade (Optional)", 20, y));
            _txtGrade = new TextBox
            {
                Location = new Point(20, y + 22),
                Width = 280,
                Font = new Font("Segoe UI", 10F)
            };
            grpEnroll.Controls.Add(_txtGrade);

            // Action Buttons
            _btnEnroll = CreateButton("Enroll Student", Color.FromArgb(13, 110, 253), 20, 310, 280);
            _btnEnroll.Click += BtnEnroll_Click;

            _btnSetGrade = CreateButton("Save / Update Grade", Color.FromArgb(25, 135, 84), 20, 360, 280);
            _btnSetGrade.Click += BtnSetGrade_Click;

            _btnUnenroll = CreateButton("Unenroll Student", Color.FromArgb(220, 53, 69), 20, 410, 280);
            _btnUnenroll.Click += BtnUnenroll_Click;

            grpEnroll.Controls.Add(_btnEnroll);
            grpEnroll.Controls.Add(_btnSetGrade);
            grpEnroll.Controls.Add(_btnUnenroll);

            container.Controls.Add(grpEnroll);
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
            _loading = true;
            try
            {
                // Load Students Combo
                List<Student> students = _studentRepo.GetAll();
                List<ComboItem> studentItems = students.Select(s => new ComboItem(s.Id, string.Format("{0} - {1} {2}", s.StudentNumber, s.FirstName, s.LastName))).ToList();
                _cmbStudent.DataSource = studentItems;
                _cmbStudent.DisplayMember = "Display";
                _cmbStudent.ValueMember = "Id";

                // Load Courses Combo
                List<Course> courses = _courseRepo.GetAll();
                List<ComboItem> courseItems = courses.Select(c => new ComboItem(c.Id, string.Format("{0} - {1}", c.Code, c.Name))).ToList();
                _cmbCourse.DataSource = courseItems;
                _cmbCourse.DisplayMember = "Display";
                _cmbCourse.ValueMember = "Id";

                // Load Enrollments
                _allEnrollments = _enrollmentRepo.GetAll();
                _dgvEnrollments.Rows.Clear();

                foreach (var en in _allEnrollments)
                {
                    _dgvEnrollments.Rows.Add(
                        en.StudentId,
                        en.CourseId,
                        en.StudentNumber,
                        en.StudentName,
                        en.CourseCode + " - " + en.CourseName,
                        en.Grade.HasValue ? en.Grade.Value.ToString("0.00") : "N/A",
                        en.EnrolledAt.ToString("MM/dd/yyyy")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading enrollment data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loading = false;
            }
        }

        private void DgvEnrollments_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading || _dgvEnrollments.SelectedRows.Count == 0) return;

            DataGridViewRow row = _dgvEnrollments.SelectedRows[0];
            int studentId = Convert.ToInt32(row.Cells["StudentId"].Value);
            int courseId = Convert.ToInt32(row.Cells["CourseId"].Value);

            _cmbStudent.SelectedValue = studentId;
            _cmbCourse.SelectedValue = courseId;

            var enrollment = _allEnrollments.FirstOrDefault(en => en.StudentId == studentId && en.CourseId == courseId);
            if (enrollment != null)
            {
                _txtGrade.Text = enrollment.Grade.HasValue ? enrollment.Grade.Value.ToString("0.00") : "";
            }
        }

        private void BtnEnroll_Click(object sender, EventArgs e)
        {
            if (_cmbStudent.SelectedValue == null || _cmbCourse.SelectedValue == null)
            {
                MessageBox.Show("Please select both a student and a course.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = (int)_cmbStudent.SelectedValue;
            int courseId = (int)_cmbCourse.SelectedValue;

            try
            {
                bool success = _enrollmentRepo.Enroll(studentId, courseId);
                if (success)
                {
                    MessageBox.Show("Student enrolled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Student is already enrolled in this course.", "Duplicate Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error enrolling student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetGrade_Click(object sender, EventArgs e)
        {
            if (_cmbStudent.SelectedValue == null || _cmbCourse.SelectedValue == null)
            {
                MessageBox.Show("Please select a student and course.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = (int)_cmbStudent.SelectedValue;
            int courseId = (int)_cmbCourse.SelectedValue;

            decimal? gradeVal = null;
            if (!string.IsNullOrWhiteSpace(_txtGrade.Text))
            {
                if (decimal.TryParse(_txtGrade.Text.Trim(), out decimal parsed))
                {
                    gradeVal = parsed;
                }
                else
                {
                    MessageBox.Show("Please enter a valid numeric grade.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                bool success = _enrollmentRepo.SetGrade(studentId, courseId, gradeVal);
                if (success)
                {
                    MessageBox.Show("Grade updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Student is not enrolled in this course.", "Enrollment Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error setting grade: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUnenroll_Click(object sender, EventArgs e)
        {
            if (_cmbStudent.SelectedValue == null || _cmbCourse.SelectedValue == null)
            {
                MessageBox.Show("Please select a student and course to unenroll.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = (int)_cmbStudent.SelectedValue;
            int courseId = (int)_cmbCourse.SelectedValue;

            if (MessageBox.Show("Are you sure you want to unenroll this student from the course?", "Confirm Unenrollment", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    bool success = _enrollmentRepo.Unenroll(studentId, courseId);
                    if (success)
                    {
                        MessageBox.Show("Student unenrolled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error unenrolling student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private sealed class ComboItem
        {
            public int Id { get; }
            public string Display { get; }

            public ComboItem(int id, string display)
            {
                Id = id;
                Display = display;
            }
        }
    }
}
