echo "Start building Electron.NET dev stack..."

echo "Restore & Build the project"
cd CryptoManager
dotnet restore
dotnet build
cd ../

echo "Invoke electronize build Windows"
electronize build /target win /dotnet-configuration release /PublishReadyToRun true