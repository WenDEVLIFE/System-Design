using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace System_Design
{
    public partial class Form2 : Form
    {
        private GradientSidebarPanel _pnlSidebar;
        private Panel _pnlHeader;
        private Label _lblHeaderTitle;
        private Panel _pnlMainContent;

        private SidebarNavItem _btnNavDashboard;
        private SidebarNavItem _btnNavStudents;
        private SidebarNavItem _btnNavEnrollment;
        private SidebarNavItem _btnNavSubject;
        private SidebarNavItem _btnNavGrades;
        private SidebarNavItem _btnNavReports;
        private SidebarNavItem _btnNavUsers;
        private SidebarNavItem _btnNavSettings;
        private SidebarNavItem _btnNavLogout;

        private SidebarNavItem _currentActiveButton;

        private DashboardControl _viewDashboard;
        private StudentsViewControl _viewStudents;
        private EnrollmentViewControl _viewEnrollment;
        private SubjectViewControl _viewSubject;
        private GradesViewControl _viewGrades;
        private ReportsViewControl _viewReports;
        private UsersViewControl _viewUsers;
        private SettingsViewControl _viewSettings;

        public Form2()
        {
            InitializeComponent();
            DoubleBuffered = true;
            MinimumSize = new Size(1100, 700);

            BuildLayout();
            InitViews();
            SetActiveView(_btnNavDashboard, _viewDashboard, "Dashboard");
        }

        private void BuildLayout()
        {
            // 1. Sidebar Panel (Left)
            _pnlSidebar = new GradientSidebarPanel
            {
                Dock = DockStyle.Left,
                Width = 210
            };
            Controls.Add(_pnlSidebar);

            int btnY = 65;
            int btnHeight = 44;
            int btnSpacing = 6;

            _btnNavDashboard = CreateNavItem(NavIconType.Dashboard, "Dashboard", btnY);
            btnY += btnHeight + btnSpacing;

            _btnNavStudents = CreateNavItem(NavIconType.Students, "Students", btnY);
            btnY += btnHeight + btnSpacing;

            _btnNavEnrollment = CreateNavItem(NavIconType.Enrollment, "Enrollment", btnY);
            btnY += btnHeight + btnSpacing;

            _btnNavSubject = CreateNavItem(NavIconType.Subject, "Subject", btnY);
            btnY += btnHeight + btnSpacing;

            _btnNavGrades = CreateNavItem(NavIconType.Grades, "Grades", btnY);
            btnY += btnHeight + btnSpacing;

            _btnNavReports = CreateNavItem(NavIconType.Reports, "Reports", btnY);
            btnY += btnHeight + btnSpacing;

            _btnNavUsers = CreateNavItem(NavIconType.Users, "Users", btnY);
            btnY += btnHeight + btnSpacing;

            _btnNavSettings = CreateNavItem(NavIconType.Settings, "Settings", btnY);
            btnY += btnHeight + btnSpacing;

            // Logout at bottom of sidebar
            _btnNavLogout = CreateNavItem(NavIconType.Logout, "Logout", 670);
            _btnNavLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            _pnlSidebar.Controls.Add(_btnNavDashboard);
            _pnlSidebar.Controls.Add(_btnNavStudents);
            _pnlSidebar.Controls.Add(_btnNavEnrollment);
            _pnlSidebar.Controls.Add(_btnNavSubject);
            _pnlSidebar.Controls.Add(_btnNavGrades);
            _pnlSidebar.Controls.Add(_btnNavReports);
            _pnlSidebar.Controls.Add(_btnNavUsers);
            _pnlSidebar.Controls.Add(_btnNavSettings);
            _pnlSidebar.Controls.Add(_btnNavLogout);

            // 2. Header Panel (Top)
            _pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.FromArgb(220, 232, 250)
            };
            Controls.Add(_pnlHeader);

            _lblHeaderTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 90),
                Location = new Point(20, 12),
                AutoSize = true
            };
            _pnlHeader.Controls.Add(_lblHeaderTitle);

            User user = Session.CurrentUser;
            string username = user != null ? user.Username : "Admin";
            Label lblUserStatus = new Label
            {
                Text = string.Format("Logged as: {0}", username),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 60, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(_pnlHeader.Width - 180, 14),
                AutoSize = true
            };
            _pnlHeader.Controls.Add(lblUserStatus);

            // 3. Main Content Panel
            _pnlMainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            Controls.Add(_pnlMainContent);
            _pnlMainContent.BringToFront();
        }

        private static SidebarNavItem CreateNavItem(NavIconType icon, string title, int y)
        {
            return new SidebarNavItem(icon, title)
            {
                Location = new Point(10, y),
                Size = new Size(190, 42)
            };
        }

        private void InitViews()
        {
            _viewDashboard = new DashboardControl();
            _viewStudents = new StudentsViewControl();
            _viewEnrollment = new EnrollmentViewControl();
            _viewSubject = new SubjectViewControl();
            _viewGrades = new GradesViewControl();
            _viewReports = new ReportsViewControl();
            _viewUsers = new UsersViewControl();
            _viewSettings = new SettingsViewControl();

            _btnNavDashboard.Click += (s, e) => SetActiveView(_btnNavDashboard, _viewDashboard, "Dashboard");
            _btnNavStudents.Click += (s, e) => SetActiveView(_btnNavStudents, _viewStudents, "Students");
            _btnNavEnrollment.Click += (s, e) => SetActiveView(_btnNavEnrollment, _viewEnrollment, "Enrollment");
            _btnNavSubject.Click += (s, e) => SetActiveView(_btnNavSubject, _viewSubject, "Subject");
            _btnNavGrades.Click += (s, e) => SetActiveView(_btnNavGrades, _viewGrades, "Grades");
            _btnNavReports.Click += (s, e) => SetActiveView(_btnNavReports, _viewReports, "Reports");
            _btnNavUsers.Click += (s, e) => SetActiveView(_btnNavUsers, _viewUsers, "Users");
            _btnNavSettings.Click += (s, e) => SetActiveView(_btnNavSettings, _viewSettings, "Settings");

            _btnNavLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Session.Logout();
                    Close();
                }
            };
        }

        private void SetActiveView(SidebarNavItem item, UserControl view, string title)
        {
            if (_currentActiveButton != null)
            {
                _currentActiveButton.IsActive = false;
            }

            _currentActiveButton = item;
            _currentActiveButton.IsActive = true;

            _lblHeaderTitle.Text = title;

            _pnlMainContent.Controls.Clear();
            view.Dock = DockStyle.Fill;
            _pnlMainContent.Controls.Add(view);

            if (view is DashboardControl db) db.RefreshData();
            else if (view is StudentsViewControl st) st.LoadData();
            else if (view is EnrollmentViewControl en) en.LoadData();
            else if (view is SubjectViewControl su) su.LoadData();
            else if (view is GradesViewControl gr) gr.LoadData();
            else if (view is ReportsViewControl re) re.LoadData();
            else if (view is UsersViewControl us) us.LoadData();
        }
    }

    // Gradient Sidebar Panel with Custom Drawn Brand Header (Zero White Box Artifacts!)
    internal sealed class GradientSidebarPanel : Panel
    {
        public GradientSidebarPanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill gradient background
            using (LinearGradientBrush brush = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(10, 45, 105),
                Color.FromArgb(5, 18, 50),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            // Draw Brand Header ("🎓 SMS Admin") directly on background
            using (Font brandFont = new Font("Segoe UI", 13F, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                g.DrawString("🎓 SMS Admin", brandFont, textBrush, 15, 18);
            }
        }
    }

    internal enum NavIconType
    {
        Dashboard,
        Students,
        Enrollment,
        Subject,
        Grades,
        Reports,
        Users,
        Settings,
        Logout
    }

    // Modern Custom Sidebar Nav Control with Crisp GDI+ Vector Icons & Clean Alignment
    internal sealed class SidebarNavItem : Control
    {
        private readonly NavIconType _iconType;
        private readonly string _title;
        private bool _isActive;
        private bool _isHovered;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                Invalidate();
            }
        }

        public SidebarNavItem(NavIconType iconType, string title)
        {
            _iconType = iconType;
            _title = title;

            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw Background Pill
            if (_isActive)
            {
                using (SolidBrush activeBrush = new SolidBrush(Color.FromArgb(24, 119, 242)))
                using (GraphicsPath path = GetRoundedRectPath(ClientRectangle, 6))
                {
                    g.FillPath(activeBrush, path);
                }
            }
            else if (_isHovered)
            {
                using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(35, 255, 255, 255)))
                using (GraphicsPath path = GetRoundedRectPath(ClientRectangle, 6))
                {
                    g.FillPath(hoverBrush, path);
                }
            }

            // Draw Vector Icon on Left
            DrawVectorIcon(g, _iconType, 16, 10, 20, 20);

            // Draw Title Text at X=46 (safely after icon, crisp bold white text!)
            using (Font font = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(_title, font, textBrush, 46, 10);
            }
        }

        private static void DrawVectorIcon(Graphics g, NavIconType type, int x, int y, int w, int h)
        {
            using (Pen pen = new Pen(Color.White, 2F))
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                pen.LineJoin = LineJoin.Round;

                switch (type)
                {
                    case NavIconType.Dashboard:
                        g.FillRectangle(brush, x, y, 8, 8);
                        g.FillRectangle(brush, x + 11, y, 8, 8);
                        g.FillRectangle(brush, x, y + 11, 8, 8);
                        g.FillRectangle(brush, x + 11, y + 11, 8, 8);
                        break;

                    case NavIconType.Students:
                        g.FillEllipse(brush, x + 5, y, 10, 10);
                        g.DrawArc(pen, x + 1, y + 11, 18, 12, 180, 180);
                        break;

                    case NavIconType.Enrollment:
                        Point[] cap = {
                            new Point(x + w / 2, y + 2),
                            new Point(x + w - 1, y + 8),
                            new Point(x + w / 2, y + 14),
                            new Point(x + 1, y + 8)
                        };
                        g.FillPolygon(brush, cap);
                        g.DrawArc(pen, x + 5, y + 10, 10, 7, 0, 180);
                        break;

                    case NavIconType.Subject:
                        g.DrawEllipse(pen, x + 2, y + 6, 16, 8);
                        g.DrawEllipse(pen, x + 6, y + 2, 8, 16);
                        g.FillEllipse(brush, x + 8, y + 8, 4, 4);
                        break;

                    case NavIconType.Grades:
                        g.DrawRectangle(pen, x + 2, y + 2, 16, 17);
                        g.DrawLine(pen, x + 6, y + 7, x + 14, y + 7);
                        g.DrawLine(pen, x + 6, y + 11, x + 14, y + 11);
                        g.DrawLine(pen, x + 6, y + 15, x + 10, y + 15);
                        break;

                    case NavIconType.Reports:
                        g.DrawLine(pen, x + 2, y + 18, x + 18, y + 18);
                        g.FillRectangle(brush, x + 4, y + 10, 3, 8);
                        g.FillRectangle(brush, x + 9, y + 5, 3, 13);
                        g.FillRectangle(brush, x + 14, y + 2, 3, 16);
                        break;

                    case NavIconType.Users:
                        g.FillEllipse(brush, x + 2, y + 2, 6, 6);
                        g.DrawArc(pen, x, y + 9, 10, 8, 180, 180);

                        g.FillEllipse(brush, x + 11, y + 4, 6, 6);
                        g.DrawArc(pen, x + 9, y + 11, 10, 7, 180, 180);
                        break;

                    case NavIconType.Settings:
                        g.DrawEllipse(pen, x + 6, y + 6, 8, 8);
                        g.DrawLine(pen, x + 10, y + 1, x + 10, y + 19);
                        g.DrawLine(pen, x + 1, y + 10, x + 19, y + 10);
                        break;

                    case NavIconType.Logout:
                        g.DrawRectangle(pen, x + 2, y + 2, 10, 16);
                        g.DrawLine(pen, x + 8, y + 10, x + 18, y + 10);
                        g.DrawLine(pen, x + 14, y + 6, x + 18, y + 10);
                        g.DrawLine(pen, x + 14, y + 14, x + 18, y + 10);
                        break;
                }
            }
        }

        private static GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
