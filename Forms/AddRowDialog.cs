using Parquet.Data;
using Parquet.Schema;

namespace DataView;

public enum DialogMode { Add, Clone, Edit }

/// <summary>
/// Dialog som bygger inputfelter ud fra skemaet.
/// Add-mode: alle felter tomme.
/// Clone/Edit-mode: felter præudfyldt fra sourceRow.
/// Edit-mode bruger "Save"-knap og returnerer UpdatedRow.
/// </summary>
public class AddRowDialog : Form
{
    private readonly DataField[] _fields;
    private readonly Dictionary<string, object?>? _sourceRow;
    private readonly DialogMode _mode;
    private readonly Dictionary<string, Control> _inputs = new();

    public Dictionary<string, object?> NewRow { get; } = new();
    public Dictionary<string, object?> UpdatedRow => NewRow;

    public AddRowDialog(IEnumerable<DataField> fields, Dictionary<string, object?>? sourceRow = null, DialogMode mode = DialogMode.Add)
    {
        _fields = fields.ToArray();
        _sourceRow = sourceRow;
        _mode = mode;
        BuildUI();
    }

    // ── UI ───────────────────────────────────────────────────────────────────

    private void BuildUI()
    {
        Text = _mode switch
        {
            DialogMode.Edit  => "Edit row",
            DialogMode.Clone => "Clone row",
            _                => "Add new row"
        };
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        const int labelWidth = 160;
        const int inputWidth = 230;
        const int rowHeight = 30;
        const int margin = 10;
        int y = margin;

        var panel = new Panel { AutoScroll = true };

        foreach (var field in _fields)
        {
            var label = new Label
            {
                Text = $"{field.Name} ({field.ClrType.Name}{(field.IsNullable ? "?" : "")})",
                Location = new Point(margin, y + 3),
                Width = labelWidth,
                AutoEllipsis = true
            };

            var suggested = SuggestValue(field);
            var input = CreateInput(field, suggested);
            input.Location = new Point(margin + labelWidth + 5, y);
            input.Width = inputWidth;

            panel.Controls.Add(label);
            panel.Controls.Add(input);
            _inputs[field.Name] = input;

            y += rowHeight + 4;
        }

        var btnOk = new Button
        {
            Text = _mode == DialogMode.Edit ? "Save" : "Add",
            DialogResult = DialogResult.OK,
            Location = new Point(margin, y + 6),
            Width = 90
        };
        btnOk.Click += BtnOk_Click;

        var btnCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(margin + 100, y + 6),
            Width = 90
        };

        panel.Controls.Add(btnOk);
        panel.Controls.Add(btnCancel);

        ClientSize = new Size(labelWidth + inputWidth + margin * 3 + 20, Math.Min(y + 80, 600));
        panel.Dock = DockStyle.Fill;
        Controls.Add(panel);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    // ── Forslag til startværdi ────────────────────────────────────────────────

    private object? SuggestValue(DataField field)
    {
        if (_sourceRow == null) return null;
        if (!_sourceRow.TryGetValue(field.Name, out var raw) || raw is null) return null;

        // DateTime/DateTimeOffset: hvis værdien har midnight-tid → returner DateOnly
        // så CreateInput kan vise en simpel datovælger uden tidsdel
        if (field.ClrType == typeof(DateTime) || field.ClrType == typeof(DateTimeOffset))
        {
            bool isMidnight = raw switch
            {
                DateTime dt        => dt.TimeOfDay == TimeSpan.Zero,
                DateTimeOffset dto => dto.TimeOfDay == TimeSpan.Zero,
                _                  => true
            };

            if (isMidnight)
                return raw is DateTimeOffset dtoRaw
                    ? DateOnly.FromDateTime(dtoRaw.DateTime)
                    : DateOnly.FromDateTime((DateTime)raw);

            return raw;
        }

        return raw;
    }

    // ── Opret inputfelt med foreslået værdi ──────────────────────────────────

    private Control CreateInput(DataField field, object? suggested)
    {
        Type t = field.ClrType;

        if (t == typeof(bool))
        {
            var cb = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                SelectedIndex = 0
            };
            cb.Items.AddRange(new object[] { "true", "false" });
            if (suggested is bool b)
                cb.SelectedIndex = b ? 0 : 1;
            return cb;
        }

        if (t == typeof(DateOnly))
        {
            var dtp = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            if (suggested is DateOnly d)
                dtp.Value = d.ToDateTime(TimeOnly.MinValue);
            return dtp;
        }

        if (t == typeof(DateTime) || t == typeof(DateTimeOffset))
        {
            var dtp = new DateTimePicker { Value = DateTime.Now };

            // SuggestValue returnerer DateOnly når værdien kun har dato (ingen tidsdel)
            if (suggested is DateOnly dateOnlySuggestion)
            {
                dtp.Format = DateTimePickerFormat.Short;
                dtp.Value = dateOnlySuggestion.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                if (suggested is DateTime dt)             dtp.Value = dt;
                else if (suggested is DateTimeOffset dto) dtp.Value = dto.LocalDateTime;
            }
            return dtp;
        }

        // TextBox til alt andet (strings, tal, Guid, …)
        string text = suggested is not null ? FormatSuggested(suggested) : "";
        return new TextBox { Text = text };
    }

    private static string FormatSuggested(object value) => value switch
    {
        DateTime dt        => dt.ToString("yyyy-MM-dd HH:mm:ss"),
        DateTimeOffset dto => dto.ToString("yyyy-MM-dd HH:mm:ss"),
        _                  => value.ToString() ?? ""
    };

    // ── Parse inputværdier ved OK ─────────────────────────────────────────────

    private void BtnOk_Click(object? sender, EventArgs e)
    {
        foreach (var field in _fields)
            NewRow[field.Name] = ParseValue(field, _inputs[field.Name]);
    }

    private static object? ParseValue(DataField field, Control ctrl)
    {
        Type t = field.ClrType;

        if (ctrl is ComboBox cb)
            return bool.Parse(cb.SelectedItem?.ToString() ?? "false");

        if (ctrl is DateTimePicker dtp)
        {
            if (t == typeof(DateOnly))       return DateOnly.FromDateTime(dtp.Value);
            if (t == typeof(DateTimeOffset)) return new DateTimeOffset(dtp.Value);
            return dtp.Value;
        }

        // TextBox
        string text = ((TextBox)ctrl).Text.Trim();

        if (string.IsNullOrEmpty(text))
            return field.IsNullable ? null : GetDefault(t);

        try
        {
            if (t == typeof(string))  return text;
            if (t == typeof(int))     return int.Parse(text);
            if (t == typeof(long))    return long.Parse(text);
            if (t == typeof(short))   return short.Parse(text);
            if (t == typeof(byte))    return byte.Parse(text);
            if (t == typeof(sbyte))   return sbyte.Parse(text);
            if (t == typeof(ushort))  return ushort.Parse(text);
            if (t == typeof(uint))    return uint.Parse(text);
            if (t == typeof(ulong))   return ulong.Parse(text);
            if (t == typeof(float))   return float.Parse(text);
            if (t == typeof(double))  return double.Parse(text);
            if (t == typeof(decimal)) return decimal.Parse(text);
            if (t == typeof(Guid))    return Guid.Parse(text);
            return Convert.ChangeType(text, t);
        }
        catch
        {
            return GetDefault(t);
        }
    }

    private static object GetDefault(Type t) =>
        t == typeof(string)                                    ? "" :
        t == typeof(bool)                                      ? false :
        t == typeof(DateTime) || t == typeof(DateTimeOffset)   ? DateTime.MinValue :
        Convert.ChangeType(0, t);
}
