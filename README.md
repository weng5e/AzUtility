# AzUtility

## BlobUploader
BlobUploader is a tool that can upload all files within a folder to Azure Blob Storage. It can be downloaded from the [release page](https://github.com/weng5e/AzUtility/releases/).

### Usage
```console
BlobUploader.exe [options]

Options:
  -d            Relative or absolute path to the folder that you want to upload.
  -c            Storage connection string. Can be ignored when '-l' is specified.
  -l            Use local storage emulator.
  -?|-h|--help  Show help information
```

## CosmosDBCompass
CosmosDBCompass is a tool that can backup and recover Azure Cosmos DB. It can be downloaded from the [release page](https://github.com/weng5e/AzUtility/releases/).

### Usage
```console
CosmosDBCompass.exe [options]

Options:
  -b            Backup.
  -d            Relative or absolute path to the folder that you want to upload or download to.
  -c            CosmosDB connection string.
  -?|-h|--help  Show help information
```