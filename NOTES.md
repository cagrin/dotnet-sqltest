## Show output during test

```dotnet test ./SqlTest.Tests -l "console;verbosity=detailed" /p:CollectCoverage=true /p:CoverletOutputFormat=lcov```

## Run project

```dotnet run --project SqlTest -- runall --image cagrin/azure-sql-edge-arm64 --project Database.Tests```