# flag arguments to target specific builds are available.

# sh ./buildAll.sh

# sh ./buildAll.sh -t osx
# sh ./buildAll.sh -t win
# sh ./buildAll.sh -t linux

# sh ./buildAll.sh -t osx -p *.NET5.csproj
# sh ./buildAll.sh -t win -p *.NET5.csproj
# sh ./buildAll.sh -t linux -p *.NET5.csproj

# sh ./buildAll.sh -t osx -p *.NET6.csproj
# sh ./buildAll.sh -t win -p *.NET6.csproj
# sh ./buildAll.sh -t linux -p *.NET6.csproj

target=default
project="*.NET5.cspoj"
while getopts t:p: flag; do
    case "${flag}" in
    t) target=${OPTARG} ;;
    p) project=${OPTARG} ;;
    esac
done

echo "Targeting $target & Project $project"

dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
echo "Start building Electron.NET dev stack..."

echo "Restore & Build the project"
cd CryptoManager
dotnet restore
dotnet build
cd ..

echo "Restore & Build "
pushd $dir/CryptoManager
    dotnet restore $project
    dotnet build $project

    echo "Invoke electronize build in WebApp Demo"

    if [[ "$target" != "default" ]]; then
        echo "/target $target (dev-build)"
        /mnt/c/Users/Sebastien/.dotnet/tools/electronize.exe build /target $target /dotnet-project $project
    else
        echo "/target win (dev-build)"
        /mnt/c/Users/Sebastien/.dotnet/tools/electronize.exe build /target win /dotnet-project $project

        echo "/target linux (dev-build)"
        /mnt/c/Users/Sebastien/.dotnet/tools/electronize.exe build /target AppImage /dotnet-project $project

        # Cannot publish osx/win on windows due to:
        # NETSDK1095: Optimizing assemblies for performance is not supported for the selected target platform or architecture.
        if [[ "$OSTYPE" != "linux-gnu"* ]]; then
            echo "/target osx (dev-build)"
            /mnt/c/Users/Sebastien/.dotnet/tools/electronize.exe build /target osx /dotnet-project $project

            echo "/target custom win7-x86;win (dev-build)"
            /mnt/c/Users/Sebastien/.dotnet/tools/electronize.exe build /target custom "win7-x86;win" /dotnet-project $project
        fi
    fi

# Be aware, that for non-electronnet-dev environments the correct
# invoke command would be dotnet electronize ...