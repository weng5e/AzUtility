echo Start Nginx
start C:\\nginx\\nginx.exe -g "daemon off;"

echo Start populating blob.
C:\\app\\BlobUploader\\BlobUploader.exe -d:c:\\app\\sample-data -l
echo Finished populating blob.