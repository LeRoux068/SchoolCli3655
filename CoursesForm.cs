using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolCli3655
{
    public partial class CoursesForm : Form
    {
        private DataGridView dgvCourses;
        private TextBox txtCourseName, txtCourseCode, txtInstructor, txtCredits, txtDescription;
        private Button btnAdd, btnUpdate, btnDelete, btnRefresh;
        private TextBox txtSearch;

        public CoursesForm()
        {
            InitializeComponent();
            LoadCourses();
        }

        private void InitializeComponent()
        {
            this.Text = "Course Management";
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
            dgvCourses = new DataGridView
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
            dgvCourses.CellClick += DgvCourses_CellClick;
            mainLayout.Controls.Add(dgvCourses, 0, 1);

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
                BackColor = Color.FromArgb(155, 89, 182),
                Padding = new Padding(10)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                AutoSize = false
            };

            AddLabeledControl(layout, "Course Name:", ref txtCourseName, 0, 0);
            AddLabeledControl(layout, "Course Code:", ref txtCourseCode, 1, 0);
            AddLabeledControl(layout, "Instructor:", ref txtInstructor, 2, 0);
            AddLabeledControl(layout, "Credits:", ref txtCredits, 3, 0);

            Label descLabel = new Label 
            { 
                Text = "Description:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            txtDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                Height = 40
            };
            layout.Controls.Add(descLabel, 0, 1);
            layout.Controls.Add(txtDescription, 1, 1);

            // Search
            Label searchLabel = new Label 
            { 
                Text = "Search:", 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            txtSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(searchLabel, 2, 1);
            layout.Controls.Add(txtSearch, 3, 1);

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateButtonPanel()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, AutoSize = false };

            btnAdd = CreateStyledButton("✚ Add Course", Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = CreateStyledButton("✎ Update", Color.FromArgb(155, 89, 182));
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = CreateStyledButton("🗑 Delete", Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = CreateStyledButton("🔄 Refresh", Color.FromArgb(52, 152, 219));
            btnRefresh.Click += (s, e) => LoadCourses();

            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnUpdate);
            panel.Controls.Add(btnDelete);
            panel.Controls.Add(btnRefresh);

            return panel;
        }

        private void AddLabeledControl(TableLayoutPanel layout, string label, 
            ref TextBox control, int col, int row)
        {
            Label lbl = new Label 
            { 
                Text = label, 
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Top
            };
            control = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            layout.Controls.Add(lbl, col, row);
            layout.Controls.Add(control, col, row + 1);
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

        private void LoadCourses()
        {
            DataTable dt = DatabaseManager.ExecuteQuery(
                "SELECT CourseID, CourseName, CourseCode, Instructor, Credits, Description FROM Courses ORDER BY CourseName");
            dgvCourses.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string query = @"INSERT INTO Courses (CourseName, CourseCode, Instructor, Credits, Description) 
                           VALUES (@name, @code, @instr, @credits, @desc)";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@name", txtCourseName.Text),
                new SQLiteParameter("@code", txtCourseCode.Text),
                new SQLiteParameter("@instr", txtInstructor.Text),
                new SQLiteParameter("@credits", int.Parse(txtCredits.Text)),
                new SQLiteParameter("@desc", txtDescription.Text)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Course added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadCourses();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a course to update.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            int courseId = (int)dgvCourses.SelectedRows[0].Cells[0].Value;
            string query = @"UPDATE Courses SET CourseName = @name, CourseCode = @code, 
                           Instructor = @instr, Credits = @credits, Description = @desc WHERE CourseID = @id";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@name", txtCourseName.Text),
                new SQLiteParameter("@code", txtCourseCode.Text),
                new SQLiteParameter("@instr", txtInstructor.Text),
                new SQLiteParameter("@credits", int.Parse(txtCredits.Text)),
                new SQLiteParameter("@desc", txtDescription.Text),
                new SQLiteParameter("@id", courseId)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Course updated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadCourses();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a course to delete.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this course?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int courseId = (int)dgvCourses.SelectedRows[0].Cells[0].Value;
                string query = "DELETE FROM Courses WHERE CourseID = @id";
                
                if (DatabaseManager.ExecuteNonQuery(query, 
                    new SQLiteParameter("@id", courseId)) > 0)
                {
                    MessageBox.Show("Course deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCourses();
                }
            }
        }

        private void DgvCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtCourseName.Text = dgvCourses.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
            txtCourseCode.Text = dgvCourses.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";
            txtInstructor.Text = dgvCourses.Rows[e.RowIndex].Cells[3].Value?.ToString() ?? "";
            txtCredits.Text = dgvCourses.Rows[e.RowIndex].Cells[4].Value?.ToString() ?? "";
            txtDescription.Text = dgvCourses.Rows[e.RowIndex].Cells[5].Value?.ToString() ?? "";
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtCourseName.Text) || 
                string.IsNullOrWhiteSpace(txtCourseCode.Text) ||
                string.IsNullOrWhiteSpace(txtInstructor.Text))
            {
                MessageBox.Show("Course name, code, and instructor are required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtCredits.Text, out _))
            {
                MessageBox.Show("Credits must be a valid number.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtCourseName.Clear();
            txtCourseCode.Clear();
            txtInstructor.Clear();
            txtCredits.Clear();
            txtDescription.Clear();
        }
    }
}
