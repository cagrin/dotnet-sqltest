## Show output during test

```dotnet test ./SqlTest.Tests -e CollectCoverage=true -e CoverletOutputFormat=lcov -l "console;verbosity=detailed" ```

## Run project with .NET 6

```dotnet run --project SqlTest --framework net6.0 -- runall --image cagrin/azure-sql-edge-arm64 --project Database.Tests/Ok```

## Switch dotnet cli to a non-system language

```$Env:DOTNET_CLI_UI_LANGUAGE="en"```

## Filter smoke tests

```dotnet test ./SqlTest.Tests -e CollectCoverage=true -e CoverletOutputFormat=lcov --filter "(ClassName!~SqlTest.DatabaseTests)"```