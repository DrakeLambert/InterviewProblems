using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DrakeLambert.ScheduledFileIO;

namespace DrakeLambert.WriteSchedulerDemo
{
    class Program
    {
        const int minimumFileSize = 1000;
        const int maximumFileSize = 10000000;
        const int fileCount = 1000;

        const int deviceCount = 10;
        static readonly string fileWriteBasePath = Path.Combine(Path.GetTempPath(), "WriteSchedulerDemo");

        static void Main(string[] args)
        {
            // generate file data
            Console.Write($"Generating {fileCount} file(s) with random data between {minimumFileSize} and {maximumFileSize} bytes long...");
            var random = new Random();
            var files = Enumerable.Range(0, fileCount).Select(i =>
            {
                var byteCount = random.Next(minimumFileSize, maximumFileSize);
                var bytes = new byte[byteCount];
                return new MockFile { Name = i.ToString(), Data = bytes };
            }).ToList();
            Console.WriteLine("Done");

            // partition files according to processor count
            var threadCount = Environment.ProcessorCount;
            Console.Write($"Partitioning files into {threadCount} groups...");
            var partitionSize = (int)Math.Ceiling(fileCount / (double)threadCount);
            var partitions = new List<List<MockFile>>();
            for (var i = 0; i < threadCount; i++)
            {
                partitions.Add(files.Skip(i * partitionSize).Take(partitionSize).ToList());
            }
            Console.WriteLine("Done");

            var devices = Enumerable.Range(0, deviceCount).Select(i => new FileDevice(Path.Combine(fileWriteBasePath, $"device{i}"), mockWrite: false)).ToArray();

            var roundRobinTotal = 0;
            var optimizedTotal = 0;
            var count = 2;
            for (var i = 0; i < count; i++)
            {
                roundRobinTotal += (int)RunSchedulerTest(new OptimizedWriteScheduler(devices), partitions);
            }
            for (var i = 0; i < count; i++)
            {
                optimizedTotal += (int)RunSchedulerTest(new OptimizedWriteScheduler(devices), partitions);
            }
            Console.WriteLine("-------------");
            Console.WriteLine("AVERAGE TIMES");
            var roundRobinAverage = roundRobinTotal / (double)count;
            Console.WriteLine($"Round Robin: {roundRobinAverage}");
            var optimizedAverage = optimizedTotal / (double)count;
            Console.WriteLine($"Optimized: {optimizedAverage}");
            Console.WriteLine($"{(optimizedAverage - roundRobinAverage) / roundRobinAverage * -100}% improvement");
        }

        static long RunSchedulerTest(WriteScheduler scheduler, List<List<MockFile>> partitions)
        {
            // create temporary directories
            var directories = Enumerable.Range(0, deviceCount).Select(i => Path.Combine(fileWriteBasePath, $"device{i}"));
            foreach (var directory in directories)
            {
                Directory.CreateDirectory(directory);
            }

            // start file writer threads
            var partitionCount = partitions.Count;
            var fileWriterTasks = new Task[partitionCount];
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < partitionCount; i++)
            {
                var partition = partitions[i];
                fileWriterTasks[i] = Task.Run(() =>
                {
                    foreach (var file in partition)
                    {
                        scheduler.Write(file.Name, file.Data);
                    }
                });
            }
            Task.WhenAll(fileWriterTasks).GetAwaiter().GetResult();
            stopwatch.Stop();

            // temporary file cleanup
            Directory.Delete(fileWriteBasePath, true);

            return stopwatch.ElapsedMilliseconds;
        }
    }

    class MockFile
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
