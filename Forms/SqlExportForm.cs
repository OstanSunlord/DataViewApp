using Parquet.Schema;

namespace DataView;

public partial class SqlExportForm : Form
{
    private const string TagServer   = "server";
    private const string TagDatabase = "database";
    private const string TagTable    = "table";
    private const string TagLoading  = "loading";

    private SqlExportService? _service;
    private string?           _currentDatabase;
    private string            _baseExportDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    private static readonly (string Label, string Filter, string Ext)[] Formats =
    {
        ("CSV (.csv)",         "CSV files (*.csv)|*.csv",               ".csv"),
        ("TSV (.tsv)",         "TSV files (*.tsv)|*.tsv",               ".tsv"),
        ("JSON (.json)",       "JSON files (*.json)|*.json",             ".json"),
        ("Parquet (.parquet)", "Parquet files (*.parquet)|*.parquet",   ".parquet"),
        ("Avro (.avro)",       "Avro files (*.avro)|*.avro",             ".avro"),
        ("ORC (.orc)",         "ORC files (*.orc)|*.orc",               ".orc"),
    };

    public SqlExportForm()
    {
        InitializeComponent();
        cmbDbType.SelectedIndex = 0; // SQL Server
        cmbAuth.SelectedIndex   = 0; // Windows Authentication
        cmbFormat.DataSource    = Formats.Select(f => f.Label).ToArray();
        cmbFormat.SelectedIndex = 0;
    }

    // ── Connection panel visibility ───────────────────────────────────────────

    private void CmbDbType_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool isSqlite = cmbDbType.SelectedIndex == 1;

        lblServer.Visible   = !isSqlite;
        txtServer.Visible   = !isSqlite;
        lblDatabase.Visible = !isSqlite;
        txtDatabase.Visible = !isSqlite;
        lblDbHint.Visible   = !isSqlite;
        lblAuth.Visible     = !isSqlite;
        cmbAuth.Visible     = !isSqlite;

        bool sqlAuth = !isSqlite && cmbAuth.SelectedIndex == 1;
        lblUser.Visible     = sqlAuth;
        txtUser.Visible     = sqlAuth;
        lblPassword.Visible = sqlAuth;
        txtPassword.Visible = sqlAuth;

