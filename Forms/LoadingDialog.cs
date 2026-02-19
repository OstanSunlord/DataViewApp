namespace DataView;

/// <summary>
/// Modeless dialog shown during file load / save.
/// Use with: loadingDlg.Show(owner) … loadingDlg.Close()
/// </summary>
public class LoadingDialog : Form
{
    public LoadingDialog(string fileName, string verb = "Loading")
    {
        FormBorderStyle = FormBorderStyle.FixedSingle;
        ControlBox = false;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.Manual;
        ClientSize = new Size(380, 104);
        Text = verb + "…";

        var lbl = new Label
        {
            Text = $"{verb}\n{fileName}",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Bounds = new Rectangle(16, 12, 348, 52),
            Font = new Font(Font.FontFamily, 9.5f),
        };

        var bar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 22,
            Bounds = new Rectangle(16, 72, 348, 18),
        };

        Controls.Add(lbl);
        Controls.Add(bar);
    }

    protected override void OnShown(EventArgs e)
    {
        // Center over the owner now that Width/Height include the window frame
        if (Owner != null)
            Location = new Point(
                Owner.Left + (Owner.Width  - Width)  / 2,
                Owner.Top  + (Owner.Height - Height) / 2);

        base.OnShown(e); // raises the Shown event (TaskCompletionSource in MainForm)
    }
}
