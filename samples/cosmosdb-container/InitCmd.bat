echo Start Azure Cosmos Emulator
powershell.exe -NoExit -NoLogo -Command C:\\CosmosDB.Emulator\\Start.ps1

echo Start populating cosmos.
C:\\app\\CosmosDBCompass\\CosmosDBCompass.exe -k -d:c:\\app\\sample-data -c:AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;
echo Finished populating cosmos.