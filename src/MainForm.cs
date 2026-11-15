using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace PersonalTodo
{
    public class MainForm : Form
    {
        private AppData _data;
        private CDataGridView _grid;
        private TextBox _search;
        private Label _summary;
        private string _view = "Todo";
        private ModernCheckBox _filter;

        private readonly string _dueExceeded    = "期日超過 😰";
        private readonly string _dueToday       = "期日当日 😕";
        private readonly string _dueApproaching = "期日間近";

        private readonly string _enableFilter   = "フィルタを有効にする";

        private readonly string _priorityHigh   = "高";
        private readonly string _priorityMiddle = "中";
        private readonly string _priorityLow    = "低";

        public MainForm()
        {
            _data = DataStore.Load();
            InitializeForm();
            RefreshGrid();
        }

        private void InitializeForm()
        {
            this.Text = "My ToDo Manager";
            this.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
            this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            this.Size = new Size(1120, 700);
            this.MinimumSize = new Size(900, 550);
            this.Font = new Font("Meiryo UI", 9F);
            this.BackColor = Color.LightGray;
            this.Padding = new Padding(8, 0, 8, 0);

            Panel top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 48;

            FlowLayoutPanel nav = new FlowLayoutPanel();
            nav.Dock = DockStyle.Left;
            nav.Width = 500;
            nav.WrapContents = false;
            nav.Padding = new Padding(6, 7, 0, 0);

            AddNavButton(nav, "ToDo", "Todo", true);
            AddNavButton(nav, "メモ", "Memo", false);
            AddNavButton(nav, "フォルダー", "Folder", false);
            AddNavButton(nav, "完了一覧", "Completed", false);

            Label searchLabel = new Label();
            searchLabel.Text = "フィルタ";
            searchLabel.AutoSize = true;
            searchLabel.Location = new Point(630, 17);
            searchLabel.Font = Common.FONT_SIZE11_BOLD;

            _search = new TextBox();
            _search.Text = string.Empty;
            _search.Location = new Point(695, 15);
            _search.Size = new Size(210, 24);
            _search.BackColor = Color.Gainsboro;
            _search.Font = Common.FONT_SIZE11;
            _search.TextChanged += delegate { RefreshGrid(); };

            _filter = new ModernCheckBox();
            _filter.Location = new Point(920, 15);
            _filter.Size = new Size(220, 28);
            _filter.Text = _enableFilter;
            _filter.Font = Common.FONT_SIZE9_BOLD;
            _filter.CheckedChanged += delegate { RefreshGrid(); };

            top.Controls.Add(searchLabel);
            top.Controls.Add(_search);
            top.Controls.Add(_filter);
            top.Controls.Add(nav);

            ToolStrip tools = new ToolStrip();
            tools.GripStyle = ToolStripGripStyle.Hidden;
            tools.BackColor = Color.LightGray;
            tools.Dock = DockStyle.Top;
            tools.Font = Common.FONT_SIZE10;
            tools.Items.Add(new ToolStripSeparator());
            tools.Items.Add(MakeToolButton("新規登録", "additem.png", delegate { AddItem(); }));
            tools.Items.Add(MakeToolButton("編集", "edit.png", delegate { EditSelected(); }));
            tools.Items.Add(MakeToolButton("削除", "remove.png", delegate { DeleteSelected(); }));
            tools.Items.Add(new ToolStripSeparator());
            tools.Items.Add(MakeToolButton("開く", "link.png", delegate { OpenSelected(); }));
            tools.Items.Add(MakeToolButton("完了にする", "checked.png", delegate { CompleteSelected(); }));

            _grid = new CDataGridView();
            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = false;
            _grid.RowHeadersVisible = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.CellDoubleClick += delegate { OpenOrEditByDoubleClick(); };
            _grid.CellFormatting += GridCellFormatting;
            _grid.DefaultCellStyle.Font = Common.FONT_SIZE11_BOLD;
            _grid.RowTemplate.Height = 48;

            _summary = new Label();
            _summary.Dock = DockStyle.Bottom;
            _summary.Height = 26;
            _summary.Padding = new Padding(8, 4, 0, 0);
            _summary.ForeColor = Color.Black;

            this.Controls.Add(_grid);
            this.Controls.Add(_summary);
            this.Controls.Add(tools);
            this.Controls.Add(top);

            CreateColumns();
            FormClosing += delegate
            {
                try { DataStore.Save(_data); }
                catch (Exception ex)
                {
                    MessageBox.Show("データ保存に失敗しました。\r\n" + ex.Message,
                        "保存エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void AddNavButton(FlowLayoutPanel panel, string text, string view, bool check)
        {
            CButton button = new CButton();
            button.Text = text;
            button.Width = 105;
            button.Height = 30;
            button.Checked = check;
            button.Name = text;
            button.CheckedChanged += delegate
            {
                _view = view;
                RefreshGrid();
            };
            panel.Controls.Add(button);
        }

        private CToolStripButton MakeToolButton(string text, string path, Action action)
        {
            CToolStripButton button = new CToolStripButton(text);
            button.Margin = new Padding(3, 0, 3, 0);
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.Click += delegate { action(); };
            button.IconImage = Resources.GetBitmapFromAssembly(path);
            return button;
        }

        private void CreateColumns()
        {
            _grid.Columns.Add(TextColumn("Title", "件名", 30));
            _grid.Columns.Add(TextColumn("StatusText", "ステータス", 14));
            _grid.Columns.Add(TextColumn("PriorityText", "優先度", 10));
            _grid.Columns.Add(TextColumn("CreatedText", "登録日", 14));
            _grid.Columns.Add(TextColumn("DueText", "期限", 14));
            _grid.Columns.Add(TextColumn("DueState", "期限状態", 18));
            _grid.Columns.Add(TextColumn("Path", "フォルダー", 30));
        }

        private DataGridViewTextBoxColumn TextColumn(string name, string header, float fill)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = header;
            column.DataPropertyName = name;
            column.FillWeight = fill;
            return column;
        }

        private void RefreshGrid()
        {
            if (_grid == null)
                return;

            _grid.Rows.Clear();
            string key = _search == null ? "" : _search.Text.Trim().ToLowerInvariant();
            if (!_filter.Checked)
                key = string.Empty;

            if (_view == "Folder")
            {
                foreach (FolderBookmark folder in _data.Folders)
                {
                    if (Matches(folder.Name, key) || Matches(folder.Path, key) || Matches(folder.Note, key))
                    {
                        int row = _grid.Rows.Add(folder.Name, "", "", "", "", folder.Note, folder.Path);
                        _grid.Rows[row].Tag = folder;
                    }
                }
                _summary.Text = "フォルダー覚書：" + _grid.Rows.Count + "件";
                SetHeadersForFolder();
                return;
            }

            bool completed = _view == "Completed";
            ItemKind kind = _view == "Memo" ? ItemKind.Memo : ItemKind.Todo;

            var items = _data.Items.Where(delegate(WorkItem item)
            {
                if (item.Kind != kind)
                    return false;
                if (kind == ItemKind.Todo && (item.Status == TodoStatus.Completed) != completed)
                    return false;
                return Matches(item.Title, key) || Matches(item.Body, key) || Matches(item.MailUrl, key);
            }).OrderBy(delegate(WorkItem item)
            {
                return item.DueDate.HasValue ? item.DueDate.Value : DateTime.MaxValue;
            });

            foreach (WorkItem item in items)
            {
                string status = item.Kind == ItemKind.Memo ? "メモ" : StatusText(item.Status);
                string priority = item.Kind == ItemKind.Memo ? "" : PriorityText(item.Priority);
                string due = item.DueDate.HasValue ? item.DueDate.Value.ToString("yyyy/MM/dd") : "なし";
                string dueState = item.Kind == ItemKind.Memo ? "" : DueState(item);
                int row = _grid.Rows.Add(item.Title, status, priority,
                    item.CreatedAt.ToString("yyyy/MM/dd"), due, dueState, "");
                _grid.Rows[row].Tag = item;
            }

            _summary.Text = "表示：" + _grid.Rows.Count + "件　　データ保存先：" + DataStore.GetDataFilePath();
            SetHeadersForItems();
        }

        private void SetHeadersForItems()
        {
            _grid.Columns[0].HeaderText = "件名";
            _grid.Columns[1].HeaderText = "ステータス";
            _grid.Columns[2].HeaderText = "優先度";
            _grid.Columns[3].HeaderText = "登録日";
            _grid.Columns[4].HeaderText = "期限";
            _grid.Columns[5].HeaderText = "期限状態";
            _grid.Columns[6].HeaderText = "";
            _grid.Columns[1].Visible = true;
            _grid.Columns[2].Visible = true;
            _grid.Columns[3].Visible = true;
            _grid.Columns[4].Visible = true;
            _grid.Columns[5].Visible = true;
            _grid.Columns[6].Visible = false;
        }

        private void SetHeadersForFolder()
        {
            _grid.Columns[0].HeaderText = "フォルダー名";
            _grid.Columns[1].HeaderText = "";
            _grid.Columns[2].HeaderText = "";
            _grid.Columns[3].HeaderText = "";
            _grid.Columns[4].HeaderText = "";
            _grid.Columns[5].HeaderText = "用途・備考";
            _grid.Columns[6].HeaderText = "ネットワークパス";
            _grid.Columns[1].Visible = false;
            _grid.Columns[2].Visible = false;
            _grid.Columns[3].Visible = false;
            _grid.Columns[4].Visible = false;
            _grid.Columns[1].Visible = false;
            _grid.Columns[6].Visible = true;
        }

        private bool Matches(string value, string key)
        {
            return key.Length == 0 || (value != null && value.ToLowerInvariant().Contains(key));
        }

        private string StatusText(TodoStatus status)
        {
            switch (status)
            {
                case TodoStatus.InProgress: return "進行中";
                case TodoStatus.OnHold: return "保留";
                case TodoStatus.Waiting: return "回答待ち";
                case TodoStatus.Completed: return "完了";
                default: return "新規";
            }
        }

        private string PriorityText(Priority priority)
        {
            switch (priority)
            {
                case Priority.High: return _priorityHigh;
                case Priority.Low: return _priorityLow;
                default: return _priorityMiddle;
            }
        }

        private string DueState(WorkItem item)
        {
            if (!item.DueDate.HasValue || item.Status == TodoStatus.Completed)
                return "";
            int days = (item.DueDate.Value.Date - DateTime.Today).Days;
            if (days < 0) return _dueExceeded;
            if (days <= 7) return days == 0 ? _dueToday : _dueApproaching;
            return "";
        }

        private void GridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
                e.CellStyle.BackColor = Color.LightCyan;
            else
                e.CellStyle.BackColor = Color.White;

            if (_grid.Columns[e.ColumnIndex].Name == "DueState" && e.Value != null)
            {
                string value = e.Value.ToString();
                if (value == _dueExceeded)
                    e.CellStyle.ForeColor = Color.Red;
                else if (value == _dueToday)
                    e.CellStyle.ForeColor = Color.DarkOrange;
                else if (value == _dueApproaching)
                    e.CellStyle.ForeColor = Color.Blue;
            }
        }

        private object SelectedObject()
        {
            if (_grid.SelectedRows.Count == 0)
                return null;
            return _grid.SelectedRows[0].Tag;
        }

        private void AddItem()
        {
            if (_view == "Folder")
            {
                using (FolderEditForm form = new FolderEditForm(null))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        _data.Folders.Add(form.Result);
                        SaveAndRefresh();
                    }
                }
                return;
            }

            ItemKind kind = _view == "Memo" ? ItemKind.Memo : ItemKind.Todo;
            WorkItem item = new WorkItem();
            item.Kind = kind;
            if (kind == ItemKind.Memo)
                item.DueDate = null;

            using (TodoEditForm form = new TodoEditForm(item))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _data.Items.Add(form.Result);
                    SaveAndRefresh();
                }
            }
        }

        private void EditSelected()
        {
            object selected = SelectedObject();
            if (selected == null)
                return;

            FolderBookmark folder = selected as FolderBookmark;
            if (folder != null)
            {
                using (FolderEditForm form = new FolderEditForm(folder))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        ReplaceFolder(folder, form.Result);
                        SaveAndRefresh();
                    }
                }
                return;
            }

            WorkItem item = selected as WorkItem;
            if (item != null)
            {
                using (TodoEditForm form = new TodoEditForm(item))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        ReplaceItem(item, form.Result);
                        SaveAndRefresh();

                        //
                        for (int i = 0; i < _grid.Rows.Count; i++)
                        {
                            WorkItem wi = _grid.Rows[i].Tag as WorkItem;
                            //Console.WriteLine("ID = " + wi.Id);
                            if (item.Id.Equals(wi.Id))
                                _grid.Rows[i].Selected = true;
                        }
                    }
                }
            }
        }

        private void ReplaceItem(WorkItem oldItem, WorkItem newItem)
        {
            int index = _data.Items.IndexOf(oldItem);
            if (index >= 0) _data.Items[index] = newItem;
        }

        private void ReplaceFolder(FolderBookmark oldFolder, FolderBookmark newFolder)
        {
            int index = _data.Folders.IndexOf(oldFolder);
            if (index >= 0) _data.Folders[index] = newFolder;
        }

        private void DeleteSelected()
        {
            object selected = SelectedObject();
            if (selected == null)
                return;

            if (MessageBox.Show("選択した項目を削除しますか？", "削除確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            FolderBookmark folder = selected as FolderBookmark;
            if (folder != null)
                _data.Folders.Remove(folder);
            else
                _data.Items.Remove(selected as WorkItem);

            SaveAndRefresh();
        }

        private void OpenSelected()
        {
            object selected = SelectedObject();
            if (selected == null)
                return;

            FolderBookmark folder = selected as FolderBookmark;
            if (folder != null)
            {
                Task t = OpenPath(folder.Path);
                return;
            }

            WorkItem item = selected as WorkItem;
            if (item != null)
            {
                if (!String.IsNullOrWhiteSpace(item.MailUrl))
                {
                    Task t1 = OpenUrl(item.MailUrl);
                }
                else
                    EditSelected();
            }
        }

        private void OpenOrEditByDoubleClick()
        {
            object selected = SelectedObject();
            FolderBookmark folder = selected as FolderBookmark;
            if (folder != null)
            {
                Task t = OpenPath(folder.Path);
            }
            else
                EditSelected();
        }

        private async Task OpenPath(string path)
        {
            await Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "\"" + path + "\"",
                        UseShellExecute = true,
                    };
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("フォルダーを開けませんでした。\r\n" + ex.Message);
                }
            });
        }

        private async Task OpenUrl(string url)
        {
            await Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo info = new ProcessStartInfo(url);
                    info.UseShellExecute = true;
                    Process.Start(info);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("URLを開けませんでした。\r\n" + ex.Message);
                }
            });
        }

        private void CompleteSelected()
        {
            WorkItem item = SelectedObject() as WorkItem;
            if (item == null || item.Kind != ItemKind.Todo)
                return;

            item.Status = TodoStatus.Completed;
            SaveAndRefresh();
        }

        private void SaveAndRefresh()
        {
            try
            {
                DataStore.Save(_data);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("データを保存できませんでした。\r\n" + ex.Message,
                    "保存エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}