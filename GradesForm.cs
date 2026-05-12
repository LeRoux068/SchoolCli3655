using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolCli3655
{
    public partial class GradesForm : Form
    {
        private DataGridView dgvGrades;
        private ComboBox cmbEnrollment;
        private TextBox txtAssessment, txtScore;
        private DateTimePicker dtpGradeDate;
        private Button btnAdd, btnUpdate, btnDelete, btnRefresh;

        public GradesForm()
        {
            InitializeComponent();
            LoadGrades();
        }

        private void InitializeComponent()
        {
            this.Text = "Grade Management";
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
            dgvGrades = new DataGridView
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
            dgvGrades.CellClick += DgvGrades_CellClick;
            mainLayout.Controls.Add(dgvGrades, 0, 1);

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
                BackColor = Color.FromArgb(230, 126, 34),
                Padding = new Padding(10)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                AutoSize = false
            };

            // Enrollment dropdown
            Label enrollLabel = new Label 
            { 
                Text = "Enrollment:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            cmbEnrollment = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadEnrollmentsIntoCombo();
            layout.Controls.Add(enrollLabel, 0, 0);
            layout.Controls.Add(cmbEnrollment, 0, 1);

            // Assessment
            Label assessLabel = new Label 
            { 
                Text = "Assessment:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            txtAssessment = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(assessLabel, 1, 0);
            layout.Controls.Add(txtAssessment, 1, 1);

            // Score
            Label scoreLabel = new Label 
            { 
                Text = "Score (0-100):", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            txtScore = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(scoreLabel, 2, 0);
            layout.Controls.Add(txtScore, 2, 1);

            // Date
            Label dateLabel = new Label 
            { 
                Text = "Date:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            dtpGradeDate = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            layout.Controls.Add(dateLabel, 3, 0);
            layout.Controls.Add(dtpGradeDate, 3, 1);

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateButtonPanel()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, AutoSize = false };

            btnAdd = CreateStyledButton("✚ Add Grade", Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = CreateStyledButton("✎ Update", Color.FromArgb(230, 126, 34));
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = CreateStyledButton("🗑 Delete", Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateStyledButton("🔄 Refresh", Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadGrades();

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

        private void LoadEnrollmentsIntoCombo()
        {
            DataTable dt = DatabaseManager.ExecuteQuery(
                @"SELECT e.EnrollmentID, 
                s.FirstName || ' ' || s.LastName || ' - ' || c.CourseName as EnrollmentInfo
                FROM Enrollments e
                JOIN Students s ON e.StudentID = s.StudentID
                JOIN Courses c ON e.CourseID = c.CourseID
                ORDER BY s.FirstName");
            cmbEnrollment.DataSource = dt;
            cmbEnrollment.DisplayMember = "EnrollmentInfo";
            cmbEnrollment.ValueMember = "EnrollmentID";
        }

        private void LoadGrades()
        {
            DataTable dt = DatabaseManager.ExecuteQuery(
                @"SELECT g.GradeID, 
                s.FirstName || ' ' || s.LastName as StudentName,
                c.CourseName, g.Assessment, g.Score, g.DateRecorded
                FROM Grades g
                JOIN Enrollments e ON g.EnrollmentID = e.EnrollmentID
                JOIN Students s ON e.StudentID = s.StudentID
                JOIN Courses c ON e.CourseID = c.CourseID
                ORDER BY g.DateRecorded DESC");
            dgvGrades.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string query = @"INSERT INTO Grades (EnrollmentID, Assessment, Score, DateRecorded) 
                           VALUES (@enroll, @assess, @score, @date)";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@enroll", cmbEnrollment.SelectedValue),
                new SQLiteParameter("@assess", txtAssessment.Text),
                new SQLiteParameter("@score", double.Parse(txtScore.Text)),
                new SQLiteParameter("@date", dtpGradeDate.Value.Date)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Grade added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadGrades();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvGrades.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a grade to update.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            int gradeId = (int)dgvGrades.SelectedRows[0].Cells[0].Value;
            string query = @"UPDATE Grades SET Assessment = @assess, Score = @score, DateRecorded = @date 
                           WHERE GradeID = @id";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@assess", txtAssessment.Text),
                new SQLiteParameter("@score", double.Parse(txtScore.Text)),
                new SQLiteParameter("@date", dtpGradeDate.Value.Date),
                new SQLiteParameter("@id", gradeId)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Grade updated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadGrades();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvGrades.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a grade to delete.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this grade?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int gradeId = (int)dgvGrades.SelectedRows[0].Cells[0].Value;
                string query = "DELETE FROM Grades WHERE GradeID = @id";
                
                if (DatabaseManager.ExecuteNonQuery(query, 
                    new SQLiteParameter("@id", gradeId)) > 0)
                {
                    MessageBox.Show("Grade deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGrades();
                }
            }
        }

        private void DgvGrades_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtAssessment.Text = dgvGrades.Rows[e.RowIndex].Cells[3].Value?.ToString() ?? "";
            txtScore.Text = dgvGrades.Rows[e.RowIndex].Cells[4].Value?.ToString() ?? "";
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtAssessment.Text) || 
                string.IsNullOrWhiteSpace(txtScore.Text))
            {
                MessageBox.Show("Assessment and score are required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!double.TryParse(txtScore.Text, out double score) || score < 0 || score > 100)
            {
                MessageBox.Show("Score must be a number between 0 and 100.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtAssessment.Clear();
            txtScore.Clear();
            dtpGradeDate.Value = DateTime.Now;
        }
    }
}
