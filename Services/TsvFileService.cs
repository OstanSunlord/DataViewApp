using Parquet.Schema;

namespace DataView;

public class TsvFileService : ITabularFileService
{
    public Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
    {
        return Task.Run(() =>
        {
            var lines = File.ReadAllLines(path, System.Text.Encoding.UTF8);
            if (lines.Length == 0)
                return (Array.Empty<DataField>(), new List<Dictionary<string, object?>>());

            var headers = SplitTsv(lines[0]);
            var fields = headers.Select(h => new DataField(h, typeof(string), isNullable: true)).ToArray();

            var rows = new List<Dictionary<string, object?>>();
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]) && i == lines.Length - 1) continue;
                var values = SplitTsv(lines[i]);
                var row    = new Dictionary<string, object?>();
                for (int j = 0; j < headers.Length; j++)
                {
                    var val = j < values.Length ? values[j] : string.Empty;
                    row[headers[j]] = val.Length == 0 ? null : (object?)val;
                }
                rows.Add(row);
            }

            return (fields, rows);
        });
    }

    public Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows)
    {
        return Task.Run(() =>
        {
            using var writer = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            writer.WriteLine(string.Join('\t', fields.Select(f => EscapeField(f.Name))));
            foreach (var row in rows)
            {
                var cells = fields.Select(f =>
                {
                    row.TryGetValue(f.Name, out var v);
                    return EscapeField(v?.ToString() ?? string.Empty);
                });
                writer.WriteLine(string.Join('\t', cells));
            }
        });
    }

    // Simple TSV split — respects RFC 4180-style quoting with tab delimiter.
    private static string[] SplitTsv(string line)
    {
        var fields = new List<string>();
        int pos    = 0;
        while (pos <= line.Length)
        {
            string field;
            if (pos < line.Length && line[pos] == '"')
            {
                pos++; // skip opening quote
                var sb = new System.Text.StringBuilder();
                while (pos < line.Length)
                {
                    if (line[pos] == '"')
                    {
                        pos++;
                        if (pos < line.Length && line[pos] == '"') { sb.Append('"'); pos++; }
                        else break;
                    }
                    else
                    {
                        sb.Append(line[pos++]);
                    }
                }
                field = sb.ToString();
                if (pos < line.Length && line[pos] == '\t') pos++; // skip delimiter
            }
            else
            {
                int end = line.IndexOf('\t', pos);
                if (end < 0) end = line.Length;
                field = line[pos..end];
                pos   = end + 1;
            }
            fields.Add(field);
            if (pos > line.Length) break;
        }
        return fields.ToArray();
    }

    private static string EscapeField(string value)
    {
        if (value.Contains('\t') || value.Contains('\n') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
