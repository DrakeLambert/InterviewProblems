# Circular Array

Two projects are included in the solution.

- CircularArrayProblem
  - A .NET Standard 2.0 library exposing the `CircularArray<T>` API.
- CircularArrayTests
  - .NET Core Xunit unit tests that test the functionality of the above library.
  - Broken up into two classes:
    - `CircularArrayTests`: tests of core functionality. Tests are single-threaded.
    - `CircularArrayConcurrentTests`: tests for race conditions.

## `CircularArray<T>` Notes

The data structure maintains thread safety by using locking with monitors and "snapshot" variables. GC pressure is minimized by relying heavily on value types and using reference types sparingly. All reference types used internally are long lived, further alleviating GC pressure.

## How To Run The Tests

Prerequisites:

- .NET Core 2.2 SDK installed ([download](https://dotnet.microsoft.com/download))

Steps:

1. Run tests with `dotnet test`