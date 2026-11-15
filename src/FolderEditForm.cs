using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersonalTodo
{
    public class FolderEditForm : Form
    {
        private FolderBookmark _folder;
        private TextBox _name;
        private TextBox _path;
        private TextBox _note;

        public FolderBookmark Result { get { return _folder; } }

        public FolderEditForm(FolderBookmark folder)
        {
            _folder = folder == null ? new FolderBookmark() : Copy(folder);
            this.Text = "フォルダー覚書";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(560, 330);
            this.Font = new Font("Meiryo UI", 9F);
            this.BackColor = Color.LightGray;

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.Padding = new Padding(12);
            table.ColumnCount = 2;
            table.RowCount = 3;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _name = new TextBox();
            _path = new TextBox();
            _note = new TextBox();
            _note.Multiline = true;
            _note.ScrollBars = ScrollBars.Vertical;

            AddRow(table, "名前", _name, 0);
            AddRow(table, "パス", _path, 1);
            AddRow(table, "用途・備考", _note, 2);

            _name.Text = _folder.Name;
            _path.Text = _folder.Path;
            _note.Text = _folder.Note;

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.Dock = DockStyle.Bottom;
            buttons.Height = 48;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Padding = new Padding(8);

            Button save = new Button();
            save.Text = "保存";
            save.Width = 90;
            save.Click += delegate
            {
                if (String.IsNullOrWhiteSpace(_name.Text) ||
                    String.IsNullOrWhiteSpace(_path.Text))
                {
                    MessageBox.Show("名前とパスを入力してください。");
                    return;
                }
                _folder.Name = _name.Text.Trim();
                _folder.Path = _path.Text.Trim();
                _folder.Note = _note.Text;
                DialogResult = DialogResult.OK;
                Close();
            };
            save.BackColor = Color.MediumSeaGreen;

            Button cancel = new Button();
            cancel.Text = "キャンセル";
            cancel.Width = 90;
            cancel.DialogResult = DialogResult.Cancel;
            buttons.Controls.Add(cancel);
            buttons.Controls.Add(save);

            Controls.Add(table);
            Controls.Add(buttons);
            AcceptButton = save;
            CancelButton = cancel;
        }

        private FolderBookmark Copy(FolderBookmark source)
        {
            FolderBookmark item = new FolderBookmark();
            item.Id = source.Id;
            item.Name = source.Name;
            item.Path = source.Path;
            item.Note = source.Note;
            return item;
        }

        private void AddRow(TableLayoutPanel table, string labelText, Control control, int row)
        {
            Label label = new Label();
            label.Text = labelText;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            control.Dock = DockStyle.Fill;
            table.Controls.Add(label, 0, row);
            table.Controls.Add(control, 1, row);
        }
    }
}