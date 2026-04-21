global using SqlTest.Tests;

[assembly: Parallelize(Workers = 2, Scope = ExecutionScope.ClassLevel)]