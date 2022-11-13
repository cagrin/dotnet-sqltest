## Switch dotnet cli to a non-system language

```$Env:DOTNET_CLI_UI_LANGUAGE="en"```

## Show output during test with .NET 6

```dotnet test ./SqlTest.Tests --framework net6.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov -l "console;verbosity=detailed" ```

## Filter smoke tests with .NET 6

```dotnet test ./SqlTest.Tests --framework net6.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov --filter "(ClassName!~SqlTest.DatabaseTests)"```

## Run project with .NET 6

```dotnet run --project SqlTest --framework net6.0 -- runall --image cagrin/azure-sql-edge-arm64 --project Database.Tests/Ok```