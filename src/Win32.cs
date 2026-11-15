using System;
using System.Runtime.InteropServices;

namespace PersonalTodo
{
    public static class Win32
    {
        // プロセスをシステム DPI 対応に設定するための Windows API 関数です。
        // この関数を使用すると、高解像度ディスプレイでアプリケーションのスケーリングが
        // 適切に行われるようになります。
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        // Windows のデスクトップ ウィンドウ マネージャー (DWM) を使用して、
        // ウィンドウの属性を設定するための関数です。
        // この関数は、特定のウィンドウの外観や動作をカスタマイズする際に使用されます。
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    }
}