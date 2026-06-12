## Switch dotnet cli to a non-system language

```
$Env:DOTNET_CLI_UI_LANGUAGE="en"
```

## Show output during test

```
dotnet test ./SqlTest.Tests --framework net10.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov -l "console;verbosity=detailed"
```

## Filter smoke tests

```
pwsh
dotnet test ./SqlTest.Tests --framework net10.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov --filter "(ClassName!~SqlTest.DatabaseTests)"
```

## Run project

```
dotnet run --project SqlTest --framework net10.0 -- runall --image mcr.microsoft.com/mssql/server:2022-latest --project Database.Tests/Ok
```

## Exclude Dacpacks from outdated dependencies

```
dotnet-outdated -r -vl Major
```

## Run only quick unit tests

```
dotnet test --framework net10.0 --filter "UnitTests"
```