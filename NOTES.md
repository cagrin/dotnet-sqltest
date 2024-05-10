## Switch dotnet cli to a non-system language

```
$Env:DOTNET_CLI_UI_LANGUAGE="en"
```

## Show output during test with .NET 8

```
dotnet test ./SqlTest.Tests --framework net8.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov -l "console;verbosity=detailed"
```

## Filter smoke tests with .NET 8

```
pwsh
dotnet test ./SqlTest.Tests --framework net8.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov --filter "(ClassName!~SqlTest.DatabaseTests)"
```

## Run project with .NET 8

```
dotnet run --project SqlTest --framework net8.0 -- runall --image mcr.microsoft.com/mssql/server --project Database.Tests/Ok
dotnet run --project SqlTest --framework net8.0 -- runall --image mcr.microsoft.com/azure-sql-edge --project Database.Tests/Ok
```

## Exclude Dacpacks from outdated dependencies

```
dotnet-outdated -r -exc "Microsoft.SqlServer.Dacpacs"
```

## Run only quick unit tests

```
dotnet test --framework net8.0 --filter "UnitTests"
```