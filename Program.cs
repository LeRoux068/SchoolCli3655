using System;
using System.Windows.Forms;

namespace SchoolCli3655
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Initialize database
            DatabaseManager.Initialize();
            
            Application.Run(new MainForm());
        }
    }
}
