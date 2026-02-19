using Parquet.Schema;

namespace DataView;

public partial class MainForm : Form
{
    private string? _currentFilePath;
    private DataField[]? _fields;
    private ITabularFileService? _fileService;
    private readonly List<Dictionary<string, object?>> _rows = new();

    private ContextMenuStrip _contextMenu = null!;
    private ToolStripMenuItem _mnuEdit   = null!;
    private ToolStripMenuItem _mnuClone  = null!;
    private ToolStripMenuItem _mnuRemove = null!;

    private bool _loadingMeta;

    // Supported types shown in the Type dropdown
    private static readonly (string Label, Type Clr)[] TypeOptions =
    {
        ("string",   typeof(string)),
        ("int",      typeof(int)),
        ("long",     typeof(long)),
        ("float",    typeof(float)),
        ("double",   typeof(double)),
        ("decimal",  typeof(decimal)),
        ("bool",     typeof(bool)),
        ("DateTime", typeof(DateTime)),
        ("DateOnly", typeof(DateOnly)),
        ("Guid",     typeof(Guid)),
    };

    private static string TypeLabel(Type t) =>
        TypeOptions.FirstOrDefault(x => x.Clr == t).Label ?? t.Name;

    public MainForm()
    {
        InitializeComponent();
        BuildMetaGrid();
        BuildContextMenu();
    }

    private void BuildContextMenu()
    {
        _mnuEdit   = new ToolStripMenuItem("Edit line");
        _mnuClone  = new ToolStripMenuItem("Clone line");
        _mnuRemove = new ToolStripMenuItem("Remove line");

        _contextMenu = new ContextMenuStrip();
        _contextMenu.Items.Add(_mnuEdit);
        _contextMenu.Items.Add(_mnuClone);
        _contextMenu.Items.Add(new ToolStripSeparator());
        _contextMenu.Items.Add(_mnuRemove);

        _mnuEdit.Click   += MnuEdit_Click;
        _mnuClone.Click  += MnuClone_Click;
        _mnuRemove.Click += MnuRemove_Click;

        _contextMenu.Opening += (s, e) =>
        {
            bool hasSelection = dataGrid.CurrentCell != null;
            _mnuEdit.Enabled   = hasSelection;
            _mnuClone.Enabled  = hasSelection;
            _mnuRemove.Enabled = hasSelection;
        };

        // Select row on right-click before context menu opens
        dataGrid.MouseDown += (s, e) =>
        {
            if (e.Button != MouseButtons.Right) return;
            var hit = dataGrid.HitTest(e.X, e.Y);
            if (hit.RowIndex >= 0)
                dataGrid.CurrentCell = dataGrid.Rows[hit.RowIndex].Cells[0];
        };

        dataGrid.ContextMenuStrip = _contextMenu;
    }

    // ── Open file ────────────────────────────────────────────────────────────

    private async void BtnOpen_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title = "Select File",
            Filter = "Parquet files (*.parquet)|*.parquet|JSON files (*.json)|*.json|XML files (*.xml)|*.xml|CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            FilterIndex = 5
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        var path = dlg.FileName;
        var ext = Path.GetExtension(path).ToLowerInvariant();

