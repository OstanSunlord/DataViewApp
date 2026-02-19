namespace DataView;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        toolStrip = new ToolStrip();
        btnOpen = new ToolStripButton();
        btnSave = new ToolStripButton();
        toolStripSeparator1 = new ToolStripSeparator();
        btnAddRow = new ToolStripButton();
        splitContainer = new SplitContainer();
        groupMetadata = new GroupBox();
        metaGrid = new DataGridView();
        groupData = new GroupBox();
        dataGrid = new DataGridView();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        lblRowCount = new ToolStripStatusLabel();
        toolStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        groupMetadata.SuspendLayout();
        groupData.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)metaGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dataGrid).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // toolStrip
        // 
        toolStrip.Items.AddRange(new ToolStripItem[] { btnOpen, btnSave, toolStripSeparator1, btnAddRow });
        toolStrip.Location = new Point(0, 0);
        toolStrip.Name = "toolStrip";
        toolStrip.Size = new Size(1168, 25);
        toolStrip.TabIndex = 0;
        // 
        // btnOpen
        // 
        btnOpen.Name = "btnOpen";
        btnOpen.Size = new Size(55, 22);
        btnOpen.Text = "Open file...";
        btnOpen.ToolTipText = "Open a Parquet file";
        btnOpen.Click += BtnOpen_Click;
        // 
        // btnSave
        // 
        btnSave.Enabled = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(49, 22);
        btnSave.Text = "Save file";
        btnSave.ToolTipText = "Save changes to file";
        btnSave.Click += BtnSave_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(6, 25);
        // 
        // btnAddRow
        // 
        btnAddRow.Enabled = false;
        btnAddRow.Name = "btnAddRow";
        btnAddRow.Size = new Size(72, 22);
        btnAddRow.Text = "Add line";
        btnAddRow.ToolTipText = "Add an empty new row";
        btnAddRow.Click += BtnAddRow_Click;
        // 
        // splitContainer
        // 
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.FixedPanel = FixedPanel.Panel1;
        splitContainer.Location = new Point(0, 25);
        splitContainer.Name = "splitContainer";
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(groupMetadata);
        splitContainer.Panel1MinSize = 220;
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(groupData);
        splitContainer.Panel2MinSize = 400;
        splitContainer.Size = new Size(1168, 590);
        splitContainer.SplitterDistance = 300;
        splitContainer.TabIndex = 1;
        // 
        // groupMetadata
        // 
        groupMetadata.Controls.Add(metaGrid);
        groupMetadata.Dock = DockStyle.Fill;
        groupMetadata.Location = new Point(0, 0);
        groupMetadata.Name = "groupMetadata";
        groupMetadata.Padding = new Padding(5);
        groupMetadata.Size = new Size(300, 590);
        groupMetadata.TabIndex = 0;
        groupMetadata.TabStop = false;
        groupMetadata.Text = "Metadata – columns";
        //
        // metaGrid
        //
        metaGrid.AllowUserToAddRows = false;
        metaGrid.AllowUserToDeleteRows = false;
        metaGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        metaGrid.Dock = DockStyle.Fill;
        metaGrid.Location = new Point(5, 21);
        metaGrid.Name = "metaGrid";
        metaGrid.RowHeadersVisible = false;
        metaGrid.Size = new Size(290, 564);
        metaGrid.TabIndex = 0;
        // 
        // groupData
        // 
        groupData.Controls.Add(dataGrid);
        groupData.Dock = DockStyle.Fill;
        groupData.Location = new Point(0, 0);
        groupData.Name = "groupData";
        groupData.Padding = new Padding(5);
        groupData.Size = new Size(864, 590);
        groupData.TabIndex = 0;
        groupData.TabStop = false;
        groupData.Text = "Data";
        // 
        // dataGrid
        // 
        dataGrid.AllowUserToAddRows = false;
        dataGrid.AllowUserToDeleteRows = false;
        dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        dataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGrid.Dock = DockStyle.Fill;
        dataGrid.Location = new Point(5, 21);
        dataGrid.Name = "dataGrid";
        dataGrid.ReadOnly = true;
        dataGrid.Size = new Size(854, 564);
        dataGrid.TabIndex = 0;
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, lblRowCount });
        statusStrip.Location = new Point(0, 615);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1168, 22);
        statusStrip.TabIndex = 2;
        // 
        // lblStatus
        // 
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(1149, 17);
        lblStatus.Spring = true;
        lblStatus.Text = "No file opened";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblRowCount
        // 
        lblRowCount.BorderSides = ToolStripStatusLabelBorderSides.Left;
        lblRowCount.Name = "lblRowCount";
        lblRowCount.Size = new Size(4, 17);
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1168, 637);
        Controls.Add(splitContainer);
        Controls.Add(toolStrip);
        Controls.Add(statusStrip);
        MinimumSize = new Size(700, 500);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "DataView";
        Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
        toolStrip.ResumeLayout(false);
        toolStrip.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        groupMetadata.ResumeLayout(false);
        groupData.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)metaGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)dataGrid).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ToolStrip toolStrip;
    private ToolStripButton btnOpen;
    private ToolStripButton btnSave;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton btnAddRow;
    private SplitContainer splitContainer;
    private GroupBox groupMetadata;
    private DataGridView metaGrid;
    private GroupBox groupData;
    private DataGridView dataGrid;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripStatusLabel lblRowCount;
}
