using System;
using System.Collections.Generic;
using System.IO;
using DrakeLambert.ScheduledFileIO;

namespace ScheduledFileIODemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = CreateFiles(1000, 10000);

            var devices = CreateDevices(10);

            var scheduler = new OptimizedWriteScheduler(devices);

            


            WriteFilesToScheduler(scheduler, files);
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

        static Device[] CreateDevices(int count)
        {
            var devices = new Device[count];
            var devicesWritePath = Path.GetTempPath();
            for (int i = 0; i < count; i++)
            {
                var devicePath = Path.Combine(devicesWritePath, "Device" + i.ToString());
                devices[i] = new FileDevice(devicePath, false);
            }
            return devices;
        }

        static void WriteFilesToScheduler(WriteScheduler scheduler, IEnumerable<MockFile> files)
        {
            foreach (var file in files)
            {
                scheduler.Write(file.Name, file.Data);
            }
        }
    }

    class MockFile
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
