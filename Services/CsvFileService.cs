using System.Text;
using Parquet.Schema;

namespace DataView;

public class CsvFileService : ITabularFileService
{
    public async Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
    {
        var content = await File.ReadAllTextAsync(path);
        var lines = ParseCsv(content);

        if (lines.Count == 0)
            return (Array.Empty<DataField>(), new List<Dictionary<string, object?>>());

        var headers = lines[0];
        var fields = headers
            .Select(h => new DataField(h, typeof(string), isNullable: true))
            .ToArray();

        var rows = new List<Dictionary<string, object?>>();
        for (int i = 1; i < lines.Count; i++)
        {
            var line = lines[i];
            // Skip empty trailing line that results from a final newline
            if (line.Length == 1 && line[0] == "") continue;

            var row = new Dictionary<string, object?>();
            for (int j = 0; j < headers.Length; j++)
            {
                var val = j < line.Length ? line[j] : null;
                row[headers[j]] = string.IsNullOrEmpty(val) ? null : val;
            }
            rows.Add(row);
        }

        return (fields, rows);
    }

    public async Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows)
    {
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", fields.Select(f => Escape(f.Name))));

        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(",", fields.Select(f =>
                Escape(row.TryGetValue(f.Name, out var v) ? v?.ToString() : null))));
        }

        // UTF-8 with BOM so Excel opens it correctly without import wizard
        await File.WriteAllTextAsync(path, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    // ── RFC 4180 parser ───────────────────────────────────────────────────────

    private static List<string[]> ParseCsv(string text)
    {
        var result = new List<string[]>();
        int pos = 0;
        while (pos < text.Length)
            result.Add(ParseRow(text, ref pos));
        return result;
    }

    private static string[] ParseRow(string text, ref int pos)
    {
        var fields = new List<string>();
        while (true)
        {
            fields.Add(ParseField(text, ref pos));

            if (pos >= text.Length) break;

            if (text[pos] == ',')
            {
                pos++;          // skip separator, read next field
            }
            else
            {
                if (pos < text.Length && text[pos] == '\r') pos++;  // CR
                if (pos < text.Length && text[pos] == '\n') pos++;  // LF
                break;
            }
        }
        return fields.ToArray();
    }

    private static string ParseField(string text, ref int pos)
    {
        if (pos >= text.Length) return "";

        if (text[pos] == '"')
        {
            pos++; // opening quote
            var sb = new StringBuilder();
            while (pos < text.Length)
            {
                if (text[pos] == '"')
                {
                    pos++;
                    if (pos < text.Length && text[pos] == '"')
                    {
                        sb.Append('"'); // escaped quote ""
                        pos++;
                    }
                    else break; // closing quote
                }
                else sb.Append(text[pos++]);
            }
            return sb.ToString();
        }
        else
        {
            int start = pos;
            while (pos < text.Length && text[pos] != ',' && text[pos] != '\r' && text[pos] != '\n')
                pos++;
            return text[start..pos];
        }
    }

    private static string Escape(string? value)
    {
        if (value == null) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
