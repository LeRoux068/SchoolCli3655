using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace SchoolCli3655
{
    public partial class StudentsForm : Form
    {
        private DataGridView dgvStudents;
        private TextBox txtFirstName, txtLastName, txtEmail, txtPhone;
        private DateTimePicker dtpDOB;
        private Button btnAdd, btnUpdate, btnDelete, btnSearch, btnRefresh;
        private TextBox txtSearch;

        public StudentsForm()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void InitializeComponent()
        {
            this.Text = "Student Management";
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

            // Header with form inputs
            Panel formPanel = CreateFormPanel();
            mainLayout.Controls.Add(formPanel, 0, 0);

            // DataGridView
            dgvStudents = new DataGridView
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
            dgvStudents.CellClick += DgvStudents_CellClick;
            mainLayout.Controls.Add(dgvStudents, 0, 1);

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
                BackColor = Color.FromArgb(52, 152, 219),
                Padding = new Padding(10)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 2,
                AutoSize = false
            };

            // Row 1
            AddLabeledControl(layout, "First Name:", ref txtFirstName, 0, 0);
            AddLabeledControl(layout, "Last Name:", ref txtLastName, 1, 0);
            AddLabeledControl(layout, "Email:", ref txtEmail, 2, 0);
            AddLabeledControl(layout, "Phone:", ref txtPhone, 3, 0);

            Label dobLabel = new Label 
            { 
                Text = "Date of Birth:", 
                Font = new Font("Segoe UI", 10), 
                ForeColor = Color.White,
                AutoSize = false,
                Height = 25,
                Dock = DockStyle.Top
            };
            dtpDOB = new DateTimePicker
            {
                Value = DateTime.Now.AddYears(-20),
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short
            };
            layout.Controls.Add(dobLabel, 4, 0);
            layout.Controls.Add(dtpDOB, 4, 1);

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
            layout.Controls.Add(searchLabel, 0, 1);
            layout.Controls.Add(txtSearch, 1, 1);

            btnSearch = CreateActionButton("🔍 Search");
            btnSearch.Click += BtnSearch_Click;
            layout.Controls.Add(btnSearch, 2, 1);

            btnRefresh = CreateActionButton("🔄 Refresh");
            btnRefresh.Click += (s, e) => LoadStudents();
            layout.Controls.Add(btnRefresh, 3, 1);

            panel.Controls.Add(layout);
            return panel;
        }

        private Panel CreateButtonPanel()
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, AutoSize = false };

            btnAdd = CreateStyledButton("✚ Add Student", Color.FromArgb(46, 204, 113));
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = CreateStyledButton("✎ Update", Color.FromArgb(52, 152, 219));
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = CreateStyledButton("🗑 Delete", Color.FromArgb(231, 76, 60));
            btnDelete.Click += BtnDelete_Click;

            panel.Controls.Add(btnAdd);
            panel.Controls.Add(btnUpdate);
            panel.Controls.Add(btnDelete);

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

        private Button CreateActionButton(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(41, 128, 185),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = new FlatButtonAppearance { BorderSize = 0 }
            };
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

        private void LoadStudents()
        {
            DataTable dt = DatabaseManager.ExecuteQuery(
                "SELECT StudentID, FirstName, LastName, Email, Phone, DateOfBirth FROM Students WHERE Active = 1 ORDER BY FirstName");
            dgvStudents.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string query = @"INSERT INTO Students (FirstName, LastName, Email, Phone, DateOfBirth) 
                           VALUES (@first, @last, @email, @phone, @dob)";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@first", txtFirstName.Text),
                new SQLiteParameter("@last", txtLastName.Text),
                new SQLiteParameter("@email", txtEmail.Text),
                new SQLiteParameter("@phone", txtPhone.Text),
                new SQLiteParameter("@dob", dtpDOB.Value.Date)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Student added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadStudents();
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to update.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            int studentId = (int)dgvStudents.SelectedRows[0].Cells[0].Value;
            string query = @"UPDATE Students SET FirstName = @first, LastName = @last, 
                           Email = @email, Phone = @phone, DateOfBirth = @dob WHERE StudentID = @id";
            
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@first", txtFirstName.Text),
                new SQLiteParameter("@last", txtLastName.Text),
                new SQLiteParameter("@email", txtEmail.Text),
                new SQLiteParameter("@phone", txtPhone.Text),
                new SQLiteParameter("@dob", dtpDOB.Value.Date),
                new SQLiteParameter("@id", studentId)
            };

            if (DatabaseManager.ExecuteNonQuery(query, parameters) > 0)
            {
                MessageBox.Show("Student updated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadStudents();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a student to delete.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int studentId = (int)dgvStudents.SelectedRows[0].Cells[0].Value;
                string query = "UPDATE Students SET Active = 0 WHERE StudentID = @id";
                
                if (DatabaseManager.ExecuteNonQuery(query, 
                    new SQLiteParameter("@id", studentId)) > 0)
                {
                    MessageBox.Show("Student deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStudents();
                }
            }
        }

        private void DgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtFirstName.Text = dgvStudents.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
            txtLastName.Text = dgvStudents.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";
            txtEmail.Text = dgvStudents.Rows[e.RowIndex].Cells[3].Value?.ToString() ?? "";
            txtPhone.Text = dgvStudents.Rows[e.RowIndex].Cells[4].Value?.ToString() ?? "";
            
            if (dgvStudents.Rows[e.RowIndex].Cells[5].Value != null)
            {
                dtpDOB.Value = Convert.ToDateTime(dgvStudents.Rows[e.RowIndex].Cells[5].Value);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadStudents();
                return;
            }

            DataTable dt = DatabaseManager.ExecuteQuery(
                "SELECT StudentID, FirstName, LastName, Email, Phone, DateOfBirth FROM Students " +
                "WHERE (FirstName LIKE @search OR LastName LIKE @search OR Email LIKE @search) AND Active = 1",
                new SQLiteParameter("@search", "%" + txtSearch.Text + "%"));
            dgvStudents.DataSource = dt;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || 
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First name and last name are required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            dtpDOB.Value = DateTime.Now.AddYears(-20);
        }
    }
}
