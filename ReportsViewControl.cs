using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace System_Design
{
    internal sealed class ReportsViewControl : UserControl
    {
        private readonly StudentRepository _studentRepo = new StudentRepository();
        private readonly CourseRepository _courseRepo = new CourseRepository();
        private readonly EnrollmentRepository _enrollmentRepo = new EnrollmentRepository();

        private DataGridView _dgvReport;
        private Label _lblTotalStudents;
        private Label _lblTotalSubjects;
        private Label _lblTotalEnrollments;

        private ComboBox _cmbReportStudent;
        private Button _btnPreviewReportCard;

        public ReportsViewControl()
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
                Text = "Reports & System Analytics",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 30, 55),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            container.Controls.Add(lblTitle);

            // Report Card Generator Bar (Top Right)
            Label lblSelectStudent = new Label
            {
                Text = "Report Card for:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 50, 70),
                Location = new Point(480, 26),
                AutoSize = true
            };
            container.Controls.Add(lblSelectStudent);

            _cmbReportStudent = new ComboBox
            {
                Location = new Point(590, 23),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };
            container.Controls.Add(_cmbReportStudent);

            _btnPreviewReportCard = new Button
            {
                Text = "📄 Preview Report Card",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(11, 94, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(800, 21),
                Size = new Size(155, 30),
                Cursor = Cursors.Hand
            };
            _btnPreviewReportCard.FlatAppearance.BorderSize = 0;
            _btnPreviewReportCard.Click += BtnPreviewReportCard_Click;
            container.Controls.Add(_btnPreviewReportCard);

            // Summary Header Cards
            TableLayoutPanel statsRow = new TableLayoutPanel
            {
                Location = new Point(25, 65),
                Width = 930,
                Height = 85,
                ColumnCount = 3,
                RowCount = 1
            };
            statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
            statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));
            statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3F));

            statsRow.Controls.Add(CreateStatBox("TOTAL REGISTERED STUDENTS", "0", Color.FromArgb(13, 110, 253), out _lblTotalStudents), 0, 0);
            statsRow.Controls.Add(CreateStatBox("ACTIVE SUBJECTS", "0", Color.FromArgb(0, 168, 150), out _lblTotalSubjects), 1, 0);
            statsRow.Controls.Add(CreateStatBox("SYSTEM ENROLLMENTS", "0", Color.FromArgb(138, 43, 226), out _lblTotalEnrollments), 2, 0);

            container.Controls.Add(statsRow);

            // Data Table Section
            GroupBox grpReport = new GroupBox
            {
                Text = "SUBJECT ENROLLMENT BREAKDOWN",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 40, 70),
                Location = new Point(25, 165),
                Width = 930,
                Height = 440,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            _dgvReport = new DataGridView
            {
                Dock = DockStyle.Fill,
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvReport.EnableHeadersVisualStyles = false;
            _dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 232, 250);
            _dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 40, 80);
            _dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _dgvReport.ColumnHeadersHeight = 35;
            _dgvReport.RowTemplate.Height = 32;

            _dgvReport.Columns.Add("CourseCode", "Subject Code");
            _dgvReport.Columns.Add("CourseName", "Subject Name");
            _dgvReport.Columns.Add("Units", "Units");
            _dgvReport.Columns.Add("EnrolledCount", "Enrolled Students");
            _dgvReport.Columns.Add("AvgGrade", "Average Grade");

            grpReport.Controls.Add(_dgvReport);
            container.Controls.Add(grpReport);
        }

        private static Panel CreateStatBox(string title, string initialVal, Color color, out Label valLabel)
        {
            Panel p = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.White
            };
            Label t = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 130),
                Location = new Point(12, 12),
                AutoSize = true
            };
            Label v = new Label
            {
                Text = initialVal,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(12, 34),
                AutoSize = true
            };
            p.Controls.Add(t);
            p.Controls.Add(v);
            valLabel = v;
            return p;
        }

        public void LoadData()
        {
            try
            {
                var students = _studentRepo.GetAll();
                var courses = _courseRepo.GetAll();
                var enrollments = _enrollmentRepo.GetAll();

                _lblTotalStudents.Text = students.Count.ToString();
                _lblTotalSubjects.Text = courses.Count.ToString();
                _lblTotalEnrollments.Text = enrollments.Count.ToString();

                // Load Students into Combo for Report Card Generation
                List<StudentComboItem> items = new List<StudentComboItem>();
                foreach (var s in students)
                {
                    items.Add(new StudentComboItem(s, s.DisplayName));
                }

                if (items.Count == 0)
                {
                    // Fallback sample student matching screenshot
                    Student sample = new Student
                    {
                        Id = 1,
                        StudentNumber = "2024-0005",
                        FirstName = "Juan",
                        MiddleName = "Miguel",
                        LastName = "Reyes",
                        CourseName = "BS Information Technology",
                        YearLevel = "2nd Year"
                    };
                    items.Add(new StudentComboItem(sample, sample.DisplayName));
                }

                _cmbReportStudent.DataSource = items;
                _cmbReportStudent.DisplayMember = "Display";
                _cmbReportStudent.ValueMember = "Student";

                _dgvReport.Rows.Clear();
                foreach (var c in courses)
                {
                    var courseEnrollments = enrollments.Where(e => e.CourseId == c.Id).ToList();
                    var graded = courseEnrollments.Where(e => e.Grade.HasValue).ToList();
                    string avgGradeStr = graded.Count > 0 ? graded.Average(e => e.Grade.Value).ToString("0.00") : "N/A";

                    _dgvReport.Rows.Add(c.Code, c.Name, c.Units, courseEnrollments.Count, avgGradeStr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPreviewReportCard_Click(object sender, EventArgs e)
        {
            if (_cmbReportStudent.SelectedValue is Student student)
            {
                using (ReportCardForm form = new ReportCardForm(student))
                {
                    form.ShowDialog(this);
                }
            }
        }

        private sealed class StudentComboItem
        {
            public Student Student { get; }
            public string Display { get; }

            public StudentComboItem(Student student, string display)
            {
                Student = student;
                Display = display;
            }
        }
    }
}
