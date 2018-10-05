FROM microsoft/azure-cosmosdb-emulator

RUN powershell.exe -Command \
    $ErrorActionPreference = 'Stop'; \
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; \
    Invoke-WebRequest https://github.com/weng5e/AzUtility/releases/download/0.0.3/CosmosDBCompass.zip -OutFile C:\CosmosDBCompass.zip; \
    Expand-Archive -LiteralPath C:\CosmosDBCompass.zip -DestinationPath C:\app; \
    Remove-Item C:\CosmosDBCompass.zip -Force;

ADD sample-data ../app/sample-data
COPY InitCmd.bat ../InitCmd.bat

# CMD ["c:\\InitCmd.bat"]

ENTRYPOINT C:\InitCmd.bat