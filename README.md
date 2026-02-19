# DataView

A lightweight Windows desktop application for viewing and editing tabular data files. Built with .NET 8 and Windows Forms.

## Features

- **Open and view** tabular data files in a grid
- **Edit the schema** — rename columns, change data types, and toggle nullable
- **Add, edit, clone, and remove rows** via toolbar and right-click context menu
- **Save** back to the original file or convert to a different format on save
- **Async loading and saving** with a progress dialog so the UI stays responsive

## Supported File Formats

| Format | Extension |
|--------|-----------|
| Apache Parquet | `.parquet` |
| JSON | `.json` |
| XML | `.xml` |
| CSV (RFC 4180, UTF-8 BOM) | `.csv` |

You can open a file in one format and save it as another.

## Supported Column Types

`string`, `int`, `long`, `float`, `double`, `decimal`, `bool`, `DateTime`, `DateOnly`, `Guid`

## Requirements

- Windows 10 or later (x64)
- No installation needed — runs as a single self-contained executable

## Building

```bash
dotnet build
```

### Publish as single executable

```bash
dotnet publish -c Release
```

The output is a single self-contained `.exe` in `bin/Release/net8.0-windows/win-x64/publish/`.

### Build the installer (MSI)

Requires [WiX Toolset v5](https://wixtoolset.org/) and the `dotnet wix` global tool:

```bash
dotnet tool install --global wix
```

Then publish the app first, and build the installer:

```bash
dotnet publish -c Release
dotnet build Setup/DataViewSetup.wixproj -c Release
```

The installer `DataViewSetup.msi` is placed in `Setup/bin/Release/`.

## Project Structure

```
DataViewApp/
├── Program.cs              # Entry point
├── Forms/                  # UI forms
│   ├── MainForm            # Main window with data grid and schema editor
│   ├── AddRowDialog        # Dialog for adding, editing, and cloning rows
│   └── LoadingDialog       # Progress dialog shown during load/save
├── Services/               # File format implementations
│   ├── ITabularFileService # Common interface for all formats
│   ├── CsvFileService
│   ├── JsonFileService
│   ├── ParquetFileService
│   ├── ParquetService
│   └── XmlFileService
├── Setup/                  # WiX v5 installer project
│   ├── DataViewSetup.wixproj
│   └── Package.wxs
└── assets/icon/            # Application icon
```

## Dependencies

- [Parquet.Net](https://github.com/aloneguid/parquet-dotnet) — Apache Parquet read/write support
