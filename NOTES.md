## Switch dotnet cli to a non-system language

```
$Env:DOTNET_CLI_UI_LANGUAGE="en"
```

## Show output during test

```
dotnet test ./SqlTest.Tests --framework net10.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov -l "console;verbosity=detailed"
```

## Test categories

- no category - unit tests with in-memory fakes and replayed fixtures (`SqlTest.Tests/Fixtures`), run in milliseconds
- `Integration` - real `dotnet` processes and real database projects, no SQL Server or Docker required
- `Docker` - real SQL Server containers; together with the categories above they cover 100% of lines

## Filter tests without Docker

```
dotnet test ./SqlTest.Tests --framework net10.0 -e CollectCoverage=true -e CoverletOutputFormat=lcov --filter "TestCategory!=Docker"
```

## Use Podman instead of Docker (Windows)

Podman forwards the Docker API to `npipe:////./pipe/docker_engine`, so Testcontainers needs no configuration.
Docker tests pull the image with `podman` when `docker` is not on PATH (override with `SQLTEST_CONTAINER_CLI`).

```
podman machine start
dotnet test ./SqlTest.Tests --framework net10.0
```

If the pipe is taken by another tool, point Testcontainers to Podman directly:

```
$Env:DOCKER_HOST="npipe:////./pipe/podman-machine-default"
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
dotnet test --framework net10.0 --filter "TestCategory!=Docker&TestCategory!=Integration"
```

## Refresh fixtures from real runs

Records what real processes, SQL Server and SQLCover returned into `SqlTest.Tests/Fixtures/*.json`.

```
$Env:SQLTEST_RECORD_FIXTURES="1"
dotnet test ./SqlTest.Tests --framework net10.0 --filter "TestCategory=Docker|TestCategory=Integration"
Remove-Item Env:SQLTEST_RECORD_FIXTURES
```