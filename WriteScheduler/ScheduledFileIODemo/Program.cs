using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using DrakeLambert.ScheduledFileIO;

namespace ScheduledFileIODemo
{
    class Program
    {
        static void Main(string[] args)
        {
            args = args.Select(arg => arg.ToLower()).ToArray();

            // constants
            var minimumFileSize = 1_000; // bytes
            var maximumFileSize = 1_000_000; // bytes
            var threadCount = Environment.ProcessorCount;

            var fileCount = 500;
            if (args.Contains("-filecount"))
            {
                var fileIndex = Array.IndexOf(args, "-filecount");
                fileCount = int.Parse(args[fileIndex + 1]);
            }

            var testCount = 50;
            if (args.Contains("-testcount"))
            {
                var testIndex = Array.IndexOf(args, "-testcount");
                testCount = int.Parse(args[testIndex + 1]);
            }

            var mockWrite = args.Contains("-mockwrite");

            var useOptimizedScheduler = args.Contains("-optimized");

            Console.WriteLine("Starting test...");
            Console.WriteLine($"Using {(useOptimizedScheduler ? "optimized" : "round robin")} scheduler.");
            Console.WriteLine($"{fileCount} files per test run.");
            Console.WriteLine($"{testCount} test runs.");
            Console.WriteLine($"{(mockWrite ? "Simulating" : "Using")} file system writes.");


            // create files
            var filePartitions = Enumerable.Range(0, threadCount)
                .Select(i => minimumFileSize + i * (maximumFileSize - minimumFileSize) / (double)threadCount)
                .Select(size => CreateFiles(fileCount, (int)size)).ToList();

            // create devices
            var tempWritePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var devices = CreateDevices(10, tempWritePath, mockWrite);

            // create scheduler
            var scheduler = useOptimizedScheduler
                ? new OptimizedWriteScheduler(devices) as WriteScheduler
                : new RoundRobinWriteScheduler(devices) as WriteScheduler;

            // arrange tests
            var testTimes = new List<int>();
            var stopwatch = new Stopwatch();
            for (var i = 0; i < testCount; i++)
            {
                // create threads
                var fileWriterWorkers = new List<(FileWriter, Thread)>();
                for (var j = 0; j < threadCount; j++)
                {
                    var fileWriter = new FileWriter(scheduler, filePartitions[j]);
                    var thread = new Thread(fileWriter.WriteFilesToScheduler);
                    fileWriterWorkers.Add((fileWriter, thread));
                }

                // Execute threads
                stopwatch.Start();
                foreach (var (_, thread) in fileWriterWorkers)
                {
                    thread.Start();
                }
                foreach (var (_, thread) in fileWriterWorkers)
                {
                    thread.Join();
                }
                stopwatch.Stop();
                testTimes.Add((int)stopwatch.ElapsedMilliseconds);
                // Console.WriteLine($"Test {i + 1} Time (ms): {stopwatch.ElapsedMilliseconds}");
                stopwatch.Reset();
            }
            Console.WriteLine($"Average Time (ms): {testTimes.Average()}");
            Console.WriteLine($"Standard Deviation (ms): {StandardDeviation(testTimes)}");

            // cleanup temporary folders
            Directory.Delete(tempWritePath, recursive: true);
        }

        static MockFile[] CreateFiles(int count, int filesLength)
        {
            var files = new MockFile[count];
            for (var i = 0; i < count; i++)
            {
                files[i] = new MockFile { Name = i.ToString(), Data = new byte[filesLength] };
            }
            return files;
        }

        static Device[] CreateDevices(int count, string writePath, bool mockWrite)
        {
            var devices = new Device[count];
            for (int i = 0; i < count; i++)
            {
                var devicePath = Path.Combine(writePath, "Device" + i.ToString());
                devices[i] = new FileDevice(devicePath, mockWrite);
            }
            return devices;
        }

        static double StandardDeviation(IEnumerable<int> numbers)
        {
            var mean = numbers.Average();
            var standardDeviation = Math.Sqrt(numbers.Select(n => n - mean).Select(n => n * n).Average());
            return standardDeviation;
        }
    }

    class FileWriter
    {
        private readonly WriteScheduler _scheduler;
        private readonly IEnumerable<MockFile> _files;

        public long ElapsedMilliseconds { get; private set; } = 0;

        public FileWriter(WriteScheduler scheduler, IEnumerable<MockFile> files)
            => (_scheduler, _files) = (scheduler, files);

        public void WriteFilesToScheduler()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var file in _files)
            {
                _scheduler.Write(file.Name, file.Data);
            }
            stopwatch.Stop();
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        }
    }

    class MockFile
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
