using Parquet.Data;
using Parquet.Schema;

namespace DataView;

public class ParquetFileService : ITabularFileService
{
    private ParquetSchema? _schema;

    public async Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
    {
        var (schema, rows) = await ParquetService.LoadAsync(path);
        _schema = schema;
        return (schema.GetDataFields(), rows);
    }

    public Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows)
    {
        // Rebuild schema from the (possibly edited) fields so type/name changes are persisted
        var schema = new ParquetSchema(fields);
        return ParquetService.SaveAsync(path, schema, rows);
    }
}
