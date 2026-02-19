using System.Xml.Linq;
using Parquet.Schema;

namespace DataView;

public class XmlFileService : ITabularFileService
{
    private string _rootName = "rows";
    private string _rowName  = "row";

    public Task<(DataField[] Fields, List<Dictionary<string, object?>> Rows)> LoadAsync(string path)
    {
        var doc = XDocument.Load(path);
        var root = doc.Root ?? throw new InvalidDataException("XML file has no root element.");

        _rootName = root.Name.LocalName;

        // Collect ordered union of all child element names across all row elements
        var fieldNames = new List<string>();
        var fieldSet = new HashSet<string>(StringComparer.Ordinal);

        var rowElements = root.Elements().ToList();
        if (rowElements.Count > 0)
            _rowName = rowElements[0].Name.LocalName;

        foreach (var rowEl in rowElements)
        {
            foreach (var fieldEl in rowEl.Elements())
            {
                if (fieldSet.Add(fieldEl.Name.LocalName))
                    fieldNames.Add(fieldEl.Name.LocalName);
            }
        }

        var fields = fieldNames
            .Select(n => new DataField(n, typeof(string), isNullable: true))
            .ToArray();

        var rows = new List<Dictionary<string, object?>>();
        foreach (var rowEl in rowElements)
        {
            var row = new Dictionary<string, object?>();
            foreach (var name in fieldNames)
            {
                var child = rowEl.Element(name);
                row[name] = child == null ? null : child.Value;
            }
            rows.Add(row);
        }

        return Task.FromResult((fields, rows));
    }

    public Task SaveAsync(string path, DataField[] fields, List<Dictionary<string, object?>> rows)
    {
        var root = new XElement(_rootName,
            rows.Select(row =>
                new XElement(_rowName,
                    fields.Select(f =>
                        new XElement(f.Name,
                            row.TryGetValue(f.Name, out var v) ? v?.ToString() ?? "" : "")))));

        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), root);
        doc.Save(path);
        return Task.CompletedTask;
    }
}
