using System;
using System.Windows.Forms;

namespace PersonalTodo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // 高DPI対応
            Win32.SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}