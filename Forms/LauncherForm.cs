namespace DataView;

public partial class LauncherForm : Form
{
    public LauncherForm()
    {
        InitializeComponent();
    }

    private void OpenTool<T>(Func<T> factory) where T : Form
    {
        var form = factory();
        form.FormClosed += (_, _) => Show();
        Hide();
        form.Show();
    }

    private void PnlDataView_Click(object sender, EventArgs e)
        => OpenTool(() => new MainForm());

    private void PnlSqlExport_Click(object sender, EventArgs e)
        => OpenTool(() => new SqlExportForm());
}
