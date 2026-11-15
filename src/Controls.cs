using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PersonalTodo
{
    public static class Common
    {
        public static string FONT_NAME              = "Meiryo UI";

        public static Font FONT_SIZE9               = new Font(FONT_NAME, 9);
        public static Font FONT_SIZE9_BOLD          = new Font(FONT_NAME, 9, FontStyle.Bold);
        public static Font FONT_SIZE10              = new Font(FONT_NAME, 10);
        public static Font FONT_SIZE10_BOLD         = new Font(FONT_NAME, 10, FontStyle.Bold);
        public static Font FONT_SIZE11              = new Font(FONT_NAME, 11);
        public static Font FONT_SIZE11_BOLD         = new Font(FONT_NAME, 11, FontStyle.Bold);
 
        //public static Color CB_DEFAULT_FROM_COLOR   = Color.FromArgb(236, 236, 236);
        //public static Color CB_DEFAULT_TO_COLOR     = Color.FromArgb(160, 160, 160);
        //public static Color CB_ENTER_FROM_COLOR     = Color.FromArgb(255, 255, 236);
        //public static Color CB_ENTER_TO_COLOR       = Color.FromArgb(255, 160, 160);

        // タブ切り替えボタン配色
        public static Color TB_DEFAULT_FROM_COLOR   = Color.FromArgb(236, 236, 236);
        public static Color TB_DEFAULT_TO_COLOR     = Color.FromArgb(160, 160, 160);
        public static Color TB_ENTER_FROM_COLOR     = Color.FromArgb(255, 255, 236);
        public static Color TB_ENTER_TO_COLOR       = Color.FromArgb(100, 160, 255);
        public static Color TB_SELECT_FROM_COLOR    = Color.FromArgb(255, 255, 236);
        public static Color TB_SELECT_TO_COLOR      = Color.FromArgb(100, 255, 160);

        //
        public static Pen WHITE_PEN                 = new Pen(Color.White, 1);
        public static Pen BLACK_PEN                 = new Pen(Color.Black, 1);

        // テキスト
        public static SolidBrush BLACK_BRUSH        = new SolidBrush(Color.Black);
        public static SolidBrush RED_BRUSH          = new SolidBrush(Color.Red);
    }

    public class CButton : RadioButton
    {
        private bool isMouseOver = false;

        private Color setFromColor;
        private Color setToColor;

        public CButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                        ControlStyles.UserPaint | 
                        ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            this.Appearance = Appearance.Button;

            setFromColor = Common.TB_DEFAULT_FROM_COLOR;
            setToColor = Common.TB_DEFAULT_TO_COLOR;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Checked) {
                setFromColor = Common.TB_SELECT_FROM_COLOR;
                setToColor = Common.TB_SELECT_TO_COLOR;
            }
            else if (isMouseOver)
            {
                setFromColor = Common.TB_ENTER_FROM_COLOR;
                setToColor = Common.TB_ENTER_TO_COLOR;
            }
            else
            {
                setFromColor = Common.TB_DEFAULT_FROM_COLOR;
                setToColor = Common.TB_DEFAULT_TO_COLOR;
            }

            RectangleF rect = e.Graphics.VisibleClipBounds;

            LinearGradientBrush gb = new LinearGradientBrush(
                    rect,
                    setFromColor,
                    setToColor,
                    LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(gb, rect);

            e.Graphics.DrawLine(Common.WHITE_PEN, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
            e.Graphics.DrawLine(Common.WHITE_PEN, rect.X, rect.Y, rect.X + rect.Width, rect.Y);
            e.Graphics.DrawLine(Common.BLACK_PEN, rect.X + rect.Width - 1, rect.Y, rect.X + rect.Width - 1, rect.Y + rect.Height);
            e.Graphics.DrawLine(Common.BLACK_PEN, rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width, rect.Y + rect.Height - 1);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            
            e.Graphics.DrawString(this.Text, Common.FONT_SIZE11_BOLD, Common.BLACK_BRUSH, rect, stringFormat);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            
            isMouseOver = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            isMouseOver = false;
        }
    }


    public class CDataGridView : DataGridView
    {
        public CDataGridView()
        {
            this.DoubleBuffered = true;
        }
    }


    public class CToolStripButtonX : ToolStripButton
    {
        private bool isMouseOver = false;

        public Color HoverBackColor { get; set; } // 薄い青など
        public Color HoverForeColor { get; set; }

        public CToolStripButtonX(string text)
        {
            this.Text = text;

            HoverBackColor = Color.FromArgb(220, 235, 252);
            HoverForeColor = Color.Red;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isMouseOver = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isMouseOver = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (isMouseOver)
            {
                using (SolidBrush brush = new SolidBrush(HoverBackColor))
                {
                    e.Graphics.FillRectangle(brush, this.ContentRectangle);

                }
                //    StringFormat stringFormat = new StringFormat();
                //    stringFormat.Alignment = StringAlignment.Center;
                //    stringFormat.LineAlignment = StringAlignment.Center;
                    
                //    e.Graphics.DrawString(this.Text, Common.FONT_SIZE11, Common.RED_BRUSH, e.Graphics.VisibleClipBounds, stringFormat);
            }

            base.OnPaint(e);
        }
    }

    public class CToolStripButton : ToolStripButton
    {
        private Bitmap _scaledIcon;

        public CToolStripButton(string text)
        {
            this.Text = text;
            
            this.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            this.ImageScaling =
                ToolStripItemImageScaling.None;

            this.TextImageRelation =
                TextImageRelation.ImageBeforeText;

            this.ImageAlign = ContentAlignment.MiddleCenter;
            this.TextAlign = ContentAlignment.MiddleCenter;

            this.AutoSize = true;
            this.Padding = new Padding(8, 5, 8, 5);
            this.Margin = new Padding(2, 2, 2, 2);
        }

        /// <summary>
        /// 16x16アイコン画像
        /// 設定された画像は自動的に16x16に変換される
        /// </summary>
        public Image IconImage
        {
            get
            {
                return this.Image;
            }
            set
            {
                if (_scaledIcon != null)
                {
                    this.Image = null;
                    _scaledIcon.Dispose();
                    _scaledIcon = null;
                }

                if (value == null)
                {
                    this.Image = null;
                    return;
                }

                _scaledIcon = new Bitmap(16, 16);

                using (Graphics g =
                    Graphics.FromImage(_scaledIcon))
                {
                    g.Clear(Color.Transparent);

                    g.InterpolationMode =
                        System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    g.DrawImage(
                        value,
                        new Rectangle(0, 0, 16, 16));
                }

                this.Image = _scaledIcon;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Image = null;

                if (_scaledIcon != null)
                {
                    _scaledIcon.Dispose();
                    _scaledIcon = null;
                }
            }

            base.Dispose(disposing);
        }
    }

    public class ModernCheckBox : UserControl
    {
        private bool _checked;
        private bool _hover;

        private Color _accentColor = Color.MediumSeaGreen;
        //    Color.FromArgb(99, 102, 241);

        private Color _uncheckedColor = Color.Silver;
        //    Color.FromArgb(65, 72, 88);

        private Color _textColor = Color.Black;
        //    Color.FromArgb(230, 230, 240);

        private Color _boxBackColor = Color.LightGray;
        //    Color.FromArgb(45, 51, 68);

        private Timer _timer;
        private float _checkProgress;

        public event EventHandler CheckedChanged;

        public ModernCheckBox()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true
            );

            Size = new Size(240, 36);
            Font = new Font("Meiryo UI", 10F);
            ForeColor = _textColor;
            BackColor = Color.Transparent;

            Cursor = Cursors.Hand;
            TabStop = true;

            _timer = new Timer();
            _timer.Interval = 15;
            _timer.Tick += Timer_Tick;
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked == value)
                    return;

                _checked = value;

                _timer.Start();

                if (CheckedChanged != null)
                    CheckedChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        public Color AccentColor
        {
            get { return _accentColor; }
            set
            {
                _accentColor = value;
                Invalidate();
            }
        }

        public Color UncheckedColor
        {
            get { return _uncheckedColor; }
            set
            {
                _uncheckedColor = value;
                Invalidate();
            }
        }

        public Color BoxBackColor
        {
            get { return _boxBackColor; }
            set
            {
                _boxBackColor = value;
                Invalidate();
            }
        }

        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                Invalidate();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            float target = _checked ? 1f : 0f;

            float diff = target - _checkProgress;

            if (Math.Abs(diff) < 0.05f)
            {
                _checkProgress = target;
                _timer.Stop();
            }
            else
            {
                _checkProgress += diff * 0.30f;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int boxSize = 22;
            int boxX = 2;
            int boxY = (Height - boxSize) / 2;

            Rectangle rect = new Rectangle(
                boxX, boxY, boxSize, boxSize
            );

            Color fillColor = _checked
                ? _accentColor
                : _boxBackColor;

            Color borderColor = _hover
                ? _accentColor
                : _uncheckedColor;

            using (GraphicsPath path =
                CreateRoundRect(rect, 5))
            {
                using (SolidBrush brush =
                    new SolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }

                using (Pen pen =
                    new Pen(borderColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Check mark animation
            if (_checkProgress > 0.01f)
            {
                PointF p1 = new PointF(
                    boxX + 5,
                    boxY + 11
                );

                PointF p2 = new PointF(
                    boxX + 9,
                    boxY + 15
                );

                PointF p3 = new PointF(
                    boxX + 17,
                    boxY + 7
                );

                PointF mid = new PointF(
                    p1.X + (p2.X - p1.X) *
                    Math.Min(1f, _checkProgress * 2f),

                    p1.Y + (p2.Y - p1.Y) *
                    Math.Min(1f, _checkProgress * 2f)
                );

                PointF end = new PointF(
                    p2.X + (p3.X - p2.X) *
                    Math.Max(0f,
                    (_checkProgress - 0.5f) * 2f),

                    p2.Y + (p3.Y - p2.Y) *
                    Math.Max(0f,
                    (_checkProgress - 0.5f) * 2f)
                );

                using (Pen pen =
                    new Pen(Color.White, 2.2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    g.DrawLine(pen, p1, mid);

                    if (_checkProgress > 0.5f)
                        g.DrawLine(pen, p2, end);
                }
            }

            // Text
            using (SolidBrush brush =
                new SolidBrush(_textColor))
            {
                Rectangle textRect = new Rectangle(
                    34, 0,
                    Width - 36,
                    Height
                );

                StringFormat sf = new StringFormat();

                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString(
                    Text,
                    Font,
                    brush,
                    textRect,
                    sf
                );

                sf.Dispose();
            }

            // Focus
            if (Focused && ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(
                    g,
                    new Rectangle(0, 0,
                        Width - 1, Height - 1)
                );
            }
        }

        private GraphicsPath CreateRoundRect(
            Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int d = radius * 2;

            path.AddArc(
                rect.X, rect.Y, d, d, 180, 90
            );

            path.AddArc(
                rect.Right - d,
                rect.Y, d, d, 270, 90
            );

            path.AddArc(
                rect.Right - d,
                rect.Bottom - d,
                d, d, 0, 90
            );

            path.AddArc(
                rect.X,
                rect.Bottom - d,
                d, d, 90, 90
            );

            path.CloseFigure();

            return path;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Checked = !Checked;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space ||
                e.KeyCode == Keys.Enter)
            {
                Checked = !Checked;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            base.Dispose(disposing);
        }
    }
}