using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace System_Design
{
    internal sealed class DashboardForm : Form
    {
        private const int NoCourseId = -1;

        private readonly CourseRepository _courseRepository = new CourseRepository();
        private readonly StudentRepository _studentRepository = new StudentRepository();
        private readonly EnrollmentRepository _enrollmentRepository = new EnrollmentRepository();

        private bool _loading;

        private TabControl _tabs;
        private DataGridView _dgvStudents;
        private DataGridView _dgvCourses;
        private DataGridView _dgvEnrollments;
        private TextBox _txtStudentNumber;
        private TextBox _txtFirstName;
        private TextBox _txtLastName;
        private TextBox _txtEmail;
        private TextBox _txtPhone;
        private ComboBox _cmbStudentCourse;
        private TextBox _txtCourseCode;
        private TextBox _txtCourseName;
        private TextBox _txtUnits;
        private ComboBox _cmbEnrollStudent;
        private ComboBox _cmbEnrollCourse;
        private TextBox _txtGrade;

        public DashboardForm()
        {
            Text = "Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1020, 690);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Font = new Font("Microsoft YaHei UI", 9F);

            BuildHeader();
            BuildTabs();
            RefreshStudentsTab();
        }

        // --------------------------------------------------------------------
        // Layout
        // --------------------------------------------------------------------

        private void BuildHeader()
        {
            User user = Session.CurrentUser;
            string username = user != null ? user.Username : "User";
            string role = user != null ? user.Role : "user";

            Label welcome = new Label
            {
                Text = string.Format("Welcome, {0} (role: {1})", username, role),
                Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 18)
            };

            Button logout = new Button
            {
                Text = "Logout",
                Location = new Point(918, 14),
                Size = new Size(84, 32)
            };
            logout.Click += (sender, args) =>
            {
                Session.Logout();
                Close();
            };

            Controls.Add(welcome);
            Controls.Add(logout);
        }

        private void BuildTabs()
        {
            _tabs = new TabControl
            {
                Location = new Point(12, 56),
                Size = new Size(996, 620),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            _tabs.TabPages.Add(BuildStudentsTab());
            _tabs.TabPages.Add(BuildCoursesTab());
            _tabs.TabPages.Add(BuildEnrollmentsTab());
            _tabs.SelectedIndexChanged += tabs_SelectedIndexChanged;

            Controls.Add(_tabs);
        }

        private TabPage BuildStudentsTab()
        {
            TabPage page = new TabPage("Students");

            _dgvStudents = new DataGridView
            {
                Location = new Point(12, 12),
                Size = new Size(968, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            _dgvStudents.SelectionChanged += dgvStudents_SelectionChanged;

            Label lblStudentNumber = new Label { Text = "Student No", Location = new Point(12, 293), AutoSize = true };
            _txtStudentNumber = new TextBox { Location = new Point(100, 289), Size = new Size(130, 24) };

            Label lblFirstName = new Label { Text = "First Name", Location = new Point(250, 293), AutoSize = true };
            _txtFirstName = new TextBox { Location = new Point(340, 289), Size = new Size(130, 24) };

            Label lblLastName = new Label { Text = "Last Name", Location = new Point(490, 293), AutoSize = true };
            _txtLastName = new TextBox { Location = new Point(580, 289), Size = new Size(130, 24) };

            Label lblEmail = new Label { Text = "Email", Location = new Point(730, 293), AutoSize = true };
            _txtEmail = new TextBox { Location = new Point(790, 289), Size = new Size(190, 24) };

            Label lblPhone = new Label { Text = "Phone", Location = new Point(12, 343), AutoSize = true };
            _txtPhone = new TextBox { Location = new Point(70, 339), Size = new Size(150, 24) };

            Label lblCourse = new Label { Text = "Course", Location = new Point(240, 343), AutoSize = true };
            _cmbStudentCourse = new ComboBox
            {
                Location = new Point(300, 339),
                Size = new Size(320, 24),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Button btnAdd = MakeButton("Add", 12, 393, 90);
            btnAdd.Click += btnAddStudent_Click;

            Button btnUpdate = MakeButton("Update", 112, 393, 90);
            btnUpdate.Click += btnUpdateStudent_Click;

            Button btnDelete = MakeButton("Delete", 212, 393, 90);
            btnDelete.Click += btnDeleteStudent_Click;

            Button btnClear = MakeButton("Clear", 312, 393, 90);
            btnClear.Click += (sender, args) => ClearStudentFields();

            page.Controls.Add(_dgvStudents);
            page.Controls.Add(lblStudentNumber);
            page.Controls.Add(_txtStudentNumber);
            page.Controls.Add(lblFirstName);
            page.Controls.Add(_txtFirstName);
            page.Controls.Add(lblLastName);
            page.Controls.Add(_txtLastName);
            page.Controls.Add(lblEmail);
            page.Controls.Add(_txtEmail);
            page.Controls.Add(lblPhone);
            page.Controls.Add(_txtPhone);
            page.Controls.Add(lblCourse);
            page.Controls.Add(_cmbStudentCourse);
            page.Controls.Add(btnAdd);
            page.Controls.Add(btnUpdate);
            page.Controls.Add(btnDelete);
            page.Controls.Add(btnClear);

            return page;
        }

        private TabPage BuildCoursesTab()
        {
            TabPage page = new TabPage("Courses");

            _dgvCourses = new DataGridView
            {
                Location = new Point(12, 12),
                Size = new Size(968, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            _dgvCourses.SelectionChanged += dgvCourses_SelectionChanged;

            Label lblCode = new Label { Text = "Code", Location = new Point(12, 343), AutoSize = true };
            _txtCourseCode = new TextBox { Location = new Point(70, 339), Size = new Size(150, 24) };

            Label lblName = new Label { Text = "Name", Location = new Point(240, 343), AutoSize = true };
            _txtCourseName = new TextBox { Location = new Point(300, 339), Size = new Size(300, 24) };

            Label lblUnits = new Label { Text = "Units", Location = new Point(620, 343), AutoSize = true };
            _txtUnits = new TextBox { Location = new Point(670, 339), Size = new Size(70, 24), Text = "3" };

            Button btnAdd = MakeButton("Add", 12, 393, 90);
            btnAdd.Click += btnAddCourse_Click;

            Button btnUpdate = MakeButton("Update", 112, 393, 90);
            btnUpdate.Click += btnUpdateCourse_Click;

            Button btnDelete = MakeButton("Delete", 212, 393, 90);
            btnDelete.Click += btnDeleteCourse_Click;

            Button btnClear = MakeButton("Clear", 312, 393, 90);
            btnClear.Click += (sender, args) => ClearCourseFields();

            page.Controls.Add(_dgvCourses);
            page.Controls.Add(lblCode);
            page.Controls.Add(_txtCourseCode);
            page.Controls.Add(lblName);
            page.Controls.Add(_txtCourseName);
            page.Controls.Add(lblUnits);
            page.Controls.Add(_txtUnits);
            page.Controls.Add(btnAdd);
            page.Controls.Add(btnUpdate);
            page.Controls.Add(btnDelete);
            page.Controls.Add(btnClear);

            return page;
        }

        private TabPage BuildEnrollmentsTab()
        {
            TabPage page = new TabPage("Enrollments");

            Label lblStudent = new Label { Text = "Student", Location = new Point(12, 22), AutoSize = true };
            _cmbEnrollStudent = new ComboBox
            {
                Location = new Point(90, 18),
                Size = new Size(360, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "DisplayName"
            };
            _cmbEnrollStudent.SelectedIndexChanged += cmbEnrollStudent_SelectedIndexChanged;

            Label lblCourse = new Label { Text = "Course", Location = new Point(470, 22), AutoSize = true };
            _cmbEnrollCourse = new ComboBox
            {
                Location = new Point(540, 18),
                Size = new Size(300, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "DisplayName"
            };

            Label lblGrade = new Label { Text = "Grade", Location = new Point(860, 22), AutoSize = true };
            _txtGrade = new TextBox { Location = new Point(910, 18), Size = new Size(70, 24) };

            Button btnEnroll = MakeButton("Enroll", 12, 66, 90);
            btnEnroll.Click += btnEnroll_Click;

            Button btnUnenroll = MakeButton("Unenroll", 112, 66, 90);
            btnUnenroll.Click += btnUnenroll_Click;

            Button btnSetGrade = MakeButton("Set Grade", 212, 66, 100);
            btnSetGrade.Click += btnSetGrade_Click;

            Button btnRefresh = MakeButton("Refresh", 322, 66, 90);
            btnRefresh.Click += (sender, args) => RefreshEnrollmentsTab();

            _dgvEnrollments = new DataGridView
            {
                Location = new Point(12, 112),
                Size = new Size(968, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            page.Controls.Add(lblStudent);
            page.Controls.Add(_cmbEnrollStudent);
            page.Controls.Add(lblCourse);
            page.Controls.Add(_cmbEnrollCourse);
            page.Controls.Add(lblGrade);
            page.Controls.Add(_txtGrade);
            page.Controls.Add(btnEnroll);
            page.Controls.Add(btnUnenroll);
            page.Controls.Add(btnSetGrade);
            page.Controls.Add(btnRefresh);
            page.Controls.Add(_dgvEnrollments);

            return page;
        }

        private static Button MakeButton(string text, int x, int y, int width)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 36)
            };
        }

        // --------------------------------------------------------------------
        // Tab switching
        // --------------------------------------------------------------------

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (_tabs.SelectedIndex)
            {
                case 0:
                    RefreshStudentsTab();
                    break;
                case 1:
                    RefreshCoursesTab();
                    break;
                case 2:
                    RefreshEnrollmentsTab();
                    break;
            }
        }

        // --------------------------------------------------------------------
        // Students
        // --------------------------------------------------------------------

        private void RefreshStudentsTab()
        {
            _loading = true;
            try
            {
                ReloadStudentCourseCombo();
                LoadStudents();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
            finally
            {
                _loading = false;
            }
        }

        private void ReloadStudentCourseCombo()
        {
            int? previous = SelectedCourseId(_cmbStudentCourse);
            _cmbStudentCourse.BeginUpdate();
            _cmbStudentCourse.Items.Clear();
            _cmbStudentCourse.Items.Add(new Course { Id = NoCourseId, Name = "(No course)" });

            List<Course> courses = _courseRepository.GetAll();
            foreach (Course course in courses)
            {
                _cmbStudentCourse.Items.Add(course);
            }

            SelectCourseById(_cmbStudentCourse, previous.HasValue ? previous.Value : NoCourseId);
            _cmbStudentCourse.EndUpdate();
        }

        private void LoadStudents()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Student Number", typeof(string));
            table.Columns.Add("First Name", typeof(string));
            table.Columns.Add("Last Name", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Phone", typeof(string));
            table.Columns.Add("CourseId", typeof(int));
            table.Columns.Add("Course", typeof(string));

            foreach (Student student in _studentRepository.GetAll())
            {
                table.Rows.Add(
                    student.Id,
                    student.StudentNumber,
                    student.FirstName,
                    student.LastName,
                    student.Email,
                    student.Phone,
                    (object)student.CourseId ?? DBNull.Value,
                    (object)student.CourseName ?? string.Empty);
            }

            _dgvStudents.DataSource = table;
            _dgvStudents.Columns["Id"].Visible = false;
            _dgvStudents.Columns["CourseId"].Visible = false;
            _dgvStudents.Columns["Student Number"].Width = 110;
            _dgvStudents.Columns["First Name"].Width = 120;
            _dgvStudents.Columns["Last Name"].Width = 120;
            _dgvStudents.Columns["Email"].Width = 200;
            _dgvStudents.Columns["Phone"].Width = 110;
            _dgvStudents.Columns["Course"].Width = 200;
        }

        private void dgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading || _dgvStudents.CurrentRow == null)
            {
                return;
            }

            DataGridViewRow row = _dgvStudents.CurrentRow;
            _txtStudentNumber.Text = Convert.ToString(row.Cells["Student Number"].Value);
            _txtFirstName.Text = Convert.ToString(row.Cells["First Name"].Value);
            _txtLastName.Text = Convert.ToString(row.Cells["Last Name"].Value);
            _txtEmail.Text = Convert.ToString(row.Cells["Email"].Value);
            _txtPhone.Text = Convert.ToString(row.Cells["Phone"].Value);

            object courseIdValue = row.Cells["CourseId"].Value;
            int? courseId = courseIdValue == null || courseIdValue == DBNull.Value
                ? (int?)null
                : Convert.ToInt32(courseIdValue);

            SelectCourseById(_cmbStudentCourse, courseId.HasValue ? courseId.Value : NoCourseId);
        }

        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            string number = _txtStudentNumber.Text.Trim();
            string first = _txtFirstName.Text.Trim();
            string last = _txtLastName.Text.Trim();

            if (number.Length == 0 || first.Length == 0 || last.Length == 0)
            {
                MessageBox.Show(this, "Student number, first name and last name are required.",
                    "Add Student", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _studentRepository.Create(BuildStudentFromFields(-1));
                MessageBox.Show(this, "Student added successfully.", "Add Student",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshStudentsTab();
            }
            catch (MySqlException exception)
            {
                HandleUniqueOrDbError(exception, "student number");
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void btnUpdateStudent_Click(object sender, EventArgs e)
        {
            int id = SelectedGridId(_dgvStudents);
            if (id <= 0)
            {
                MessageBox.Show(this, "Please select a student from the list first.",
                    "Update Student", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool updated = _studentRepository.Update(BuildStudentFromFields(id));
                MessageBox.Show(this, updated
                        ? "Student updated successfully."
                        : "Unable to update the student.",
                    "Update Student", MessageBoxButtons.OK,
                    updated ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                RefreshStudentsTab();
            }
            catch (MySqlException exception)
            {
                HandleUniqueOrDbError(exception, "student number");
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void btnDeleteStudent_Click(object sender, EventArgs e)
        {
            int id = SelectedGridId(_dgvStudents);
            if (id <= 0)
            {
                MessageBox.Show(this, "Please select a student from the list first.",
                    "Delete Student", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(this, "Delete this student? Their enrollments will also be removed.",
                "Delete Student", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted = _studentRepository.Delete(id);
                MessageBox.Show(this, deleted ? "Student deleted." : "Unable to delete the student.",
                    "Delete Student", MessageBoxButtons.OK,
                    deleted ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                RefreshStudentsTab();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private Student BuildStudentFromFields(int id)
        {
            Course selected = _cmbStudentCourse.SelectedItem as Course;
            int? courseId = selected != null && selected.Id != NoCourseId ? (int?)selected.Id : null;

            return new Student
            {
                Id = id,
                StudentNumber = _txtStudentNumber.Text.Trim(),
                FirstName = _txtFirstName.Text.Trim(),
                LastName = _txtLastName.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Phone = _txtPhone.Text.Trim(),
                CourseId = courseId
            };
        }

        private void ClearStudentFields()
        {
            _txtStudentNumber.Text = string.Empty;
            _txtFirstName.Text = string.Empty;
            _txtLastName.Text = string.Empty;
            _txtEmail.Text = string.Empty;
            _txtPhone.Text = string.Empty;
            SelectCourseById(_cmbStudentCourse, NoCourseId);
            _dgvStudents.ClearSelection();
        }

        // --------------------------------------------------------------------
        // Courses
        // --------------------------------------------------------------------

        private void RefreshCoursesTab()
        {
            _loading = true;
            try
            {
                LoadCourses();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
            finally
            {
                _loading = false;
            }
        }

        private void LoadCourses()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Units", typeof(int));

            foreach (Course course in _courseRepository.GetAll())
            {
                table.Rows.Add(course.Id, course.Code, course.Name, course.Units);
            }

            _dgvCourses.DataSource = table;
            _dgvCourses.Columns["Id"].Visible = false;
            _dgvCourses.Columns["Code"].Width = 120;
            _dgvCourses.Columns["Name"].Width = 400;
            _dgvCourses.Columns["Units"].Width = 80;
        }

        private void dgvCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading || _dgvCourses.CurrentRow == null)
            {
                return;
            }

            DataGridViewRow row = _dgvCourses.CurrentRow;
            _txtCourseCode.Text = Convert.ToString(row.Cells["Code"].Value);
            _txtCourseName.Text = Convert.ToString(row.Cells["Name"].Value);
            _txtUnits.Text = Convert.ToString(row.Cells["Units"].Value);
        }

        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            string code = _txtCourseCode.Text.Trim();
            string name = _txtCourseName.Text.Trim();

            if (code.Length == 0 || name.Length == 0)
            {
                MessageBox.Show(this, "Course code and name are required.", "Add Course",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int units;
            if (!int.TryParse(_txtUnits.Text.Trim(), out units) || units <= 0 || units > 255)
            {
                MessageBox.Show(this, "Units must be a number between 1 and 255.", "Add Course",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _courseRepository.Create(new Course { Code = code, Name = name, Units = units });
                MessageBox.Show(this, "Course added successfully.", "Add Course",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshCoursesTab();
            }
            catch (MySqlException exception)
            {
                HandleUniqueOrDbError(exception, "course code");
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void btnUpdateCourse_Click(object sender, EventArgs e)
        {
            int id = SelectedGridId(_dgvCourses);
            if (id <= 0)
            {
                MessageBox.Show(this, "Please select a course from the list first.",
                    "Update Course", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string code = _txtCourseCode.Text.Trim();
            string name = _txtCourseName.Text.Trim();

            if (code.Length == 0 || name.Length == 0)
            {
                MessageBox.Show(this, "Course code and name are required.", "Update Course",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int units;
            if (!int.TryParse(_txtUnits.Text.Trim(), out units) || units <= 0 || units > 255)
            {
                MessageBox.Show(this, "Units must be a number between 1 and 255.", "Update Course",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool updated = _courseRepository.Update(new Course { Id = id, Code = code, Name = name, Units = units });
                MessageBox.Show(this, updated ? "Course updated successfully." : "Unable to update the course.",
                    "Update Course", MessageBoxButtons.OK,
                    updated ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                RefreshCoursesTab();
            }
            catch (MySqlException exception)
            {
                HandleUniqueOrDbError(exception, "course code");
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void btnDeleteCourse_Click(object sender, EventArgs e)
        {
            int id = SelectedGridId(_dgvCourses);
            if (id <= 0)
            {
                MessageBox.Show(this, "Please select a course from the list first.",
                    "Delete Course", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(this, "Delete this course?", "Delete Course",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted = _courseRepository.Delete(id);
                if (deleted)
                {
                    MessageBox.Show(this, "Course deleted.", "Delete Course",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this,
                        "Cannot delete this course because students are enrolled in it. " +
                        "Unenroll the students first.",
                        "Delete Course", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                RefreshCoursesTab();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void ClearCourseFields()
        {
            _txtCourseCode.Text = string.Empty;
            _txtCourseName.Text = string.Empty;
            _txtUnits.Text = "3";
            _dgvCourses.ClearSelection();
        }

        // --------------------------------------------------------------------
        // Enrollments
        // --------------------------------------------------------------------

        private void RefreshEnrollmentsTab()
        {
            _loading = true;
            try
            {
                int? previousStudentId = SelectedStudentId();
                int? previousCourseId = SelectedCourseId(_cmbEnrollCourse);

                _cmbEnrollStudent.BeginUpdate();
                _cmbEnrollStudent.Items.Clear();
                foreach (Student student in _studentRepository.GetAll())
                {
                    _cmbEnrollStudent.Items.Add(student);
                }
                SelectStudentById(previousStudentId);
                _cmbEnrollStudent.EndUpdate();

                _cmbEnrollCourse.BeginUpdate();
                _cmbEnrollCourse.Items.Clear();
                foreach (Course course in _courseRepository.GetAll())
                {
                    _cmbEnrollCourse.Items.Add(course);
                }
                SelectCourseById(_cmbEnrollCourse, previousCourseId.HasValue ? previousCourseId.Value : NoCourseId);
                _cmbEnrollCourse.EndUpdate();

                LoadEnrollments();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
            finally
            {
                _loading = false;
            }
        }

        private void cmbEnrollStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            try
            {
                LoadEnrollments();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void LoadEnrollments()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Course Code", typeof(string));
            table.Columns.Add("Course Name", typeof(string));
            table.Columns.Add("Grade", typeof(decimal));
            table.Columns.Add("Enrolled At", typeof(DateTime));

            Student selected = _cmbEnrollStudent.SelectedItem as Student;
            if (selected != null)
            {
                foreach (Enrollment enrollment in _enrollmentRepository.GetByStudent(selected.Id))
                {
                    table.Rows.Add(
                        enrollment.Id,
                        enrollment.CourseCode,
                        enrollment.CourseName,
                        (object)enrollment.Grade ?? DBNull.Value,
                        enrollment.EnrolledAt);
                }
            }

            _dgvEnrollments.DataSource = table;
            _dgvEnrollments.Columns["Id"].Visible = false;
            _dgvEnrollments.Columns["Course Code"].Width = 110;
            _dgvEnrollments.Columns["Course Name"].Width = 300;
            _dgvEnrollments.Columns["Grade"].Width = 80;
            _dgvEnrollments.Columns["Enrolled At"].Width = 160;
        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            Student student = _cmbEnrollStudent.SelectedItem as Student;
            Course course = _cmbEnrollCourse.SelectedItem as Course;

            if (student == null || course == null)
            {
                MessageBox.Show(this, "Please select both a student and a course.", "Enroll",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool enrolled = _enrollmentRepository.Enroll(student.Id, course.Id);
                MessageBox.Show(this, enrolled
                        ? "Student enrolled successfully."
                        : "That student is already enrolled in this course.",
                    "Enroll", MessageBoxButtons.OK,
                    enrolled ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                LoadEnrollments();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void btnUnenroll_Click(object sender, EventArgs e)
        {
            Student student = _cmbEnrollStudent.SelectedItem as Student;
            Course course = _cmbEnrollCourse.SelectedItem as Course;

            if (student == null || course == null)
            {
                MessageBox.Show(this, "Please select both a student and a course.", "Unenroll",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool removed = _enrollmentRepository.Unenroll(student.Id, course.Id);
                MessageBox.Show(this, removed
                        ? "Enrollment removed."
                        : "That student is not enrolled in the selected course.",
                    "Unenroll", MessageBoxButtons.OK,
                    removed ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                LoadEnrollments();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        private void btnSetGrade_Click(object sender, EventArgs e)
        {
            Student student = _cmbEnrollStudent.SelectedItem as Student;
            Course course = _cmbEnrollCourse.SelectedItem as Course;

            if (student == null || course == null)
            {
                MessageBox.Show(this, "Please select both a student and a course.", "Set Grade",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal? grade = null;
            string gradeText = _txtGrade.Text.Trim();
            if (gradeText.Length > 0)
            {
                decimal parsed;
                if (!decimal.TryParse(gradeText, out parsed) || parsed < 0 || parsed > 100)
                {
                    MessageBox.Show(this, "Grade must be a number between 0 and 100 (leave empty to clear).",
                        "Set Grade", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                grade = parsed;
            }

            try
            {
                bool updated = _enrollmentRepository.SetGrade(student.Id, course.Id, grade);
                MessageBox.Show(this, updated
                        ? "Grade saved."
                        : "That student is not enrolled in the selected course.",
                    "Set Grade", MessageBoxButtons.OK,
                    updated ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                LoadEnrollments();
            }
            catch (MySqlException)
            {
                ShowDbError();
            }
            catch (Exception)
            {
                ShowUnexpectedError();
            }
        }

        // --------------------------------------------------------------------
        // Shared helpers
        // --------------------------------------------------------------------

        private int SelectedGridId(DataGridView grid)
        {
            if (grid.CurrentRow == null)
            {
                return 0;
            }

            object value = grid.CurrentRow.Cells["Id"].Value;
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private int? SelectedStudentId()
        {
            Student selected = _cmbEnrollStudent.SelectedItem as Student;
            return selected != null ? (int?)selected.Id : null;
        }

        private static int? SelectedCourseId(ComboBox combo)
        {
            Course selected = combo.SelectedItem as Course;
            return selected != null ? (int?)selected.Id : null;
        }

        private void SelectStudentById(int? id)
        {
            if (!id.HasValue)
            {
                if (_cmbEnrollStudent.Items.Count > 0)
                {
                    _cmbEnrollStudent.SelectedIndex = 0;
                }
                return;
            }

            for (int i = 0; i < _cmbEnrollStudent.Items.Count; i++)
            {
                Student student = _cmbEnrollStudent.Items[i] as Student;
                if (student != null && student.Id == id.Value)
                {
                    _cmbEnrollStudent.SelectedIndex = i;
                    return;
                }
            }

            if (_cmbEnrollStudent.Items.Count > 0)
            {
                _cmbEnrollStudent.SelectedIndex = 0;
            }
        }

        private static void SelectCourseById(ComboBox combo, int id)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                Course course = combo.Items[i] as Course;
                if (course != null && course.Id == id)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        private void HandleUniqueOrDbError(MySqlException exception, string uniqueFieldLabel)
        {
            if (exception.Number == 1062)
            {
                MessageBox.Show(this, string.Format("A record with that {0} already exists.", uniqueFieldLabel),
                    "Duplicate Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                ShowDbError();
            }
        }

        private void ShowDbError()
        {
            MessageBox.Show(this,
                "Unable to reach the database. Please make sure MySQL is running and try again.",
                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowUnexpectedError()
        {
            MessageBox.Show(this, "An unexpected error occurred. Please try again.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}