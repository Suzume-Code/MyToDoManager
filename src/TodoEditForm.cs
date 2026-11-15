using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersonalTodo
{
    public class TodoEditForm : Form
    {
        private WorkItem _item;
        private ComboBox _kind;
        private TextBox _title;
        private TextBox _url;
        private TextBox _body;
        private ComboBox _status;
        private ComboBox _priority;
        private DateTimePicker _due;
        private CheckBox _noDue;

        public WorkItem Result
        {
            get { return _item; }
        }

        public TodoEditForm(WorkItem item)
        {
            _item = item == null ? new WorkItem() : Copy(item);
            InitializeForm();
            LoadValues();
        }

        private WorkItem Copy(WorkItem source)
        {
            WorkItem item = new WorkItem();
            item.Id = source.Id;
            item.Kind = source.Kind;
            item.Title = source.Title;
            item.MailUrl = source.MailUrl;
            item.Body = source.Body;
            item.CreatedAt = source.CreatedAt;
            item.DueDate = source.DueDate;
            item.Status = source.Status;
            item.Priority = source.Priority;
            return item;
        }

        private void InitializeForm()
        {
            this.Text = "ToDo / メモ 登録・編集";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(650, 570);
            this.MinimumSize = new Size(600, 520);
            this.Font = Common.FONT_SIZE9;
            this.BackColor = Color.LightGray;

            TableLayoutPanel layout = new Table(2, 9);
            layout.BackColor = Color.LightGray;
            layout.Dock = DockStyle.Fill;
            layout.Padding = new Padding(12);
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _kind = new ComboBox();
            _kind.DropDownStyle = ComboBoxStyle.DropDownList;
            _kind.Items.AddRange(new object[] { "ToDo", "メモ" });
            _kind.SelectedIndexChanged += KindChanged;

            _title = new TextBox();
            _title.Width = 420;
            _url = new TextBox();
            Button paste = new Button();
            paste.Text = "貼り付け";
            paste.AutoSize = true;
            paste.Click += delegate
            {
                if (Clipboard.ContainsText())
                    _url.Text = Clipboard.GetText();
            };

            Panel urlPanel = new Panel();
            urlPanel.Dock = DockStyle.Fill;
            _url.Dock = DockStyle.Fill;
            paste.Dock = DockStyle.Right;
            urlPanel.Controls.Add(_url);
            urlPanel.Controls.Add(paste);

            _status = new ComboBox();
            _status.DropDownStyle = ComboBoxStyle.DropDownList;
            _status.Items.AddRange(new object[] { "新規", "進行中", "保留", "回答待ち", "完了" });

            _priority = new ComboBox();
            _priority.DropDownStyle = ComboBoxStyle.DropDownList;
            _priority.Items.AddRange(new object[] { "低", "中", "高" });

            _due = new DateTimePicker();
            _due.Format = DateTimePickerFormat.Short;
            _due.Width = 140;

            _noDue = new CheckBox();
            _noDue.Text = "期限なし";
            _noDue.AutoSize = true;
            _noDue.CheckedChanged += delegate { _due.Enabled = !_noDue.Checked && _kind.SelectedIndex == 0; };

            FlowLayoutPanel duePanel = new FlowLayoutPanel();
            duePanel.Dock = DockStyle.Fill;
            duePanel.Controls.Add(_due);
            duePanel.Controls.Add(_noDue);

            _body = new TextBox();
            _body.Multiline = true;
            _body.ScrollBars = ScrollBars.Vertical;
            _body.AcceptsReturn = true;
            _body.Dock = DockStyle.Fill;

            Button appendDate = new Button();
            appendDate.Text = "日時を付けて追記";
            appendDate.AutoSize = true;
            appendDate.Click += delegate
            {
                string prefix = _body.Text.Length == 0 || _body.Text.EndsWith("\r\n")
                    ? "" : "\r\n";
                _body.AppendText(prefix + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "\r\n");
                _body.Focus();
            };

            layout.Controls.Add(MakeLabel("種類"), 0, 0);
            layout.Controls.Add(_kind, 1, 0);
            layout.Controls.Add(MakeLabel("件名"), 0, 1);
            layout.Controls.Add(_title, 1, 1);
            layout.Controls.Add(MakeLabel("メールURL"), 0, 2);
            layout.Controls.Add(urlPanel, 1, 2);
            layout.Controls.Add(MakeLabel("ステータス"), 0, 3);
            layout.Controls.Add(_status, 1, 3);
            layout.Controls.Add(MakeLabel("優先度"), 0, 4);
            layout.Controls.Add(_priority, 1, 4);
            layout.Controls.Add(MakeLabel("期限"), 0, 5);
            layout.Controls.Add(duePanel, 1, 5);
            layout.Controls.Add(MakeLabel("本文・経緯"), 0, 6);
            layout.Controls.Add(_body, 1, 6);
            layout.Controls.Add(new Label(), 0, 7);
            layout.Controls.Add(appendDate, 1, 7);

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Bottom;
            buttons.Height = 48;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Padding = new Padding(8);

            Button save = new Button();
            save.Text = "保存";
            save.Width = 90;
            save.Click += SaveClicked;
            save.BackColor = Color.MediumSeaGreen;

            Button cancel = new Button();
            cancel.Text = "キャンセル";
            cancel.Width = 90;
            cancel.DialogResult = DialogResult.Cancel;

            buttons.Controls.Add(cancel);
            buttons.Controls.Add(save);
            Controls.Add(layout);
            Controls.Add(buttons);
            AcceptButton = save;
            CancelButton = cancel;
        }

        private Label MakeLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        private void LoadValues()
        {
            _kind.SelectedIndex = _item.Kind == ItemKind.Memo ? 1 : 0;
            _title.Text = _item.Title;
            _url.Text = _item.MailUrl;
            _body.Text = _item.Body;
            _status.SelectedIndex = (int)_item.Status;
            _priority.SelectedIndex = (int)_item.Priority;
            _noDue.Checked = !_item.DueDate.HasValue;
            _due.Value = _item.DueDate.HasValue ? _item.DueDate.Value : DateTime.Today.AddDays(7);
            ApplyKindRules();
        }

        private void KindChanged(object sender, EventArgs e)
        {
            ApplyKindRules();
        }

        private void ApplyKindRules()
        {
            bool isTodo = _kind.SelectedIndex == 0;
            _status.Enabled = isTodo;
            _priority.Enabled = isTodo;
            _due.Enabled = isTodo && !_noDue.Checked;
            _noDue.Enabled = isTodo;
            if (!isTodo)
                _noDue.Checked = true;
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(_title.Text))
            {
                MessageBox.Show("件名を入力してください。", "確認",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _title.Focus();
                return;
            }

            _item.Kind = _kind.SelectedIndex == 1 ? ItemKind.Memo : ItemKind.Todo;
            _item.Title = _title.Text.Trim();
            _item.MailUrl = _url.Text.Trim();
            _item.Body = _body.Text;
            if (_item.Kind == ItemKind.Memo)
            {
                _item.DueDate = null;
            }
            else
            {
                _item.Status = (TodoStatus)_status.SelectedIndex;
                _item.Priority = (Priority)_priority.SelectedIndex;
                _item.DueDate = _noDue.Checked ? (DateTime?)null : _due.Value.Date;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private class Table : TableLayoutPanel
        {
            public Table(int columns, int rows) : base()
            {
                ColumnCount = columns;
                RowCount = rows;
            }
        }
    }
}