        lblSqlitePath.Visible = isSqlite;
        txtSqlitePath.Visible = isSqlite;
        btnBrowseDb.Visible   = isSqlite;
    }

    private void CmbAuth_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool sqlAuth        = cmbAuth.SelectedIndex == 1;
        lblUser.Visible     = sqlAuth;
        txtUser.Visible     = sqlAuth;
        lblPassword.Visible = sqlAuth;
        txtPassword.Visible = sqlAuth;
    }

    private string BuildConnectionString(string database = "")
    {
        if (cmbDbType.SelectedIndex == 1)
        {
            return $"Data Source={txtSqlitePath.Text.Trim()}";
        }

        var server = txtServer.Text.Trim();
        var db     = string.IsNullOrWhiteSpace(database) ? txtDatabase.Text.Trim() : database;

        if (cmbAuth.SelectedIndex == 0)
        {
            return $"Server={server};Database={db};Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";
        }
        else
        {
            return $"Server={server};Database={db};User Id={txtUser.Text.Trim()};Password={txtPassword.Text};TrustServerCertificate=True;Encrypt=False;";
        }
    }

    // ── Browse SQLite file ────────────────────────────────────────────────────

    private void BtnBrowseDb_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title  = "Select SQLite database",
            Filter = "SQLite databases (*.db;*.sqlite;*.sqlite3)|*.db;*.sqlite;*.sqlite3|All files (*.*)|*.*"
        };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            txtSqlitePath.Text = dlg.FileName;
        }
    }

    // ── Connect ───────────────────────────────────────────────────────────────

    private async void BtnConnect_Click(object sender, EventArgs e)
    {
        bool isSqlite = cmbDbType.SelectedIndex == 1;

        if (!isSqlite && string.IsNullOrWhiteSpace(txtServer.Text))
        {
            MessageBox.Show("Please enter a server name.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (isSqlite && string.IsNullOrWhiteSpace(txtSqlitePath.Text))
        {
            MessageBox.Show("Please select a SQLite file.", "Connect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _service?.Dispose();
        _service         = new SqlExportService(isSqlite ? DatabaseType.SQLite : DatabaseType.SqlServer);
        _currentDatabase = null;

        tvObjects.Nodes.Clear();
        dataPreview.DataSource = null;
        dataPreview.Columns.Clear();
        btnExport.Enabled = false;

        SetUiBusy(true, "Connecting…");

        try
        {
            bool hasDatabase = !isSqlite && !string.IsNullOrWhiteSpace(txtDatabase.Text);

            if (isSqlite)
            {
                await _service.ConnectAsync(BuildConnectionString());
                var tables     = await _service.GetTablesAsync();
                var serverNode = new TreeNode(System.IO.Path.GetFileName(txtSqlitePath.Text.Trim()))
                {
                    Tag = TagServer
                };
                foreach (var t in tables)
                {
                    serverNode.Nodes.Add(new TreeNode(t) { Tag = TagTable });
                }
                tvObjects.Nodes.Add(serverNode);
                serverNode.Expand();
                _currentDatabase = "";
                SetStatus($"Connected. {tables.Count} table(s).");
            }
            else if (hasDatabase)
            {
                _currentDatabase = txtDatabase.Text.Trim();
                await _service.ConnectAsync(BuildConnectionString());
                var tables     = await _service.GetTablesAsync();
                var serverNode = AddServerNode(txtServer.Text.Trim());
                var dbNode     = new TreeNode(_currentDatabase) { Tag = TagDatabase };
                foreach (var t in tables)
                {
                    dbNode.Nodes.Add(new TreeNode(t) { Tag = TagTable });
                }
                serverNode.Nodes.Add(dbNode);
                serverNode.Expand();
                dbNode.Expand();
                SetStatus($"Connected to {_currentDatabase}. {tables.Count} table(s).");
            }
            else
            {
                // No database specified — connect to master and list databases
                await _service.ConnectAsync(BuildConnectionString("master"));
                var databases  = await _service.GetDatabasesAsync();
                var serverNode = AddServerNode(txtServer.Text.Trim());
                foreach (var db in databases)
                {
                    var dbNode = new TreeNode(db) { Tag = TagDatabase };
                    dbNode.Nodes.Add(new TreeNode("Loading…") { Tag = TagLoading });
                    serverNode.Nodes.Add(dbNode);
                }
                serverNode.Expand();
                SetStatus($"Connected to {txtServer.Text.Trim()}. Expand a database to see its tables.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Connection failed:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _service.Dispose();
            _service = null;
            SetStatus("Connection failed.");
        }
        finally
        {
            SetUiBusy(false);
        }
    }

    private TreeNode AddServerNode(string serverName)
    {
        var node = new TreeNode(serverName) { Tag = TagServer };
        tvObjects.Nodes.Add(node);
        return node;
    }

    // ── TreeView — expand database (lazy load tables) ─────────────────────────

    private async void TvObjects_AfterExpand(object sender, TreeViewEventArgs e)
    {
        var node = e.Node;
        if (node?.Tag as string != TagDatabase) return;
        if (node.Nodes.Count != 1 || node.Nodes[0].Tag as string != TagLoading) return;

        SetUiBusy(true, $"Loading tables for {node.Text}…");

        try
        {
            await _service!.ConnectAsync(BuildConnectionString(node.Text));
            _currentDatabase = node.Text;

            var tables = await _service.GetTablesAsync();
            node.Nodes.Clear();
            foreach (var t in tables)
            {
                node.Nodes.Add(new TreeNode(t) { Tag = TagTable });
            }

            SetStatus($"{node.Text}: {tables.Count} table(s).");
        }
        catch (Exception ex)
        {
            node.Nodes.Clear();
            node.Nodes.Add(new TreeNode($"Error: {ex.Message}") { Tag = TagLoading });
            SetStatus($"Failed to load {node.Text}.");
        }
        finally
        {
            SetUiBusy(false);
        }
    }

    // ── TreeView — select node ────────────────────────────────────────────────

    private async void TvObjects_AfterSelect(object sender, TreeViewEventArgs e)
    {
        var node = e.Node;
        if (node == null || _service == null) return;

        // ── Database node: show info + prepare folder export path ─────────────
        if (node.Tag as string == TagDatabase)
        {
            dataPreview.DataSource = null;
            dataPreview.Columns.Clear();

            bool tablesLoaded = node.Nodes.Count > 0 && node.Nodes[0].Tag as string != TagLoading;
            if (tablesLoaded)
            {
                SetStatus($"{node.Text}: {node.Nodes.Count} table(s). Click Export to export all tables.");
                btnExport.Enabled = true;
            }
            else
            {
                SetStatus($"{node.Text}: expand the node first to load tables.");
                btnExport.Enabled = false;
            }
            txtExportPath.Text = System.IO.Path.Combine(_baseExportDir, node.Text);
            return;
        }

        if (node.Tag as string != TagTable) return;

        var tableName = node.Text;
        var dbName    = node.Parent?.Tag as string == TagDatabase ? node.Parent.Text : null;

        // Reconnect to the correct database if the user clicked into a different one
        if (dbName != null && dbName != _currentDatabase)
        {
            try
            {
                await _service.ConnectAsync(BuildConnectionString(dbName));
                _currentDatabase = dbName;
            }
            catch (Exception ex)
            {
                SetStatus($"Failed to switch to {dbName}: {ex.Message}");
                return;
            }
        }

        SetStatus($"Loading preview for {tableName}…");
        dataPreview.DataSource = null;
        dataPreview.Columns.Clear();
        btnExport.Enabled = false;
        UpdateExportPath(dbName, tableName);

        try
        {
            var (fields, rows) = await Task.Run(() =>
                _service.GetTableDataAsync(tableName, limit: 200));

            PopulatePreview(fields, rows);
            btnExport.Enabled = true;
            SetStatus($"Preview: {rows.Count} rows (max 200), {fields.Length} columns — {tableName}");
        }
        catch (Exception ex)
        {
            SetStatus($"Preview error: {ex.Message}");
        }
    }

    private void UpdateExportPath(string? dbName, string tableName)
    {
        var (_, _, ext) = Formats[cmbFormat.SelectedIndex];
        var fileName    = BuildExportFileName(tableName, ext);
        var dir = string.IsNullOrEmpty(dbName)
            ? _baseExportDir
            : System.IO.Path.Combine(_baseExportDir, dbName);
        txtExportPath.Text = System.IO.Path.Combine(dir, fileName);
    }

    private static string BuildExportFileName(string tableName, string ext)
    {
        if (tableName.Contains('.'))
        {
            var dot = tableName.IndexOf('.');
            return $"{tableName[..dot]}_{tableName[(dot + 1)..]}{ext}";
        }
        return tableName + ext;
    }

    private ITabularFileService CreateFileService(string ext) => ext switch
    {
        ".csv"     => new CsvFileService(),
        ".tsv"     => new TsvFileService(),
        ".json"    => new JsonFileService(),
        ".parquet" => new ParquetFileService(),
        ".avro"    => new AvroFileService(),
        ".orc"     => new OrcFileService(),
        _          => throw new NotSupportedException($"Unsupported format: {ext}")
    };

    private void PopulatePreview(DataField[] fields, List<Dictionary<string, object?>> rows)
    {
        dataPreview.DataSource = null;
        dataPreview.Columns.Clear();

        foreach (var f in fields)
        {
            dataPreview.Columns.Add(f.Name, f.Name);
        }

        foreach (var row in rows)
        {
            var values = fields.Select(f => row.TryGetValue(f.Name, out var v) ? v : null).ToArray();
            dataPreview.Rows.Add(values);
        }
    }

    // ── Browse export path ────────────────────────────────────────────────────

    private void CmbFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        var node = tvObjects.SelectedNode;
        if (node?.Tag as string != TagTable) return;
        var dbName = node.Parent?.Tag as string == TagDatabase ? node.Parent.Text : null;
        UpdateExportPath(dbName, node.Text);
    }

    private void BtnBrowseExport_Click(object sender, EventArgs e)
    {
        // Database node → pick base folder; FolderBrowserDialog
        if (tvObjects.SelectedNode?.Tag as string == TagDatabase)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description  = "Select base export folder (a sub-folder per database will be created inside)",
                SelectedPath = _baseExportDir
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _baseExportDir     = dlg.SelectedPath;
                txtExportPath.Text = System.IO.Path.Combine(_baseExportDir, tvObjects.SelectedNode.Text);
            }
            return;
        }

        // Table or nothing → pick file; SaveFileDialog
        var (_, filter, ext) = Formats[cmbFormat.SelectedIndex];
        var existing = txtExportPath.Text.Trim();
        var initDir  = string.IsNullOrWhiteSpace(existing) ? _baseExportDir
                       : System.IO.Path.GetDirectoryName(existing) ?? _baseExportDir;
        var initFile = string.IsNullOrWhiteSpace(existing) ? "export" + ext
                       : System.IO.Path.GetFileNameWithoutExtension(existing) + ext;

        using var saveDlg = new SaveFileDialog
        {
            Title            = "Export to file",
            Filter           = filter + "|All files (*.*)|*.*",
            InitialDirectory = initDir,
            FileName         = initFile
        };
        if (saveDlg.ShowDialog() == DialogResult.OK)
        {
            txtExportPath.Text = saveDlg.FileName;
        }
    }

    // ── Export ────────────────────────────────────────────────────────────────

    private async void BtnExport_Click(object sender, EventArgs e)
    {
        if (_service == null) return;

        var node = tvObjects.SelectedNode;

        if (node?.Tag as string == TagDatabase)
        {
            await ExportAllTablesAsync(node);
            return;
        }

        if (node?.Tag as string != TagTable)
        {
            MessageBox.Show("Select a table or database first.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var tableName  = node.Text;
        var exportPath = txtExportPath.Text.Trim();
        if (string.IsNullOrEmpty(exportPath))
        {
            BtnBrowseExport_Click(sender, e);
            exportPath = txtExportPath.Text.Trim();
            if (string.IsNullOrEmpty(exportPath)) return;
        }

        var (_, _, ext) = Formats[cmbFormat.SelectedIndex];
        SetUiBusy(true, $"Exporting {tableName}…");

        try
        {
            var fileService    = CreateFileService(ext);
            var (fields, rows) = await Task.Run(() => _service.GetTableDataAsync(tableName));
            await Task.Run(() => fileService.SaveAsync(exportPath, fields, rows));
            SetStatus($"Exported {rows.Count} rows to {exportPath}");
            MessageBox.Show($"Export complete.\n{rows.Count} rows written to:\n{exportPath}",
                "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (NotSupportedException ex)
        {
            MessageBox.Show(ex.Message, "Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            SetStatus("Export not supported.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Export failed:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Export failed.");
        }
        finally
        {
            SetUiBusy(false);
        }
    }

    private async Task ExportAllTablesAsync(TreeNode dbNode)
    {
        var dbName    = dbNode.Text;
        var exportDir = System.IO.Path.Combine(_baseExportDir, dbName);
        var (_, _, ext) = Formats[cmbFormat.SelectedIndex];

        var tables = dbNode.Nodes.Cast<TreeNode>()
            .Where(n => n.Tag as string == TagTable)
            .Select(n => n.Text)
            .ToList();

        if (tables.Count == 0)
        {
            MessageBox.Show("Expand the database node first to load tables.", "Export",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dbName != _currentDatabase)
        {
            try
            {
                await _service!.ConnectAsync(BuildConnectionString(dbName));
                _currentDatabase = dbName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to {dbName}:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        System.IO.Directory.CreateDirectory(exportDir);

        int success = 0, failed = 0;
        SetUiBusy(true, $"Exporting {tables.Count} tables from {dbName}…");

        try
        {
            foreach (var tableName in tables)
            {
                try
                {
                    var filePath    = System.IO.Path.Combine(exportDir, BuildExportFileName(tableName, ext));
                    var fileService = CreateFileService(ext);
                    var (fields, rows) = await Task.Run(() => _service!.GetTableDataAsync(tableName));
                    await Task.Run(() => fileService.SaveAsync(filePath, fields, rows));
                    success++;
                    SetStatus($"Exporting {dbName}: {success}/{tables.Count}…");
                }
                catch
                {
                    failed++;
                }
            }

            var summary = failed > 0 ? $"{success} exported, {failed} failed." : $"All {success} tables exported.";
            SetStatus($"{summary} → {exportDir}");
            MessageBox.Show($"{summary}\n\nOutput folder:\n{exportDir}", "Export Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        finally
        {
            SetUiBusy(false);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetStatus(string msg) => lblStatus.Text = msg;

    private void SetUiBusy(bool busy, string? statusMsg = null)
    {
        pnlConnect.Enabled = !busy;
        btnExport.Enabled  = !busy && _service != null && IsExportableNodeSelected();
        if (busy && statusMsg != null)
        {
            SetStatus(statusMsg);
        }
    }

    private bool IsExportableNodeSelected()
    {
        var node = tvObjects.SelectedNode;
        if (node?.Tag as string == TagTable) return true;
        if (node?.Tag as string == TagDatabase)
        {
            return node.Nodes.Count > 0 && node.Nodes[0].Tag as string != TagLoading;
        }
        return false;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        FixAnchoredControls();
    }

    private void FixAnchoredControls()
    {
        const int margin = 10;

        btnExport.Location       = new Point(pnlExport.ClientSize.Width - margin - btnExport.Width, btnExport.Top);
        btnBrowseExport.Location = new Point(btnExport.Left - 5 - btnBrowseExport.Width, btnBrowseExport.Top);
        txtExportPath.Width      = btnBrowseExport.Left - margin - txtExportPath.Left;

        btnConnect.Location  = new Point(pnlConnect.ClientSize.Width - margin - btnConnect.Width, btnConnect.Top);
        btnBrowseDb.Location = new Point(btnConnect.Left - 5 - btnBrowseDb.Width, btnBrowseDb.Top);
        txtSqlitePath.Width  = btnBrowseDb.Left - margin - txtSqlitePath.Left;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _service?.Dispose();
        base.OnFormClosed(e);
    }
}
