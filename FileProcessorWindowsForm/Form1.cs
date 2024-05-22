namespace FileProcessorWindowsForm
{
    public partial class Form1 : Form
    {
        ImageList imageList1;
        public void ShowDrives()
        {
            treeView1.BeginUpdate();
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                TreeNode tn = new TreeNode(drive);
                treeView1.Nodes.Add(tn);
                AddDirs(tn);
            }
            treeView1.EndUpdate();
        }

        public void ShowFileNames()
        {
            DirectoryInfo di = new DirectoryInfo(treeView1.SelectedNode.FullPath);
            FileInfo[] files = di.GetFiles();
            ListViewItem item;
            imageList1 = new ImageList();

            listView1.Items.Clear();
            listView1.SmallImageList = imageList1;

            if (di.Exists)
                files = di.GetFiles();

            listView1.BeginUpdate();
            foreach (FileInfo file in files)
            {
                Icon iconForFile;
                item = new ListViewItem(file.Name);
                listView1.Items.Add(item);
                iconForFile = SystemIcons.WinLogo;

                if (!imageList1.Images.ContainsKey(file.Extension))
                {
                    iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(file.FullName);
                    imageList1.Images.Add(file.Extension, iconForFile);
                }

                item.ImageKey = file.Extension;
                item.SubItems.Add((file.Length / 1024) + " КБ");
                item.SubItems.Add(file.LastWriteTime.ToString());
                item.SubItems.Add(GetAttributes(file));

            }
            listView1.EndUpdate();
        }

        private string GetAttributes(FileInfo fileInfo)
        {
            string attr = "";

            if ((fileInfo.Attributes & FileAttributes.Hidden) != 0)
                attr += "Спрятанный ";
            if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
                attr += "Только чтение ";
            if ((fileInfo.Attributes & FileAttributes.System) != 0)
                attr += "Системный";
            else if ((fileInfo.Extension == ".dll"))
                attr += "Расширение";
            else if ((fileInfo.Extension == ".exe"))
                attr += "Приложение";
            else attr += "Файл " + fileInfo.Extension;
            return attr;
        }

        public void AddDirs(TreeNode node)
        {
            DirectoryInfo di = new DirectoryInfo(node.FullPath);
            DirectoryInfo[] dirs = { };

            try
            {
                if (di.Exists)
                    dirs = di.GetDirectories();
            }
            catch { return; }

            foreach (DirectoryInfo info in dirs)
            {
                TreeNode treeNode = new TreeNode(info.Name);
                node.Nodes.Add(treeNode);
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowDrives();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowFileNames();
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            treeView1.BeginUpdate();
            foreach (TreeNode node in e.Node.Nodes)
            {
                AddDirs(node);
            }
            treeView1.EndUpdate();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolStripMenuItem[] items = new ToolStripMenuItem[] { new ToolStripMenuItem() { Name = "CopyItem", Text = "Копировать", }, new ToolStripMenuItem() { Name = "RemoveItem", Text = "Удалить" } };
                listView1.ContextMenuStrip = new ContextMenuStrip();
                listView1.ContextMenuStrip.Items.AddRange(items);
                listView1.ContextMenuStrip.ItemClicked += ItemClick;
                listView1.ContextMenuStrip.Show();
            }
        }

        private void ItemClick(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "CopyItem")
                MessageBox.Show("Copy");
            if (e.ClickedItem.Name == "RemoveItem")
                MessageBox.Show("Delete");
        }
        
        private void SelectedItemsDoubleClick(object sender, EventArgs e)
        {
            string currentDir = treeView1.SelectedNode.FullPath;
            string selectedFile = listView1.SelectedItems[0].Text;

            if (File.Exists(Path.Combine(currentDir, selectedFile)))
            {
                try
                {
                    System.Diagnostics.Process.Start(currentDir + @"\" + selectedFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }
        }
    }
}
