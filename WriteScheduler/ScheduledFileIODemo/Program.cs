using System;
using System.Collections.Generic;
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
        const int fileCount = 10000;

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
                random.NextBytes(bytes);
                return new MockFile { Name = i.ToString(), Data = bytes };
            });
            Console.WriteLine("Done");

            // partition files according to processor count
            var threadCount = Environment.ProcessorCount;
            Console.Write($"Partitioning files into {threadCount} groups...");
            var partitionSize = (int)Math.Ceiling(fileCount / (double)threadCount);
            var partitions = new IEnumerable<MockFile>[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                partitions[i] = files.Skip(i * partitionSize).Take(partitionSize);
            }
            Console.WriteLine("Done");

            // generate temporary directories
            Console.Write($"Generating {deviceCount} temporary directories...");
            var directories = Enumerable.Range(0, deviceCount).Select(i => Path.Combine(fileWriteBasePath, $"device{i}"));
            foreach (var directory in directories)
            {
                Directory.CreateDirectory(directory);
            }
            Console.WriteLine("Done");

            // configure devices and scheduler
            var devices = Enumerable.Range(0, deviceCount).Select(i => new FileDevice(Path.Combine(fileWriteBasePath, $"device{i}"))).ToArray();
            var scheduler = new RoundRobinWriteScheduler(devices);

            // start file writer threads
            Console.Write($"Starting {threadCount} file writers...");
            var fileWriterTasks = new Task[threadCount];
            for (var i = 0; i < threadCount; i++)
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
            Console.WriteLine("Done");
        }
    }

    class MockFile
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
