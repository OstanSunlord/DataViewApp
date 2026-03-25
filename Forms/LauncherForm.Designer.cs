namespace DataView;

partial class LauncherForm
{
    private System.ComponentModel.IContainer components = null;

    private Label lblTitle;
    private Label lblSubtitle;
    private TableLayoutPanel tblTiles;
    private Panel pnlDataView;
    private Label lblDataViewTitle;
    private Label lblDataViewDesc;
    private Panel pnlSqlExport;
    private Label lblSqlExportTitle;
    private Label lblSqlExportDesc;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle       = new Label();
        lblSubtitle    = new Label();
        tblTiles       = new TableLayoutPanel();
        pnlDataView    = new Panel();
        lblDataViewTitle  = new Label();
        lblDataViewDesc   = new Label();
        pnlSqlExport   = new Panel();
        lblSqlExportTitle = new Label();
        lblSqlExportDesc  = new Label();
        tblTiles.SuspendLayout();
        pnlDataView.SuspendLayout();
        pnlSqlExport.SuspendLayout();
        SuspendLayout();

        // ── Form ──────────────────────────────────────────────────────────────
        Text            = "DataView Tools";
        ClientSize      = new Size(560, 320);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.White;

        // ── lblTitle ──────────────────────────────────────────────────────────
        lblTitle.Text      = "DataView Tools";
        lblTitle.Font      = new Font("Segoe UI", 18f, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 30, 30);
        lblTitle.AutoSize  = true;
        lblTitle.Location  = new Point(24, 20);

        // ── lblSubtitle ───────────────────────────────────────────────────────
        lblSubtitle.Text      = "Select a tool to get started";
        lblSubtitle.Font      = new Font("Segoe UI", 10f);
        lblSubtitle.ForeColor = Color.FromArgb(100, 100, 100);
        lblSubtitle.AutoSize  = true;
        lblSubtitle.Location  = new Point(26, 56);

        // ── tblTiles ──────────────────────────────────────────────────────────
        tblTiles.Location    = new Point(24, 88);
        tblTiles.Size        = new Size(512, 196);
        tblTiles.ColumnCount = 2;
        tblTiles.RowCount    = 1;
        tblTiles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        tblTiles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        tblTiles.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        tblTiles.Padding     = new Padding(0);
        tblTiles.Controls.Add(pnlDataView,  0, 0);
        tblTiles.Controls.Add(pnlSqlExport, 1, 0);

        // ── pnlDataView ───────────────────────────────────────────────────────
        pnlDataView.Dock        = DockStyle.Fill;
        pnlDataView.Margin      = new Padding(0, 0, 8, 0);
        pnlDataView.BackColor   = Color.FromArgb(0, 120, 212);
        pnlDataView.Cursor      = Cursors.Hand;
        pnlDataView.Controls.Add(lblDataViewTitle);
        pnlDataView.Controls.Add(lblDataViewDesc);
        pnlDataView.Click      += PnlDataView_Click;

        lblDataViewTitle.Text      = "DataView";
        lblDataViewTitle.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblDataViewTitle.ForeColor = Color.White;
        lblDataViewTitle.AutoSize  = true;
        lblDataViewTitle.Location  = new Point(18, 18);
        lblDataViewTitle.Click    += PnlDataView_Click;

        lblDataViewDesc.Text      = "Open and edit tabular files\n(Parquet, JSON, CSV, XML)";
        lblDataViewDesc.Font      = new Font("Segoe UI", 9.5f);
        lblDataViewDesc.ForeColor = Color.FromArgb(220, 235, 255);
        lblDataViewDesc.AutoSize  = true;
        lblDataViewDesc.Location  = new Point(18, 52);
        lblDataViewDesc.Click    += PnlDataView_Click;

        // ── pnlSqlExport ──────────────────────────────────────────────────────
        pnlSqlExport.Dock        = DockStyle.Fill;
        pnlSqlExport.Margin      = new Padding(8, 0, 0, 0);
        pnlSqlExport.BackColor   = Color.FromArgb(16, 124, 16);
        pnlSqlExport.Cursor      = Cursors.Hand;
        pnlSqlExport.Controls.Add(lblSqlExportTitle);
        pnlSqlExport.Controls.Add(lblSqlExportDesc);
        pnlSqlExport.Click      += PnlSqlExport_Click;

        lblSqlExportTitle.Text      = "SQL Export";
        lblSqlExportTitle.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblSqlExportTitle.ForeColor = Color.White;
        lblSqlExportTitle.AutoSize  = true;
        lblSqlExportTitle.Location  = new Point(18, 18);
        lblSqlExportTitle.Click    += PnlSqlExport_Click;

        lblSqlExportDesc.Text      = "Connect to a local database\nand export tables to a file";
        lblSqlExportDesc.Font      = new Font("Segoe UI", 9.5f);
        lblSqlExportDesc.ForeColor = Color.FromArgb(200, 240, 200);
        lblSqlExportDesc.AutoSize  = true;
        lblSqlExportDesc.Location  = new Point(18, 52);
        lblSqlExportDesc.Click    += PnlSqlExport_Click;

        // ── Hover effects ─────────────────────────────────────────────────────
        AddHover(pnlDataView,  Color.FromArgb(0, 100, 190), Color.FromArgb(0, 120, 212));
        AddHover(pnlSqlExport, Color.FromArgb(10, 105, 10), Color.FromArgb(16, 124, 16));

        // ── Layout ────────────────────────────────────────────────────────────
        tblTiles.ResumeLayout(false);
        pnlDataView.ResumeLayout(false);
        pnlDataView.PerformLayout();
        pnlSqlExport.ResumeLayout(false);
        pnlSqlExport.PerformLayout();

        Controls.Add(lblTitle);
        Controls.Add(lblSubtitle);
        Controls.Add(tblTiles);

        ResumeLayout(false);
        PerformLayout();
    }

    private static void AddHover(Panel panel, Color hoverColor, Color normalColor)
    {
        panel.MouseEnter += (_, _) => panel.BackColor = hoverColor;
        panel.MouseLeave += (_, _) => panel.BackColor = normalColor;
        foreach (Control c in panel.Controls)
        {
            c.MouseEnter += (_, _) => panel.BackColor = hoverColor;
            c.MouseLeave += (_, _) => panel.BackColor = normalColor;
        }
    }
}
