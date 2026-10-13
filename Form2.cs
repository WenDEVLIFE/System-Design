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

            // Logout navigation item (positioned dynamically in sidebar)
            _btnNavLogout = CreateNavItem(NavIconType.Logout, "Logout", btnY);
            _pnlSidebar.Resize += (s, e) =>
            {
                if (_btnNavLogout != null && _pnlSidebar != null)
                {
                    int dynamicY = Math.Max(btnY, _pnlSidebar.ClientSize.Height - 52);
                    _btnNavLogout.Location = new Point(10, dynamicY);
                }
            };

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
                Location = new Point(_pnlHeader.Width - 210, 14),
                AutoSize = true
            };
            _pnlHeader.Controls.Add(lblUserStatus);

            Button btnHeaderLogout = new Button
            {
                Text = "Logout",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 53, 69),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 30),
                Location = new Point(_pnlHeader.Width - 95, 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnHeaderLogout.FlatAppearance.BorderSize = 0;
            btnHeaderLogout.Click += (s, e) => PerformLogout();
            _pnlHeader.Controls.Add(btnHeaderLogout);

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

        private void PerformLogout()
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Session.Logout();
                Close();
            }
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

            _btnNavLogout.Click += (s, e) => PerformLogout();
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
            BackColor = Color.FromArgb(10, 45, 105);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
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
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Seamlessly match the parent's dark gradient background to eliminate any light-gray box artifacts!
            if (Parent != null && Parent.Width > 0 && Parent.Height > 0)
            {
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                    new Rectangle(-Left, -Top, Parent.Width, Parent.Height),
                    Color.FromArgb(10, 45, 105),
                    Color.FromArgb(5, 18, 50),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bgBrush, ClientRectangle);
                }
            }
            else
            {
                using (SolidBrush darkBrush = new SolidBrush(Color.FromArgb(10, 45, 105)))
                {
                    g.FillRectangle(darkBrush, ClientRectangle);
                }
            }

            Rectangle pillRect = new Rectangle(0, 0, Width - 1, Height - 1);

            Color textColor;
            Color iconColor;

            // 2. Draw Pill Container
            if (_isActive)
            {
                // Active: Vibrant primary blue pill with crisp white text & icon
                using (SolidBrush activeBrush = new SolidBrush(Color.FromArgb(24, 119, 242)))
                using (GraphicsPath path = GetRoundedRectPath(pillRect, 6))
                {
                    g.FillPath(activeBrush, path);
                }

                textColor = Color.White;
                iconColor = Color.White;
            }
            else if (_isHovered)
            {
                // Hovered: Subtle translucent pill with clean light border
                using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
                using (Pen hoverPen = new Pen(Color.FromArgb(90, 255, 255, 255), 1F))
                using (GraphicsPath path = GetRoundedRectPath(pillRect, 6))
                {
                    g.FillPath(hoverBrush, path);
                    g.DrawPath(hoverPen, path);
                }

                textColor = Color.White;
                iconColor = Color.White;
            }
            else
            {
                // Unselected: Distinct, visible pill container on dark background so buttons are clearly visible!
                using (SolidBrush idleBrush = new SolidBrush(Color.FromArgb(22, 255, 255, 255)))
                using (Pen idlePen = new Pen(Color.FromArgb(45, 255, 255, 255), 1F))
                using (GraphicsPath path = GetRoundedRectPath(pillRect, 6))
                {
                    g.FillPath(idleBrush, path);
                    g.DrawPath(idlePen, path);
                }

                // Crisp, high-contrast readable text and icon on dark background
                textColor = Color.FromArgb(235, 242, 255);
                iconColor = Color.FromArgb(215, 230, 255);
            }

            // 3. Draw Vector Icon on Left
            DrawVectorIcon(g, _iconType, 16, 11, 20, 20, iconColor);

            // 4. Draw Title Text with bold styling
            using (Font font = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                g.DrawString(_title, font, textBrush, 46, 11);
            }
        }

        private static void DrawVectorIcon(Graphics g, NavIconType type, int x, int y, int w, int h, Color color)
        {
            using (Pen pen = new Pen(color, 2F))
            using (SolidBrush brush = new SolidBrush(color))
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
