[![NuGet](https://img.shields.io/nuget/v/dotnet-sqltest)](https://www.nuget.org/packages/dotnet-sqltest)
[![Nuget](https://img.shields.io/nuget/dt/dotnet-sqltest)](https://www.nuget.org/stats/packages/dotnet-sqltest?groupby=Version)
[![Coverage Status](https://img.shields.io/coveralls/github/cagrin/dotnet-sqltest)](https://coveralls.io/github/cagrin/dotnet-sqltest)


# dotnet-sqltest
Command line tool for running tSQLt unit tests from MSBuild.Sdk.SqlProj projects inside throwaway instances of Docker containers for all compatible SQL Server or Azure SQL Edge images.

## Usage
Install the tool from the package:

```dotnet tool install -g dotnet-sqltest```

Update the tool to newer version:

```dotnet tool update dotnet-sqltest -g```

Invoke the tool:

```sqltest [command] [options]```

## Example

Running all tSQLt tests inside throwaway SQL Server 2019 container:

```sqltest runall --project Database.Tests --image mcr.microsoft.com/mssql/server```

## Build

[dotnet-sqltest](https://www.nuget.org/packages/dotnet-sqltest) is build with .NET 6.

Powered by:
- [Dapper](https://www.nuget.org/packages/Dapper)
- [DotNet.Testcontainers](https://www.nuget.org/packages/DotNet.Testcontainers)
- [Microsoft.PowerShell.SDK](https://www.nuget.org/packages/Microsoft.PowerShell.SDK)
- [System.CommandLine](https://www.nuget.org/packages/System.CommandLine)