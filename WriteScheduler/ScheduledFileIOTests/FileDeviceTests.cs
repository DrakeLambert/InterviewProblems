using System.IO;
using System.Linq;
using System.Threading;
using DrakeLambert.ScheduledFileIO;
using Xunit;

namespace DrakeLambert.ScheduledFileIOTests
{
    public class FileDeviceTests
    {
        [Fact]
        public void Write_ValidData_IncrementsTotalWritesAndTotalBytesWritten()
        {
            var writePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var device = new FileDevice(writePath, mockWrite: false);
            var tempFileName = "testName";
            var bytesToWrite = 100;

            device.Write(tempFileName, new byte[bytesToWrite]);

            Assert.Equal(1, device.TotalWrites);
            Assert.Equal(bytesToWrite, device.TotalBytesWritten);
        }

        [Fact]
        public void Write_ValidData_IncrementsAndDecrementsPendingWrites()
        {
            var writePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var device = new FileDevice(writePath, mockWrite: true);
            var tempFileName = "testName";
            var bytesToWrite = 1_000_000;
            var threadCount = 10;
            var writerThreads = Enumerable.Range(0, threadCount)
                .Select(_ => new Thread(() => device.Write(tempFileName, new byte[bytesToWrite]))).ToList();

            foreach (var thread in writerThreads)
            {
                thread.Start();
            }
            Assert.Equal(threadCount, device.PendingWrites);
            foreach (var thread in writerThreads)
            {
                thread.Join();
            }
            Assert.Equal(0, device.PendingWrites);
        }
    }
}