        try
        {
            _fileService = ext switch
            {
                ".parquet" => new ParquetFileService(),
                ".json"    => new JsonFileService(),
                ".xml"     => new XmlFileService(),
                ".csv"     => new CsvFileService(),
                _          => throw new NotSupportedException($"Unsupported file format: {ext}")
            };
        }
        catch (NotSupportedException ex)
        {
            MessageBox.Show(ex.Message, "Unsupported Format", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _currentFilePath = path;
        await LoadFileAsync(_currentFilePath);
    }

    private async Task LoadFileAsync(string path)
    {
        SetStatus($"Loading {Path.GetFileName(path)}…");
        _rows.Clear();
        _fields = null;

        btnOpen.Enabled = false;
        btnSave.Enabled = false;
        btnAddRow.Enabled = false;

        using var loading = new LoadingDialog(Path.GetFileName(path), "Loading");
        var shown = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        loading.Shown += (_, _) => shown.TrySetResult();
        loading.Show(this);
        await shown.Task;   // wait for Shown event
        loading.Refresh();  // force synchronous paint of all controls before handing off

        try
        {
            // Run on thread pool so the UI thread stays free to animate the marquee
            (_fields, var rows) = await Task.Run(async () => await _fileService!.LoadAsync(path));
            _rows.AddRange(rows);

            ShowMetadata();
            ShowData();

            btnSave.Enabled = true;
            btnAddRow.Enabled = true;
            Text = $"DataView – {Path.GetFileName(path)}";
            SetStatus($"Opened: {path}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error loading file:\n{ex.Message}\n\n{ex.GetType().FullName}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Error loading file.");
        }
        finally
        {
            loading.Close();
            btnOpen.Enabled = true;
        }
    }

    // ── Metadata grid setup ───────────────────────────────────────────────────

    private void BuildMetaGrid()
    {
        metaGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        metaGrid.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
        metaGrid.MultiSelect         = false;
        metaGrid.EditMode            = DataGridViewEditMode.EditOnEnter;

        var nameCol = new DataGridViewTextBoxColumn
        {
            Name = "colName", HeaderText = "Column", FillWeight = 48,
        };
        var typeCol = new DataGridViewComboBoxColumn
        {
            Name = "colType", HeaderText = "Type", FillWeight = 37,
            FlatStyle  = FlatStyle.Flat,
            DataSource = TypeOptions.Select(t => t.Label).ToArray(),
        };
        var nullCol = new DataGridViewCheckBoxColumn
        {
            Name = "colNullable", HeaderText = "Nullable", FillWeight = 15,
        };

        metaGrid.Columns.AddRange(nameCol, typeCol, nullCol);

        // Commit ComboBox and CheckBox edits immediately so CellValueChanged fires
        metaGrid.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (metaGrid.CurrentCell is DataGridViewComboBoxCell or DataGridViewCheckBoxCell)
                metaGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };

        metaGrid.CellValueChanged += MetaGrid_CellValueChanged;
    }

    // ── Metadata display ──────────────────────────────────────────────────────

    private void ShowMetadata()
    {
        _loadingMeta = true;
        metaGrid.Rows.Clear();
        if (_fields != null)
            foreach (var f in _fields)
                metaGrid.Rows.Add(f.Name, TypeLabel(f.ClrType), f.IsNullable);
        _loadingMeta = false;
    }

    // ── Metadata edit ─────────────────────────────────────────────────────────

