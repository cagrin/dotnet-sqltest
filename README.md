[![NuGet](https://img.shields.io/nuget/v/dotnet-sqltest)](https://www.nuget.org/packages/dotnet-sqltest)
[![Nuget](https://img.shields.io/nuget/dt/dotnet-sqltest)](https://www.nuget.org/stats/packages/dotnet-sqltest?groupby=Version)
[![Coverage Status](https://img.shields.io/coveralls/github/cagrin/dotnet-sqltest)](https://coveralls.io/github/cagrin/dotnet-sqltest)


# dotnet-sqltest
Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects inside throwaway instances of Docker containers for all compatible SQL Server or Azure SQL Edge images.

## Usage
Install the tool from the package:

```dotnet tool install --global dotnet-sqltest```

Update the tool to newer version:

```dotnet tool update --global dotnet-sqltest```

Invoke the tool:

```sqltest runall [options]```

```
Options:
  -i, --image <image>          Docker image.
  -p, --project <project>      Database project.
  -c, --collation <collation>  Server collation.
  -r, --result <result>        Save result in JUnit XML file.
  --cc-disable                 Disable code coverage.
  --cc-include-tsqlt           Include code coverage of tSQLt schema.
  --windows-container          Run as Windows container.
  -?, -h, --help               Show help and usage information
```

## Example

Running all tSQLt tests inside throwaway SQL Server 2019 container:

```sqltest runall --project Database.Tests/Ok --image mcr.microsoft.com/mssql/server:2019-latest```

```
Preparing database... 6 s
Deploying database... 10 s
Running all tests.... 435 ms
Gathering coverage... 1 s
Uncovered statements:
  [dbo].[Example]: SELECT A = 1 INTO #Example
  [dbo].[Example]: UPDATE #Example SET [...]
Failed: 0, Passed: 1, Coverage: 60% (3/5), Duration: 18 s
```

## Build

[dotnet-sqltest](https://www.nuget.org/packages/dotnet-sqltest) is build with .NET 6 and 7.

Powered by:
- [Dapper](https://www.nuget.org/packages/Dapper)
- [Microsoft.PowerShell.SDK](https://www.nuget.org/packages/Microsoft.PowerShell.SDK)
- [SQLCoverLib](https://www.nuget.org/packages/SQLCoverLib)
- [System.CommandLine](https://www.nuget.org/packages/System.CommandLine)
- [Testcontainers](https://www.nuget.org/packages/Testcontainers)