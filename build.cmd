echo "Start building Electron.NET dev stack..."

echo "Restore & Build the project"
cd CryptoManager
dotnet restore
dotnet build
cd ../

echo "Invoke electronize build Windows"
electronize build /target win /dotnet-configuration release

echo "Invoke electronize build Linux"
electronize build /target linux /dotnet-configuration release