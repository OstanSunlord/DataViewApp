using Parquet;
using Parquet.Data;
using Parquet.Schema;

namespace DataView;

/// <summary>
/// Håndterer al Parquet fil I/O uafhængigt af UI-laget.
/// </summary>
internal static class ParquetService
{
    public static async Task<(ParquetSchema Schema, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
    {
        var fileBytes = await File.ReadAllBytesAsync(path);
        using var ms = new MemoryStream(fileBytes);
        using var reader = await ParquetReader.CreateAsync(ms);

        var schema = reader.Schema;
        var rows = new List<Dictionary<string, object?>>();

        for (int rg = 0; rg < reader.RowGroupCount; rg++)
        {
            using var rgReader = reader.OpenRowGroupReader(rg);

            var columns = new Dictionary<string, Array>();
            foreach (var field in schema.GetDataFields())
            {
                var col = await rgReader.ReadColumnAsync(field);
                columns[field.Name] = col.Data;
            }

            int rowCount = columns.Values.FirstOrDefault()?.Length ?? 0;
            for (int i = 0; i < rowCount; i++)
            {
                var row = new Dictionary<string, object?>();
                foreach (var (name, arr) in columns)
                    row[name] = i < arr.Length ? arr.GetValue(i) : null;
                rows.Add(row);
            }
        }

        return (schema, rows);
    }

    public static async Task SaveAsync(
        string path,
        ParquetSchema schema,
        IReadOnlyList<Dictionary<string, object?>> rows)
    {
        var fields = schema.GetDataFields();
        var dataColumns = fields.Select(f => new DataColumn(f, BuildColumnArray(f, rows))).ToList();

        using var ms = new MemoryStream();
        using (var writer = await ParquetWriter.CreateAsync(schema, ms))
        {
            using var rgWriter = writer.CreateRowGroup();
            foreach (var col in dataColumns)
                await rgWriter.WriteColumnAsync(col);
        }

        await File.WriteAllBytesAsync(path, ms.ToArray());
    }

    private static Array BuildColumnArray(DataField field, IReadOnlyList<Dictionary<string, object?>> rows)
    {
        Type baseType = field.ClrType;

        if (field.IsNullable && baseType.IsValueType)
        {
            var nullableType = typeof(Nullable<>).MakeGenericType(baseType);
            var list = CreateList(nullableType);
            foreach (var row in rows)
            {
                row.TryGetValue(field.Name, out var v);
                list.Add(v == null ? null : Convert.ChangeType(v, baseType));
            }
            return ToArray(list, nullableType);
        }
        else
        {
            var list = CreateList(baseType);
            foreach (var row in rows)
            {
                row.TryGetValue(field.Name, out var v);
                list.Add(v == null ? DefaultValue(baseType) : Convert.ChangeType(v, baseType));
            }
            return ToArray(list, baseType);
        }
    }

    private static System.Collections.IList CreateList(Type t) =>
        (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(t))!;

    private static Array ToArray(System.Collections.IList list, Type elementType)
    {
        var arr = Array.CreateInstance(elementType, list.Count);
        list.CopyTo(arr, 0);
        return arr;
    }

    private static object DefaultValue(Type t) =>
        t == typeof(string) ? "" :
        t == typeof(bool)   ? false :
        t == typeof(DateTime) || t == typeof(DateTimeOffset) ? DateTime.MinValue :
        Convert.ChangeType(0, t);
}
