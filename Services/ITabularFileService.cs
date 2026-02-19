using Parquet.Schema;

namespace DataView;

public interface ITabularFileService
{
    Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path);
    Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows);
}