    private void MetaGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_loadingMeta || e.RowIndex < 0 || _fields == null || e.RowIndex >= _fields.Length) return;

        var row        = metaGrid.Rows[e.RowIndex];
        var oldField   = _fields[e.RowIndex];
        var newName    = row.Cells["colName"].Value?.ToString() ?? oldField.Name;
        var typeName   = row.Cells["colType"].Value?.ToString() ?? "string";
        var isNullable = row.Cells["colNullable"].Value is bool b ? b : oldField.IsNullable;

        if (newName.Length == 0) newName = oldField.Name; // reject empty name

        var clrType = TypeOptions.FirstOrDefault(x => x.Label == typeName).Clr ?? typeof(string);

        // If column was renamed, update _rows dictionary keys and data grid header
        if (newName != oldField.Name)
        {
            foreach (var r in _rows)
            {
                if (r.TryGetValue(oldField.Name, out var val))
                {
                    r.Remove(oldField.Name);
                    r[newName] = val;
                }
            }
            if (dataGrid.Columns.Contains(oldField.Name))
            {
                var col = dataGrid.Columns[oldField.Name];
                col.Name       = newName;
                col.HeaderText = newName;
            }
        }

        _fields[e.RowIndex] = new DataField(newName, clrType, isNullable: isNullable);
        SetStatus($"Schema updated – remember to save.");
    }

    // ── Data grid ────────────────────────────────────────────────────────────

    private void ShowData()
    {
        dataGrid.DataSource = null;
        dataGrid.Columns.Clear();

        if (_fields == null || _rows.Count == 0) return;

        foreach (var f in _fields)
            dataGrid.Columns.Add(f.Name, f.Name);

        foreach (var row in _rows)
        {
            var values = _fields.Select(f => row.TryGetValue(f.Name, out var v) ? v : null).ToArray();
            dataGrid.Rows.Add(values);
        }

        UpdateRowCount();
    }

    // ── Add line ─────────────────────────────────────────────────────────────

    private void BtnAddRow_Click(object sender, EventArgs e)
    {
        if (_fields == null) return;

        using var dlg = new AddRowDialog(_fields);
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var newRow = dlg.NewRow;
        _rows.Add(newRow);

        dataGrid.Rows.Add(_fields.Select(f => newRow.TryGetValue(f.Name, out var v) ? v : null).ToArray());

        UpdateRowCount();
        SetStatus("New row added – remember to save.");
    }

    // ── Edit line ─────────────────────────────────────────────────────────────

    private void MnuEdit_Click(object? sender, EventArgs e)
    {
        int rowIndex = dataGrid.CurrentCell?.RowIndex ?? -1;
        if (rowIndex < 0 || _fields == null) return;

        var sourceRow = _rows[rowIndex];
        using var dlg = new AddRowDialog(_fields, sourceRow, DialogMode.Edit);
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _rows[rowIndex] = dlg.UpdatedRow;

        var gridRow = dataGrid.Rows[rowIndex];
        foreach (var f in _fields)
            gridRow.Cells[f.Name].Value = dlg.UpdatedRow.TryGetValue(f.Name, out var v) ? v : null;

        SetStatus("Row updated.");
    }

    // ── Clone line ────────────────────────────────────────────────────────────

    private void MnuClone_Click(object? sender, EventArgs e)
    {
        int rowIndex = dataGrid.CurrentCell?.RowIndex ?? -1;
        if (rowIndex < 0 || _fields == null) return;

        var sourceRow = _rows[rowIndex];
        using var dlg = new AddRowDialog(_fields, sourceRow, DialogMode.Clone);
        if (dlg.ShowDialog() != DialogResult.OK) return;

        var newRow = dlg.NewRow;
        _rows.Add(newRow);

        dataGrid.Rows.Add(_fields.Select(f => newRow.TryGetValue(f.Name, out var v) ? v : null).ToArray());

        UpdateRowCount();
        SetStatus("New row added – remember to save.");
    }

    // ── Remove line ───────────────────────────────────────────────────────────

    private void MnuRemove_Click(object? sender, EventArgs e)
    {
        int rowIndex = dataGrid.CurrentCell?.RowIndex ?? -1;
        if (rowIndex < 0) return;

        var confirm = MessageBox.Show(
            "Are you sure you want to remove this row?",
            "Remove line",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        _rows.RemoveAt(rowIndex);
        dataGrid.Rows.RemoveAt(rowIndex);
        UpdateRowCount();
        SetStatus("Row removed.");
    }

    // ── Save file ─────────────────────────────────────────────────────────────

    private async void BtnSave_Click(object sender, EventArgs e)
    {
        if (_currentFilePath == null || _fields == null) return;

        var filterIndex = Path.GetExtension(_currentFilePath).ToLowerInvariant() switch
        {
            ".parquet" => 1,
            ".json"    => 2,
            ".xml"     => 3,
            ".csv"     => 4,
            _          => 5,
        };

        using var dlg = new SaveFileDialog
        {
            Title       = "Save File",
            Filter      = "Parquet files (*.parquet)|*.parquet|JSON files (*.json)|*.json|XML files (*.xml)|*.xml|CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            FilterIndex = filterIndex,
            FileName    = Path.GetFileName(_currentFilePath)
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        var savePath = dlg.FileName;
        var saveExt  = Path.GetExtension(savePath).ToLowerInvariant();

        // Switch service if the user chose a different format
        if (saveExt != Path.GetExtension(_currentFilePath ?? "").ToLowerInvariant())
        {
            _fileService = saveExt switch
            {
                ".parquet" => new ParquetFileService(),
                ".json"    => new JsonFileService(),
                ".xml"     => new XmlFileService(),
                ".csv"     => new CsvFileService(),
                _          => _fileService,
            };
        }

        await SaveFileAsync(savePath);
    }

    private async Task SaveFileAsync(string path)
    {
        SetStatus($"Saving {Path.GetFileName(path)}…");

        btnOpen.Enabled = false;
        btnSave.Enabled = false;
        btnAddRow.Enabled = false;

        using var loading = new LoadingDialog(Path.GetFileName(path), "Saving");
        var shown = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        loading.Shown += (_, _) => shown.TrySetResult();
        loading.Show(this);
        await shown.Task;   // wait for Shown event
        loading.Refresh();  // force synchronous paint of all controls before handing off

        // Snapshot rows for thread safety before handing off to thread pool
        var rowsSnapshot = _rows.ToList();
        try
        {
            // Run on thread pool so the UI thread stays free to animate the marquee
            await Task.Run(async () => await _fileService!.SaveAsync(path, _fields!, rowsSnapshot));

            _currentFilePath = path;
            Text = $"DataView – {Path.GetFileName(path)}";
            SetStatus($"Saved: {path}");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error saving file:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("Error saving file.");
        }
        finally
        {
            loading.Close();
            btnOpen.Enabled = true;
            btnSave.Enabled = true;
            btnAddRow.Enabled = true;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetStatus(string msg) => lblStatus.Text = msg;

    private void UpdateRowCount() => lblRowCount.Text = $"  {_rows.Count} rows  ";
}
