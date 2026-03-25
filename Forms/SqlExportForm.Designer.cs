namespace DataView;

partial class SqlExportForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Connection panel ──────────────────────────────────────────────────────
    private Panel    pnlConnect;
    private Label    lblDbType;
    private ComboBox cmbDbType;

    // SQL Server
    private Label    lblServer;
    private TextBox  txtServer;
    private Label    lblDatabase;
    private TextBox  txtDatabase;
    private Label    lblDbHint;
    private Label    lblAuth;
    private ComboBox cmbAuth;
    private Label    lblUser;
    private TextBox  txtUser;
    private Label    lblPassword;
    private TextBox  txtPassword;

    // SQLite
    private Label   lblSqlitePath;
    private TextBox txtSqlitePath;
    private Button  btnBrowseDb;

    // Shared
    private Button btnConnect;

    // ── Object Explorer (left panel) ──────────────────────────────────────────
    private SplitContainer splitMain;
    private Panel          pnlObjHeader;
    private Label          lblObjExplorer;
    private TreeView       tvObjects;

    // ── Preview (right panel) ─────────────────────────────────────────────────
    private Label        lblPreview;
    private DataGridView dataPreview;

    // ── Export bar ────────────────────────────────────────────────────────────
    private Panel    pnlExport;
    private Label    lblFormat;
    private ComboBox cmbFormat;
    private Label    lblExportPath;
    private TextBox  txtExportPath;
    private Button   btnBrowseExport;
    private Button   btnExport;

    // ── Status ────────────────────────────────────────────────────────────────
    private StatusStrip          statusStrip;
    private ToolStripStatusLabel lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlConnect = new Panel();
        lblDbType = new Label();
        cmbDbType = new ComboBox();
        lblServer = new Label();
        txtServer = new TextBox();
        lblDatabase = new Label();
        txtDatabase = new TextBox();
        lblDbHint = new Label();
        lblAuth = new Label();
        cmbAuth = new ComboBox();
        lblUser = new Label();
        txtUser = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        lblSqlitePath = new Label();
        txtSqlitePath = new TextBox();
        btnBrowseDb = new Button();
        btnConnect = new Button();
        splitMain = new SplitContainer();
        tvObjects = new TreeView();
        pnlObjHeader = new Panel();
        lblObjExplorer = new Label();
        dataPreview = new DataGridView();
        lblPreview = new Label();
        pnlExport = new Panel();
        lblFormat = new Label();
        cmbFormat = new ComboBox();
        lblExportPath = new Label();
        txtExportPath = new TextBox();
        btnBrowseExport = new Button();
        btnExport = new Button();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        pnlConnect.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
        splitMain.Panel1.SuspendLayout();
        splitMain.Panel2.SuspendLayout();
        splitMain.SuspendLayout();
        pnlObjHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataPreview).BeginInit();
        pnlExport.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // pnlConnect
        // 
        pnlConnect.BackColor = Color.FromArgb(240, 240, 240);
        pnlConnect.BorderStyle = BorderStyle.FixedSingle;
        pnlConnect.Controls.Add(lblDbType);
        pnlConnect.Controls.Add(cmbDbType);
        pnlConnect.Controls.Add(lblServer);
        pnlConnect.Controls.Add(txtServer);
        pnlConnect.Controls.Add(lblDatabase);
        pnlConnect.Controls.Add(txtDatabase);
        pnlConnect.Controls.Add(lblDbHint);
        pnlConnect.Controls.Add(lblAuth);
        pnlConnect.Controls.Add(cmbAuth);
        pnlConnect.Controls.Add(lblUser);
        pnlConnect.Controls.Add(txtUser);
        pnlConnect.Controls.Add(lblPassword);
        pnlConnect.Controls.Add(txtPassword);
        pnlConnect.Controls.Add(lblSqlitePath);
        pnlConnect.Controls.Add(txtSqlitePath);
        pnlConnect.Controls.Add(btnBrowseDb);
        pnlConnect.Controls.Add(btnConnect);
        pnlConnect.Dock = DockStyle.Top;
        pnlConnect.Location = new Point(0, 0);
        pnlConnect.Name = "pnlConnect";
        pnlConnect.Size = new Size(1020, 84);
        pnlConnect.TabIndex = 3;
        // 
        // lblDbType
        // 
        lblDbType.AutoSize = true;
        lblDbType.Location = new Point(10, 13);
        lblDbType.Name = "lblDbType";
        lblDbType.Size = new Size(35, 15);
        lblDbType.TabIndex = 0;
        lblDbType.Text = "Type:";
        // 
        // cmbDbType
        // 
        cmbDbType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbDbType.Items.AddRange(new object[] { "SQL Server", "SQLite" });
        cmbDbType.Location = new Point(51, 9);
        cmbDbType.Name = "cmbDbType";
        cmbDbType.Size = new Size(110, 23);
        cmbDbType.TabIndex = 1;
        cmbDbType.SelectedIndexChanged += CmbDbType_SelectedIndexChanged;
        // 
        // lblServer
        // 
        lblServer.AutoSize = true;
        lblServer.Location = new Point(168, 13);
        lblServer.Name = "lblServer";
        lblServer.Size = new Size(42, 15);
        lblServer.TabIndex = 2;
        lblServer.Text = "Server:";
        // 
        // txtServer
        // 
        txtServer.Location = new Point(213, 9);
        txtServer.Name = "txtServer";
        txtServer.Size = new Size(180, 23);
        txtServer.TabIndex = 3;
        txtServer.Text = "localhost";
        // 
        // lblDatabase
        // 
        lblDatabase.AutoSize = true;
        lblDatabase.Location = new Point(403, 13);
        lblDatabase.Name = "lblDatabase";
        lblDatabase.Size = new Size(58, 15);
        lblDatabase.TabIndex = 4;
        lblDatabase.Text = "Database:";
        // 
        // txtDatabase
        // 
        txtDatabase.Location = new Point(466, 9);
        txtDatabase.Name = "txtDatabase";
        txtDatabase.Size = new Size(160, 23);
        txtDatabase.TabIndex = 5;
        // 
        // lblDbHint
        // 
        lblDbHint.AutoSize = true;
        lblDbHint.Font = new Font("Segoe UI", 7.5F);
        lblDbHint.ForeColor = SystemColors.GrayText;
        lblDbHint.Location = new Point(467, 31);
        lblDbHint.Name = "lblDbHint";
        lblDbHint.Size = new Size(69, 12);
        lblDbHint.TabIndex = 6;
        lblDbHint.Text = "empty = list all";
        // 
        // lblAuth
        // 
        lblAuth.AutoSize = true;
        lblAuth.Location = new Point(10, 43);
        lblAuth.Name = "lblAuth";
        lblAuth.Size = new Size(89, 15);
        lblAuth.TabIndex = 7;
        lblAuth.Text = "Authentication:";
        // 
        // cmbAuth
        // 
        cmbAuth.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAuth.Items.AddRange(new object[] { "Windows Authentication", "SQL Server Authentication" });
        cmbAuth.Location = new Point(111, 39);
        cmbAuth.Name = "cmbAuth";
        cmbAuth.Size = new Size(190, 23);
        cmbAuth.TabIndex = 8;
        cmbAuth.SelectedIndexChanged += CmbAuth_SelectedIndexChanged;
        // 
        // lblUser
        // 
        lblUser.AutoSize = true;
        lblUser.Location = new Point(315, 43);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(40, 15);
        lblUser.TabIndex = 9;
        lblUser.Text = "Login:";
        lblUser.Visible = false;
        // 
        // txtUser
        // 
        txtUser.Location = new Point(357, 39);
        txtUser.Name = "txtUser";
        txtUser.Size = new Size(150, 23);
        txtUser.TabIndex = 10;
        txtUser.Visible = false;
        // 
        // lblPassword
        // 
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(517, 43);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(60, 15);
        lblPassword.TabIndex = 11;
        lblPassword.Text = "Password:";
        lblPassword.Visible = false;
        // 
        // txtPassword
        // 
        txtPassword.Location = new Point(578, 39);
        txtPassword.Name = "txtPassword";
        txtPassword.PasswordChar = '•';
        txtPassword.Size = new Size(150, 23);
        txtPassword.TabIndex = 12;
        txtPassword.Visible = false;
        // 
        // lblSqlitePath
        // 
        lblSqlitePath.AutoSize = true;
        lblSqlitePath.Location = new Point(168, 13);
        lblSqlitePath.Name = "lblSqlitePath";
        lblSqlitePath.Size = new Size(28, 15);
        lblSqlitePath.TabIndex = 13;
        lblSqlitePath.Text = "File:";
        lblSqlitePath.Visible = false;
        // 
        // txtSqlitePath
        // 
        txtSqlitePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtSqlitePath.Location = new Point(200, 9);
        txtSqlitePath.Name = "txtSqlitePath";
        txtSqlitePath.Size = new Size(1448, 23);
        txtSqlitePath.TabIndex = 14;
        txtSqlitePath.Visible = false;
        // 
        // btnBrowseDb
        // 
        btnBrowseDb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowseDb.Location = new Point(1654, 9);
        btnBrowseDb.Name = "btnBrowseDb";
        btnBrowseDb.Size = new Size(82, 23);
        btnBrowseDb.TabIndex = 15;
        btnBrowseDb.Text = "Browse…";
        btnBrowseDb.Visible = false;
        btnBrowseDb.Click += BtnBrowseDb_Click;
        // 
        // btnConnect
        // 
        btnConnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnConnect.Location = new Point(925, 43);
        btnConnect.Name = "btnConnect";
        btnConnect.Size = new Size(82, 23);
        btnConnect.TabIndex = 16;
        btnConnect.Text = "Connect";
        btnConnect.Click += BtnConnect_Click;
        // 
        // splitMain
        // 
        splitMain.Dock = DockStyle.Fill;
        splitMain.Location = new Point(0, 84);
        splitMain.Name = "splitMain";
        // 
        // splitMain.Panel1
        // 
        splitMain.Panel1.Controls.Add(tvObjects);
        splitMain.Panel1.Controls.Add(pnlObjHeader);
        splitMain.Panel1MinSize = 160;
        // 
        // splitMain.Panel2
        // 
        splitMain.Panel2.Controls.Add(dataPreview);
        splitMain.Panel2.Controls.Add(lblPreview);
        splitMain.Panel2MinSize = 300;
        splitMain.Size = new Size(1020, 576);
        splitMain.SplitterDistance = 240;
        splitMain.TabIndex = 2;
        // 
        // tvObjects
        // 
        tvObjects.Dock = DockStyle.Fill;
        tvObjects.Font = new Font("Segoe UI", 9F);
        tvObjects.FullRowSelect = true;
        tvObjects.HideSelection = false;
        tvObjects.Location = new Point(0, 24);
        tvObjects.Name = "tvObjects";
        tvObjects.Size = new Size(240, 552);
        tvObjects.TabIndex = 0;
        tvObjects.AfterExpand += TvObjects_AfterExpand;
        tvObjects.AfterSelect += TvObjects_AfterSelect;
        // 
        // pnlObjHeader
        // 
        pnlObjHeader.BackColor = Color.FromArgb(0, 122, 204);
        pnlObjHeader.Controls.Add(lblObjExplorer);
        pnlObjHeader.Dock = DockStyle.Top;
        pnlObjHeader.Location = new Point(0, 0);
        pnlObjHeader.Name = "pnlObjHeader";
        pnlObjHeader.Size = new Size(240, 24);
        pnlObjHeader.TabIndex = 1;
        // 
        // lblObjExplorer
        // 
        lblObjExplorer.Dock = DockStyle.Fill;
        lblObjExplorer.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblObjExplorer.ForeColor = Color.White;
        lblObjExplorer.Location = new Point(0, 0);
        lblObjExplorer.Name = "lblObjExplorer";
        lblObjExplorer.Padding = new Padding(6, 0, 0, 0);
        lblObjExplorer.Size = new Size(240, 24);
        lblObjExplorer.TabIndex = 0;
        lblObjExplorer.Text = "Object Explorer";
        lblObjExplorer.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // dataPreview
        // 
        dataPreview.AllowUserToAddRows = false;
        dataPreview.AllowUserToDeleteRows = false;
        dataPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dataPreview.Dock = DockStyle.Fill;
        dataPreview.Location = new Point(0, 22);
        dataPreview.Name = "dataPreview";
        dataPreview.ReadOnly = true;
        dataPreview.RowHeadersVisible = false;
        dataPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataPreview.Size = new Size(776, 554);
        dataPreview.TabIndex = 0;
        // 
        // lblPreview
        // 
        lblPreview.Dock = DockStyle.Top;
        lblPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPreview.Location = new Point(0, 0);
        lblPreview.Name = "lblPreview";
        lblPreview.Padding = new Padding(4, 0, 0, 0);
        lblPreview.Size = new Size(776, 22);
        lblPreview.TabIndex = 1;
        lblPreview.Text = "Preview (up to 200 rows)";
        lblPreview.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlExport
        // 
        pnlExport.BackColor = Color.FromArgb(240, 240, 240);
        pnlExport.Controls.Add(lblFormat);
        pnlExport.Controls.Add(cmbFormat);
        pnlExport.Controls.Add(lblExportPath);
        pnlExport.Controls.Add(txtExportPath);
        pnlExport.Controls.Add(btnBrowseExport);
        pnlExport.Controls.Add(btnExport);
        pnlExport.Dock = DockStyle.Bottom;
        pnlExport.Location = new Point(0, 616);
        pnlExport.Name = "pnlExport";
        pnlExport.Size = new Size(1020, 44);
        pnlExport.TabIndex = 1;
        // 
        // lblFormat
        // 
        lblFormat.AutoSize = true;
        lblFormat.Location = new Point(8, 12);
        lblFormat.Name = "lblFormat";
        lblFormat.Size = new Size(48, 15);
        lblFormat.TabIndex = 0;
        lblFormat.Text = "Format:";
        // 
        // cmbFormat
        // 
        cmbFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFormat.Location = new Point(62, 8);
        cmbFormat.Name = "cmbFormat";
        cmbFormat.Size = new Size(160, 23);
        cmbFormat.TabIndex = 1;
        cmbFormat.SelectedIndexChanged += CmbFormat_SelectedIndexChanged;
        // 
        // lblExportPath
        // 
        lblExportPath.AutoSize = true;
        lblExportPath.Location = new Point(234, 12);
        lblExportPath.Name = "lblExportPath";
        lblExportPath.Size = new Size(57, 15);
        lblExportPath.TabIndex = 2;
        lblExportPath.Text = "Export to:";
        // 
        // txtExportPath
        // 
        txtExportPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtExportPath.Location = new Point(304, 8);
        txtExportPath.Name = "txtExportPath";
        txtExportPath.Size = new Size(1390, 23);
        txtExportPath.TabIndex = 3;
        // 
        // btnBrowseExport
        // 
        btnBrowseExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBrowseExport.Location = new Point(1702, 8);
        btnBrowseExport.Name = "btnBrowseExport";
        btnBrowseExport.Size = new Size(30, 23);
        btnBrowseExport.TabIndex = 4;
        btnBrowseExport.Text = "…";
        btnBrowseExport.Click += BtnBrowseExport_Click;
        // 
        // btnExport
        // 
        btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExport.Enabled = false;
        btnExport.Location = new Point(1740, 8);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(82, 23);
        btnExport.TabIndex = 5;
        btnExport.Text = "Export";
        btnExport.Click += BtnExport_Click;
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Location = new Point(0, 594);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1020, 22);
        statusStrip.TabIndex = 0;
        // 
        // lblStatus
        // 
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(1005, 17);
        lblStatus.Spring = true;
        lblStatus.Text = "Not connected.";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // SqlExportForm
        // 
        ClientSize = new Size(1020, 660);
        Controls.Add(statusStrip);
        Controls.Add(pnlExport);
        Controls.Add(splitMain);
        Controls.Add(pnlConnect);
        MinimumSize = new Size(800, 540);
        Name = "SqlExportForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "SQL Export";
        pnlConnect.ResumeLayout(false);
        pnlConnect.PerformLayout();
        splitMain.Panel1.ResumeLayout(false);
        splitMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
        splitMain.ResumeLayout(false);
        pnlObjHeader.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dataPreview).EndInit();
        pnlExport.ResumeLayout(false);
        pnlExport.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
