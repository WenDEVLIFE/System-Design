using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace System_Design
{
    internal sealed class AddStudentForm : Form
    {
        private const int NoCourseId = -1;

        private readonly CourseRepository _courseRepo = new CourseRepository();
        private readonly StudentRepository _studentRepo = new StudentRepository();

        private readonly Student _existingStudent;
        private string _selectedPhotoPath;

        // UI Controls
        private Panel _pnlHeader;
        private Label _lblHeaderTitle;
        private PictureBox _pbPhoto;
        private Button _btnUploadPhoto;

        private TextBox _txtStudentNumber;
        private TextBox _txtFirstName;
        private TextBox _txtMiddleName;
        private TextBox _txtLastName;
        private ComboBox _cmbGender;
        private DateTimePicker _dtpDateOfBirth;

        private ComboBox _cmbCourse;
        private ComboBox _cmbYearLevel;
        private TextBox _txtAddress;
        private TextBox _txtEmail;
        private TextBox _txtPhone;
        private ComboBox _cmbStatus;

        private Button _btnSave;
        private Button _btnUpdate;
        private Button _btnCancel;

        public Student SavedStudent { get; private set; }

        public AddStudentForm(Student studentToEdit = null)
        {
            _existingStudent = studentToEdit;

            Text = _existingStudent == null ? "Student - Add New" : "Student - Edit";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(790, 450);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            InitializeComponents();
            LoadCourses();

            if (_existingStudent != null)
            {
                PopulateForm(_existingStudent);
            }
        }

        private void InitializeComponents()
        {
            // 1. Top Light Blue Header Bar
            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                BackColor = Color.FromArgb(220, 232, 250)
            };
            Controls.Add(_pnlHeader);

            _lblHeaderTitle = new Label
            {
                Text = _existingStudent == null ? "Student - Add New" : "Student - Edit",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 25, 45),
                Location = new Point(15, 10),
                AutoSize = true
            };
            _pnlHeader.Controls.Add(_lblHeaderTitle);

            // 2. Section Subheader "STUDENT INFORMATION"
            Label lblSubheader = new Label
            {
                Text = "STUDENT INFORMATION",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 240),
                Location = new Point(20, 56),
                AutoSize = true
            };
            Controls.Add(lblSubheader);

            // 3. Photo Box & Upload Button (Left Side)
            _pbPhoto = new PictureBox
            {
                Location = new Point(20, 92),
                Size = new Size(145, 145),
                BackColor = Color.FromArgb(225, 228, 232),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            _pbPhoto.Paint += PbPhoto_Paint;
            Controls.Add(_pbPhoto);

            _btnUploadPhoto = new Button
            {
                Text = "Upload Photo",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 232, 250),
                ForeColor = Color.FromArgb(15, 25, 45),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 245),
                Size = new Size(145, 30),
                Cursor = Cursors.Hand
            };
            _btnUploadPhoto.FlatAppearance.BorderSize = 0;
            _btnUploadPhoto.Click += BtnUploadPhoto_Click;
            Controls.Add(_btnUploadPhoto);

            // 4. Form Fields (Two Column Grid)
            int yStart = 95;
            int yGap = 42;

            // --- Column 1 Fields ---
            int col1LblX = 185;
            int col1FieldX = 280;
            int col1FieldW = 180;

            // Student ID
            Controls.Add(CreateLabel("Student ID", col1LblX, yStart));
            _txtStudentNumber = CreateTextBox(col1FieldX, yStart - 3, col1FieldW);
            Controls.Add(_txtStudentNumber);

            // First Name
            Controls.Add(CreateLabel("First Name", col1LblX, yStart + yGap));
            _txtFirstName = CreateTextBox(col1FieldX, yStart + yGap - 3, col1FieldW);
            Controls.Add(_txtFirstName);

            // Middle Name
            Controls.Add(CreateLabel("Middle Name", col1LblX, yStart + yGap * 2));
            _txtMiddleName = CreateTextBox(col1FieldX, yStart + yGap * 2 - 3, col1FieldW);
            Controls.Add(_txtMiddleName);

            // Last Name
            Controls.Add(CreateLabel("Last Name", col1LblX, yStart + yGap * 3));
            _txtLastName = CreateTextBox(col1FieldX, yStart + yGap * 3 - 3, col1FieldW);
            Controls.Add(_txtLastName);

            // Gender
            Controls.Add(CreateLabel("Gender", col1LblX, yStart + yGap * 4));
            _cmbGender = CreateComboBox(col1FieldX, yStart + yGap * 4 - 3, col1FieldW);
            _cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            _cmbGender.SelectedIndex = 0;
            Controls.Add(_cmbGender);

            // Date of Birth
            Controls.Add(CreateLabel("Date of Birth", col1LblX, yStart + yGap * 5));
            _dtpDateOfBirth = new DateTimePicker
            {
                Location = new Point(col1FieldX, yStart + yGap * 5 - 3),
                Width = col1FieldW,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9.5F)
            };
            Controls.Add(_dtpDateOfBirth);

            // --- Column 2 Fields ---
            int col2LblX = 480;
            int col2FieldX = 565;
            int col2FieldW = 200;

            // Course
            Controls.Add(CreateLabel("Course", col2LblX, yStart));
            _cmbCourse = CreateComboBox(col2FieldX, yStart - 3, col2FieldW);
            Controls.Add(_cmbCourse);

            // Year Level
            Controls.Add(CreateLabel("Year Level", col2LblX, yStart + yGap));
            _cmbYearLevel = CreateComboBox(col2FieldX, yStart + yGap - 3, col2FieldW);
            _cmbYearLevel.Items.AddRange(new object[] { "1st Year", "2nd Year", "3rd Year", "4th Year" });
            _cmbYearLevel.SelectedIndex = 0;
            Controls.Add(_cmbYearLevel);

            // Address
            Controls.Add(CreateLabel("Address", col2LblX, yStart + yGap * 2));
            _txtAddress = CreateTextBox(col2FieldX, yStart + yGap * 2 - 3, col2FieldW);
            Controls.Add(_txtAddress);

            // Email
            Controls.Add(CreateLabel("Email", col2LblX, yStart + yGap * 3));
            _txtEmail = CreateTextBox(col2FieldX, yStart + yGap * 3 - 3, col2FieldW);
            Controls.Add(_txtEmail);

            // Phone
            Controls.Add(CreateLabel("Phone", col2LblX, yStart + yGap * 4));
            _txtPhone = CreateTextBox(col2FieldX, yStart + yGap * 4 - 3, col2FieldW);
            Controls.Add(_txtPhone);

            // Status
            Controls.Add(CreateLabel("Status", col2LblX, yStart + yGap * 5));
            _cmbStatus = CreateComboBox(col2FieldX, yStart + yGap * 5 - 3, col2FieldW);
            _cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "Graduated", "Dropped" });
            _cmbStatus.SelectedIndex = 0;
            Controls.Add(_cmbStatus);

            // 5. Action Buttons (Bottom Right)
            int btnY = 385;

            _btnSave = new Button
            {
                Text = "Save",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(11, 94, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(460, btnY),
                Size = new Size(90, 36),
                Cursor = Cursors.Hand
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += BtnSave_Click;
            Controls.Add(_btnSave);

            _btnUpdate = new Button
            {
                Text = "Update",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 232, 250),
                ForeColor = Color.FromArgb(11, 94, 215),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(560, btnY),
                Size = new Size(90, 36),
                Cursor = Cursors.Hand
            };
            _btnUpdate.FlatAppearance.BorderSize = 0;
            _btnUpdate.Click += BtnUpdate_Click;
            Controls.Add(_btnUpdate);

            _btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(225, 228, 232),
                ForeColor = Color.FromArgb(40, 45, 55),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(660, btnY),
                Size = new Size(90, 36),
                Cursor = Cursors.Hand
            };
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Controls.Add(_btnCancel);
        }

        private static Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 35, 45),
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
                Font = new Font("Segoe UI", 9.5F)
            };
        }

        private static ComboBox CreateComboBox(int x, int y, int width)
        {
            return new ComboBox
            {
                Location = new Point(x, y),
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };
        }

        private void PbPhoto_Paint(object sender, PaintEventArgs e)
        {
            if (_pbPhoto.Image == null)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int cx = _pbPhoto.Width / 2;
                int cy = _pbPhoto.Height / 2 - 8;

                using (Pen pen = new Pen(Color.FromArgb(50, 50, 50), 3F))
                {
                    // Head circle
                    g.DrawEllipse(pen, cx - 25, cy - 35, 50, 50);
                    // Body arc
                    g.DrawArc(pen, cx - 45, cy + 20, 90, 70, 180, 180);
                }
            }
        }

        private void BtnUploadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Student Photo";
                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _selectedPhotoPath = dlg.FileName;
                        _pbPhoto.Image = Image.FromFile(_selectedPhotoPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to load image: " + ex.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadCourses()
        {
            try
            {
                List<Course> courses = _courseRepo.GetAll();
                List<CourseComboItem> items = new List<CourseComboItem>
                {
                    new CourseComboItem(NoCourseId, "-- Select Course --")
                };
                foreach (var c in courses)
                {
                    items.Add(new CourseComboItem(c.Id, string.Format("{0} - {1}", c.Code, c.Name)));
                }
                _cmbCourse.DataSource = items;
                _cmbCourse.DisplayMember = "Display";
                _cmbCourse.ValueMember = "Id";
            }
            catch
            {
                // Fallback
            }
        }

        private void PopulateForm(Student s)
        {
            _txtStudentNumber.Text = s.StudentNumber;
            _txtFirstName.Text = s.FirstName;
            _txtMiddleName.Text = s.MiddleName;
            _txtLastName.Text = s.LastName;
            if (!string.IsNullOrEmpty(s.Gender)) _cmbGender.SelectedItem = s.Gender;
            if (s.DateOfBirth.HasValue) _dtpDateOfBirth.Value = s.DateOfBirth.Value;

            if (s.CourseId.HasValue) _cmbCourse.SelectedValue = s.CourseId.Value;
            if (!string.IsNullOrEmpty(s.YearLevel)) _cmbYearLevel.SelectedItem = s.YearLevel;
            _txtAddress.Text = s.Address;
            _txtEmail.Text = s.Email;
            _txtPhone.Text = s.Phone;
            if (!string.IsNullOrEmpty(s.Status)) _cmbStatus.SelectedItem = s.Status;

            if (!string.IsNullOrEmpty(s.PhotoPath) && File.Exists(s.PhotoPath))
            {
                try
                {
                    _selectedPhotoPath = s.PhotoPath;
                    _pbPhoto.Image = Image.FromFile(_selectedPhotoPath);
                }
                catch { }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            Student student = new Student
            {
                StudentNumber = _txtStudentNumber.Text.Trim(),
                FirstName = _txtFirstName.Text.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(_txtMiddleName.Text) ? null : _txtMiddleName.Text.Trim(),
                LastName = _txtLastName.Text.Trim(),
                Gender = _cmbGender.SelectedItem != null ? _cmbGender.SelectedItem.ToString() : null,
                DateOfBirth = _dtpDateOfBirth.Value,
                YearLevel = _cmbYearLevel.SelectedItem != null ? _cmbYearLevel.SelectedItem.ToString() : null,
                Address = string.IsNullOrWhiteSpace(_txtAddress.Text) ? null : _txtAddress.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(_txtPhone.Text) ? null : _txtPhone.Text.Trim(),
                Status = _cmbStatus.SelectedItem != null ? _cmbStatus.SelectedItem.ToString() : "Active",
                PhotoPath = _selectedPhotoPath,
                CourseId = (int)_cmbCourse.SelectedValue == NoCourseId ? (int?)null : (int)_cmbCourse.SelectedValue
            };

            try
            {
                int newId = _studentRepo.Create(student);
                student.Id = newId;
                SavedStudent = student;
                MessageBox.Show("Student saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_existingStudent == null)
            {
                MessageBox.Show("No existing student selected to update.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            Student student = new Student
            {
                Id = _existingStudent.Id,
                StudentNumber = _txtStudentNumber.Text.Trim(),
                FirstName = _txtFirstName.Text.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(_txtMiddleName.Text) ? null : _txtMiddleName.Text.Trim(),
                LastName = _txtLastName.Text.Trim(),
                Gender = _cmbGender.SelectedItem != null ? _cmbGender.SelectedItem.ToString() : null,
                DateOfBirth = _dtpDateOfBirth.Value,
                YearLevel = _cmbYearLevel.SelectedItem != null ? _cmbYearLevel.SelectedItem.ToString() : null,
                Address = string.IsNullOrWhiteSpace(_txtAddress.Text) ? null : _txtAddress.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(_txtEmail.Text) ? null : _txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(_txtPhone.Text) ? null : _txtPhone.Text.Trim(),
                Status = _cmbStatus.SelectedItem != null ? _cmbStatus.SelectedItem.ToString() : "Active",
                PhotoPath = _selectedPhotoPath,
                CourseId = (int)_cmbCourse.SelectedValue == NoCourseId ? (int?)null : (int)_cmbCourse.SelectedValue
            };

            try
            {
                _studentRepo.Update(student);
                SavedStudent = student;
                MessageBox.Show("Student updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating student: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(_txtStudentNumber.Text) ||
                string.IsNullOrWhiteSpace(_txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(_txtLastName.Text))
            {
                MessageBox.Show("Student ID, First Name, and Last Name are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
