using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace PersonalTodo
{
    /// <summary>
    /// 
    /// </summary>
    public static class Resources {

        /// <summary>
        /// 指定したビットマップをアセンブリ情報から取得する。
        /// </summary>
        /// <param name="filePath">アセンブリ上のパスを設定する。</param>
        /// <returns>Bitmap形式で返却する。読み込めない場合はアプリケーションを終了する。</returns>
        public static Bitmap GetBitmapFromAssembly(string filePath)
        {
            try
            {
                Assembly assem = Assembly.GetExecutingAssembly();
                using (Stream reader = assem.GetManifestResourceStream(filePath))
                    return new Bitmap(Image.FromStream(reader));
            }
            catch (Exception ex)
            {
                string msg = String.Format("ファイル: {0}がアセンブリリソース上にありません。\n{1}", filePath, ex.Message);
                throw new Exception(msg);
            }
        }
    }
}