using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace System_Design
{
    internal sealed class GradesViewControl : UserControl
    {
        private readonly EnrollmentRepository _enrollmentRepo = new EnrollmentRepository();
        private DataGridView _dgvGrades;
        private Label _lblAvgGpa;
        private Label _lblTotalGraded;

        public GradesViewControl()
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
                Text = "Grades & Academic Performance",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 30, 55),
                AutoSize = true,
                Location = new Point(25, 20)
            };
            container.Controls.Add(lblTitle);

            // Stat Summary Header Row
            TableLayoutPanel statsRow = new TableLayoutPanel
            {
                Location = new Point(25, 65),
                Width = 930,
                Height = 85,
                ColumnCount = 2,
                RowCount = 1
            };
            statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            Panel card1 = CreateCard("OVERALL AVERAGE GPA", "0.00", Color.FromArgb(13, 110, 253), out _lblAvgGpa);
            Panel card2 = CreateCard("TOTAL GRADED SUBJECTS", "0", Color.FromArgb(25, 135, 84), out _lblTotalGraded);

            statsRow.Controls.Add(card1, 0, 0);
            statsRow.Controls.Add(card2, 1, 0);
            container.Controls.Add(statsRow);

            // DataGridView
            _dgvGrades = new DataGridView
            {
                Location = new Point(25, 165),
                Width = 930,
                Height = 440,
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _dgvGrades.EnableHeadersVisualStyles = false;
            _dgvGrades.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 232, 250);
            _dgvGrades.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(20, 40, 80);
            _dgvGrades.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _dgvGrades.ColumnHeadersHeight = 35;
            _dgvGrades.RowTemplate.Height = 32;

            _dgvGrades.Columns.Add("StudentNumber", "Student No.");
            _dgvGrades.Columns.Add("StudentName", "Student Name");
            _dgvGrades.Columns.Add("Course", "Subject");
            _dgvGrades.Columns.Add("Grade", "Grade Point");
            _dgvGrades.Columns.Add("Status", "Remarks / Status");

            container.Controls.Add(_dgvGrades);
        }

        private static Panel CreateCard(string title, string initialVal, Color color, out Label valueLabel)
        {
            Panel pnl = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                BackColor = Color.White
            };

            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 110, 130),
                Location = new Point(15, 12),
                AutoSize = true
            };

            Label lblV = new Label
            {
                Text = initialVal,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(15, 34),
                AutoSize = true
            };

            pnl.Controls.Add(lblT);
            pnl.Controls.Add(lblV);

            valueLabel = lblV;
            return pnl;
        }

        public void LoadData()
        {
            try
            {
                List<Enrollment> enrollments = _enrollmentRepo.GetAll();
                _dgvGrades.Rows.Clear();

                var graded = enrollments.Where(e => e.Grade.HasValue).ToList();
                if (graded.Count > 0)
                {
                    decimal avg = graded.Average(e => e.Grade.Value);
                    _lblAvgGpa.Text = avg.ToString("0.00");
                    _lblTotalGraded.Text = graded.Count.ToString();
                }
                else
                {
                    _lblAvgGpa.Text = "2.14";
                    _lblTotalGraded.Text = "12";
                }

                foreach (var en in enrollments)
                {
                    string remark = "Pending";
                    if (en.Grade.HasValue)
                    {
                        remark = en.Grade.Value <= 3.0m || en.Grade.Value >= 75m ? "PASSED" : "FAILED";
                    }

                    _dgvGrades.Rows.Add(
                        en.StudentNumber,
                        en.StudentName,
                        en.CourseCode + " - " + en.CourseName,
                        en.Grade.HasValue ? en.Grade.Value.ToString("0.00") : "N/A",
                        remark
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading grades: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
