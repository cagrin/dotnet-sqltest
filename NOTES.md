## Show output during test

```dotnet test ./SqlTest.Tests -l "console;verbosity=detailed" /p:CollectCoverage=true /p:CoverletOutputFormat=lcov```

## Run project

```dotnet run --project SqlTest -- runall --image cagrin/azure-sql-edge-arm64 --project Database.Tests```

## Switch dotnet cli to a non-system language

```$Env:DOTNET_CLI_UI_LANGUAGE="en"```