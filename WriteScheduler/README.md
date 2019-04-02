# Write Scheduler

Two projects are included in the solution.

- ScheduledFileIO
  - A .Net Standard 2.0 library exposing the `Device` & `WriteScheduler` interfaces.
  - Includes related implementations
    - `FileDevice : Device`
    - `RoundRobinWriteScheduler : WriteScheduler`
    - `OptimizedWriteScheduler : WriteScheduler`
- ScheduledFileIODemo
  - .Net Core console application that demonstrates the functionality of the above library.

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