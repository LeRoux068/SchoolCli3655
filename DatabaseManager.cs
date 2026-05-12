using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SchoolCli3655
{
    public class DatabaseManager
    {
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            "SchoolDatabase.db");
        
        private static readonly string ConnectionString = 
            $"Data Source={DbPath};Version=3;";

        public static void Initialize()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    CreateTables(conn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            string[] tableCreationScripts = new[]
            {
                // Students Table
                @"CREATE TABLE IF NOT EXISTS Students (
                    StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    Email TEXT UNIQUE,
                    Phone TEXT,
                    DateOfBirth DATE,
                    EnrollmentDate DATE DEFAULT CURRENT_DATE,
                    Active BOOLEAN DEFAULT 1
                );",

                // Courses Table
                @"CREATE TABLE IF NOT EXISTS Courses (
                    CourseID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CourseName TEXT NOT NULL UNIQUE,
                    CourseCode TEXT NOT NULL UNIQUE,
                    Instructor TEXT NOT NULL,
                    Credits INTEGER,
                    Description TEXT
                );",

                // Enrollment Table
                @"CREATE TABLE IF NOT EXISTS Enrollments (
                    EnrollmentID INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentID INTEGER NOT NULL,
                    CourseID INTEGER NOT NULL,
                    EnrollmentDate DATE DEFAULT CURRENT_DATE,
                    Grade REAL,
                    Status TEXT DEFAULT 'Active',
                    FOREIGN KEY(StudentID) REFERENCES Students(StudentID),
                    FOREIGN KEY(CourseID) REFERENCES Courses(CourseID)
                );",

                // Grades Table
                @"CREATE TABLE IF NOT EXISTS Grades (
                    GradeID INTEGER PRIMARY KEY AUTOINCREMENT,
                    EnrollmentID INTEGER NOT NULL,
                    Assessment TEXT NOT NULL,
                    Score REAL NOT NULL,
                    DateRecorded DATE DEFAULT CURRENT_DATE,
                    FOREIGN KEY(EnrollmentID) REFERENCES Enrollments(EnrollmentID)
                );"
            };

            foreach (string script in tableCreationScripts)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(script, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ExecuteQuery(string query, 
            params SQLiteParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static int ExecuteNonQuery(string query, 
            params SQLiteParameter[] parameters)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Execution error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public static object ExecuteScalar(string query, 
            params SQLiteParameter[] parameters)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Scalar execution error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
