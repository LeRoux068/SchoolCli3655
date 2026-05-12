using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolCli3655
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ConfigureStyles();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "School Management System";
            this.Icon = SystemIcons.Application;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main container
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
                AutoSize = false
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Header
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            Label titleLabel = new Label
            {
                Text = "📚 School Management System",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerPanel.Controls.Add(titleLabel);

            // Content area
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // Navigation buttons
            TableLayoutPanel navPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 200,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(0)
            };

            Button btnStudents = CreateNavButton("👤 Students", navPanel_Click);
            Button btnCourses = CreateNavButton("📖 Courses", btnCourses_Click);
            Button btnEnrollments = CreateNavButton("✓ Enrollments", btnEnrollments_Click);
            Button btnGrades = CreateNavButton("📊 Grades", btnGrades_Click);
            Button btnExit = CreateNavButton("❌ Exit", btnExit_Click);

            navPanel.Controls.Add(btnStudents, 0, 0);
            navPanel.Controls.Add(btnCourses, 0, 1);
            navPanel.Controls.Add(btnEnrollments, 0, 2);
            navPanel.Controls.Add(btnGrades, 0, 3);
            navPanel.Controls.Add(btnExit, 0, 4);

            contentPanel.Controls.Add(navPanel);

            mainPanel.Controls.Add(headerPanel, 0, 0);
            mainPanel.Controls.Add(contentPanel, 0, 1);

            this.Controls.Add(mainPanel);
        }

        private Button CreateNavButton(string text, EventHandler click)
        {
            Button btn = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 73, 94),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = new FlatButtonAppearance { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            btn.Click += click;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(41, 128, 185);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(52, 73, 94);
            return btn;
        }

        private void ConfigureStyles()
        {
            this.BackColor = Color.FromArgb(236, 240, 241);
        }

        private void navPanel_Click(object sender, EventArgs e)
        {
            StudentsForm form = new StudentsForm();
            form.ShowDialog();
        }

        private void btnCourses_Click(object sender, EventArgs e)
        {
            CoursesForm form = new CoursesForm();
            form.ShowDialog();
        }

        private void btnEnrollments_Click(object sender, EventArgs e)
        {
            EnrollmentsForm form = new EnrollmentsForm();
            form.ShowDialog();
        }

        private void btnGrades_Click(object sender, EventArgs e)
        {
            GradesForm form = new GradesForm();
            form.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
