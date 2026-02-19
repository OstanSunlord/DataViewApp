using System.Text.Json;
using Parquet.Schema;

namespace DataView;

public class JsonFileService : ITabularFileService
{
    public async Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("JSON file must contain a top-level array of objects.");

        // Collect ordered union of all property names
        var fieldNames = new List<string>();
        var fieldSet = new HashSet<string>(StringComparer.Ordinal);

        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object) continue;
            foreach (var prop in element.EnumerateObject())
            {
                if (fieldSet.Add(prop.Name))
                    fieldNames.Add(prop.Name);
            }
        }

        var fields = fieldNames
            .Select(n => new DataField(n, typeof(string), isNullable: true))
            .ToArray();

        var rows = new List<Dictionary<string, object?>>();
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (element.ValueKind != JsonValueKind.Object) continue;
            var row = new Dictionary<string, object?>();
            foreach (var name in fieldNames)
            {
                if (element.TryGetProperty(name, out var prop))
                    row[name] = prop.ValueKind == JsonValueKind.Null ? null : prop.ToString();
                else
                    row[name] = null;
            }
            rows.Add(row);
        }

        return (fields, rows);
    }

    public async Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows)
    {
        // Serialize as array of objects with fields in schema order
        var ordered = rows.Select(row =>
        {
            var dict = new Dictionary<string, object?>();
            foreach (var f in fields)
                dict[f.Name] = row.TryGetValue(f.Name, out var v) ? v : null;
            return dict;
        }).ToList();

        var json = JsonSerializer.Serialize(ordered, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }
}
