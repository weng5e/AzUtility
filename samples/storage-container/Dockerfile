FROM microsoft/azure-storage-emulator

RUN powershell.exe -Command \
    $ErrorActionPreference = 'Stop'; \
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; \
    Invoke-WebRequest https://github.com/weng5e/AzUtility/releases/download/0.0.1/BlobUploader.zip -OutFile C:\BlobUploader.zip; \
    Expand-Archive -LiteralPath C:\BlobUploader.zip -DestinationPath C:\app; \
    Remove-Item C:\BlobUploader.zip -Force;

ADD sample-data ../app/sample-data
COPY InitCmd.bat ../InitCmd.bat

CMD ["c:\\InitCmd.bat"]