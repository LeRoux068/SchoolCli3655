using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolCli3655
{
    public partial class EnrollmentsForm : Form
    {
        private DataGridView dgvEnrollments;
        private ComboBox cmbStudent, cmbCourse;
        private TextBox txtStatus;
        private DateTimePicker dtpEnrollmentDate;
        private Button btnAdd, btnUpdate, btnDelete, btnRefresh;

        public EnrollmentsForm()
        {
            InitializeComponent();
            LoadEnrollments();
        }

        private void InitializeComponent()
        {
            this.Text = "Enrollment Management";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(15)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            // Form inputs
            Panel formPanel = CreateFormPanel();
            mainLayout.Controls.Add(formPanel, 0, 0);

            // DataGridView
            dgvEnrollments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                BackgroundColor = Color.White,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle 
                { 
                    BackColor = Color.FromArgb(240, 240, 240) 
                }
            };
            dgvEnrollments.CellClick += DgvEnrollments_CellClick;
            mainLayout.Controls.Add(dgvEnrollments, 0, 1);

            // Buttons
            Panel buttonPanel = CreateButtonPanel();
            mainLayout.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(mainLayout);
            this.BackColor = Color.FromArgb(236, 240, 241);
        }

        private Panel CreateFormPanel()
        {
            Panel panel = new Panel
            {
                BackColor = Color.FromArgb(26, 188, 156),
                Padding = new Padding(10)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                AutoSize = false
            };

            // Student dropdown
            Label studentLabel = new Label 
            { 
                Text = "Student:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            cmbStudent = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadStudentsIntoCombo();
            layout.Controls.Add(studentLabel, 0, 0);
            layout.Controls.Add(cmbStudent, 0, 1);

            // Course dropdown
            Label courseLabel = new Label 
            { 
                Text = "Course:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            cmbCourse = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadCoursesIntoCombo();
            layout.Controls.Add(courseLabel, 1, 0);
            layout.Controls.Add(cmbCourse, 1, 1);

            // Enrollment Date
            Label dateLabel = new Label 
            { 
                Text = "Enrollment Date:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            dtpEnrollmentDate = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            layout.Controls.Add(dateLabel, 2, 0);
            layout.Controls.Add(dtpEnrollmentDate, 2, 1);

            // Status
            Label statusLabel = new Label 
            { 
                Text = "Status:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            txtStatus = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Text = "Active"
            };
            layout.Controls.Add(statusLabel, 3, 0);
            layout.Controls.Add(txtStatus, 3, 1);

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateButtonPanel()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, AutoSize = false };

            btnAdd = CreateStyledButton("✚ Enroll Student", Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = CreateStyledButton("✎ Update", Color.FromArgb(26, 188, 156));
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = CreateStyledButton("🗑 Unenroll", Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateStyledButton("🔄 Refresh", Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadEnrollments();

            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnUpdate);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnRefresh);

            return panel;
        }

        private Button CreateStyledButton(string text, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 150,
                Height = 45,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = new FlatButtonAppearance { BorderSize = 0 },
                Cursor = Cursors.Hand,
                Margin = new Padding(5)
            };
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Dark(btn.BackColor, 0.1f);
            btn.MouseLeave += (s, e) => btn.BackColor = color;
            return btn;
        }

        private void LoadStudentsIntoCombo()
        {
            DataTable dt = DatabaseManager.ExecuteQuery(
                "SELECT StudentID, FirstName || ' ' || LastName as FullName FROM Students WHERE Active = 1");
            cmbStudent.DataSource = dt;
            cmbStudent.DisplayMember = "FullName";
            cmbStudent.ValueMember = "StudentID";
        }

        private void LoadCoursesIntoCombo()
        {
            DataTable dt = DatabaseManager.ExecuteQuery(
                "SELECT CourseID, CourseName FROM Courses");
            cmbCourse.DataSource = dt;
            cmbCourse.DisplayMember = "CourseName";
            cmbCourse.ValueMember = "CourseID";
        }

        private void LoadEnrollments()
        {
            DataTable dt = DatabaseManager.ExecuteQuery(
                @"SELECT e.EnrollmentID, s.FirstName || ' ' || s.LastName as StudentName, 
                c.CourseName, e.EnrollmentDate, e.Status, COALESCE(e.Grade, 'N/A') as Grade
                FROM Enrollments e
                JOIN Students s ON e.StudentID = s.StudentID
                JOIN Courses c ON e.CourseID = c.CourseID
                ORDER BY e.EnrollmentDate DESC");
            dgvEnrollments.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbStudent.SelectedValue == null || cmbCourse.SelectedValue == null)
            {
                MessageBox.Show("Please select both student and course.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"INSERT INTO Enrollments (StudentID, CourseID, EnrollmentDate, Status) 
                           VALUES (@student, @course, @date, @status)";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@student", cmbStudent.SelectedValue),
                new SQLiteParameter("@course", cmbCourse.SelectedValue),
                new SQLiteParameter("@date", dtpEnrollmentDate.Value.Date),
                new SQLiteParameter("@status", txtStatus.Text)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Enrollment added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEnrollments();
                LoadStudentsIntoCombo();
                LoadCoursesIntoCombo();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEnrollments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an enrollment to update.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int enrollmentId = (int)dgvEnrollments.SelectedRows[0].Cells[0].Value;
            string query = @"UPDATE Enrollments SET Status = @status WHERE EnrollmentID = @id";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@status", txtStatus.Text),
                new SQLiteParameter("@id", enrollmentId)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Enrollment updated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEnrollments();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEnrollments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an enrollment to delete.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to unenroll this student?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int enrollmentId = (int)dgvEnrollments.SelectedRows[0].Cells[0].Value;
                string query = "DELETE FROM Enrollments WHERE EnrollmentID = @id";
                
                if (DatabaseManager.ExecuteNonQuery(query, 
                    new SQLiteParameter("@id", enrollmentId)) > 0)
                {
                    MessageBox.Show("Enrollment deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEnrollments();
                }
            }
        }

        private void DgvEnrollments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtStatus.Text = dgvEnrollments.Rows[e.RowIndex].Cells[4].Value?.ToString() ?? "Active";
        }
    }
}
