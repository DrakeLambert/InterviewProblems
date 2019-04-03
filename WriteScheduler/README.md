# Write Scheduler

Three projects are included in the solution.

- ScheduledFileIO
  - A .NET Standard 2.0 library exposing the `Device` & `WriteScheduler` interfaces.
  - Includes related implementations
    - `FileDevice : Device`
    - `RoundRobinWriteScheduler : WriteScheduler`
    - `OptimizedWriteScheduler : WriteScheduler`
- ScheduledFileIODemo
  - .NET Core console application that demonstrates the functionality of the above library.
- ScheduledFileIOTests
  - .NET Core Xunit unit tests that test the functionality of the above library.

## `FileDevice` Notes

- Implements `Device`.
- Writes files sequentially.
  - Uses Monitor to mirror queue-like sequential file writes.
- Only updates `TotalBytesWritten` after completed write.
- Can simulate file system writes with thread delay proportional to data length.
- Uses atomic shared integer operations for thread safety.

## `RoundRobinWriteScheduler` Notes

- Implements `WriteScheduler`.
- Deals write requests to devices evenly, in order.
- Uses Monitor to ensure thread safety.

## `OptimizedWriteScheduler` Notes

- Implements `WriteScheduler`.
- Goal of optimization is to process writes as fast as possible.
- Deals write requests to device with fewest pending writes.
- Attempts to balance write operations between devices by testing next device first.
- Uses Monitor to ensure thread safety.

## Demo Application

This app spins up 20 threads that produce a variable amount of files written to 5 devices. It measures how long it takes to write all files to a each type of scheduler, and averages this over several test runs.

### CLI Arguments

`-mockwrite`: blocks CPU for a time proportional to file data length instead of writing to file system.

`-filecount [count]`: specifies how many files the thread pool will process.

`-testcount [count]`: specifies how many duplicate tests are run.

## How To Run The Tests & Demo

Prerequisites:

- .NET Core 2.2 SDK installed ([download](https://dotnet.microsoft.com/download))

Steps:

1. Run `runDemo.ps1` in PowerShell

Alternatively:

1. Run tests with `dotnet test .\ScheduledFileIOTests\`
1. Run demo with `dotnet run --project .\ScheduledFileIODemo\`