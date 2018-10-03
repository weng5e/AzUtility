FROM microsoft/azure-storage-emulator

WORKDIR /app

RUN powershell.exe -Command \
    $ErrorActionPreference = 'Stop'; \
    Invoke-WebRequest https://aka.ms/vs/15/release/vc_redist.x64.exe -OutFile c:\vc_redist.x64.exe; \
    Start-Process c:\vc_redist.x64.exe -ArgumentList '/quiet /install' -Wait; \
    Remove-Item c:\vc_redist.x64.exe -Force;

COPY Gateway.zip /Gateway.zip

RUN powershell.exe -Command \
    $ErrorActionPreference = 'Stop'; \
    Expand-Archive -LiteralPath C:\Gateway.zip -DestinationPath C:\app; \
    Remove-Item C:\Gateway.zip;
    
https://github.com/weng5e/AzUtility/releases/download/0.0.1/BlobUploader.zip