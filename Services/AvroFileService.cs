using Avro;
using Avro.File;
using Avro.Generic;
using Parquet.Schema;

namespace DataView;

public class AvroFileService : ITabularFileService
{
    public Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
    {
        return Task.Run(() =>
        {
            using var fileStream = File.OpenRead(path);
            using var reader     = DataFileReader<GenericRecord>.OpenReader(fileStream);

            var records    = new List<GenericRecord>();
            RecordSchema?  avroSchema = null;
            while (reader.HasNext())
            {
                var record = reader.Next();
                avroSchema ??= (RecordSchema)record.Schema;
                records.Add(record);
            }

            if (avroSchema == null)
                return (Array.Empty<DataField>(), new List<Dictionary<string, object?>>());

            var fields = avroSchema.Fields
                .Select(f => new DataField(f.Name, typeof(string), isNullable: true))
                .ToArray();

            var rows = records.Select(record =>
            {
                var row = new Dictionary<string, object?>();
                foreach (var f in fields)
                {
                    record.TryGetValue(f.Name, out var val);
                    row[f.Name] = val?.ToString();
                }
                return row;
            }).ToList();

            return (fields, rows);
        });
    }

    public Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows)
    {
        return Task.Run(() =>
        {
            var avroFields = fields.Select(f => new
            {
                name    = f.Name,
                type    = new object[] { "null", ClrTypeToAvro(f.ClrType) },
                @default = (object?)null
            }).ToArray<object>();

            var schemaJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                type   = "record",
                name   = "Row",
                fields = avroFields
            });

            var schema      = (RecordSchema)Schema.Parse(schemaJson);
            var datumWriter = new GenericDatumWriter<GenericRecord>(schema);

            using var fileStream = File.Create(path);
            using var fileWriter = DataFileWriter<GenericRecord>.OpenWriter(datumWriter, fileStream);

            foreach (var row in rows)
            {
                var record = new GenericRecord(schema);
                foreach (var f in fields)
                {
                    row.TryGetValue(f.Name, out var val);
                    record.Add(f.Name, ConvertValue(val, f.ClrType));
                }
                fileWriter.Append(record);
            }

            fileWriter.Flush();
        });
    }

    private static string ClrTypeToAvro(Type t)
    {
        if (t == typeof(int))    return "int";
        if (t == typeof(long))   return "long";
        if (t == typeof(float))  return "float";
        if (t == typeof(double)) return "double";
        if (t == typeof(bool))   return "boolean";
        return "string";
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null) return null;
        var str = value.ToString();
        if (string.IsNullOrEmpty(str)) return null;

        try
        {
            if (targetType == typeof(int)    && int.TryParse(str,    out var i)) return i;
            if (targetType == typeof(long)   && long.TryParse(str,   out var l)) return l;
            if (targetType == typeof(float)  && float.TryParse(str,  out var f)) return f;
            if (targetType == typeof(double) && double.TryParse(str, out var d)) return d;
            if (targetType == typeof(bool)   && bool.TryParse(str,   out var b)) return b;
        }
        catch { }

        return str;
    }
}
