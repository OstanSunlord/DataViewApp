using Parquet.Schema;

namespace DataView;

/// <summary>
/// ORC export placeholder. Writing ORC files is not natively supported in .NET
/// without a native C++ bridge. Use Parquet as an alternative columnar format.
/// </summary>
public class OrcFileService : ITabularFileService
{
    public Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
        => throw new NotSupportedException(
            "ORC file reading is not supported in this version.\n" +
            "Tip: Parquet is a widely supported alternative columnar format.");

    public Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows)
        => throw new NotSupportedException(
            "ORC file writing is not supported in this version.\n" +
            "Tip: Parquet is a widely supported alternative columnar format.");
}
