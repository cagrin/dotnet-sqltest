## Show output during test

```dotnet test ./SqlTest.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=lcov -l "console;verbosity=detailed" ```

## Run project

```dotnet run --project SqlTest -- runall --image cagrin/azure-sql-edge-arm64 --project Database.Tests/Ok```

## Switch dotnet cli to a non-system language

```$Env:DOTNET_CLI_UI_LANGUAGE="en"```