using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DrakeLambert.ScheduledFileIO
{
    public class FileDevice : Device
    {
        private int _totalWrites;
        private int _pendingWrites;
        private int _totalBytesWritten;

        private object _writeLock = new object();

        private string _fileWritePath;
        private readonly bool _mockWrite;

        /// <summary>
        /// Creates a new instance and the specified directory.
        /// </summary>
        /// <param name="fileWritePath">The path to write files.</param>
        /// <param name="mockWrite">If true, file writes do not take place: the thread is delayed according to the length of the file data. If false, the file write takes place.</param>
        /// <returns></returns>
        public FileDevice(string fileWritePath, bool mockWrite)
        {
            _fileWritePath = fileWritePath;
            _mockWrite = mockWrite;

            if (Directory.Exists(_fileWritePath))
            {
                Directory.Delete(_fileWritePath, true);
            }
            Directory.CreateDirectory(_fileWritePath);
        }

        public int PendingWrites => Interlocked.CompareExchange(ref _pendingWrites, 0, 0);
        public int TotalWrites => Interlocked.CompareExchange(ref _totalWrites, 0, 0);
        public int TotalBytesWritten => Interlocked.CompareExchange(ref _totalBytesWritten, 0, 0);

        /// <summary>
        /// Creates a new file with the given name and data.
        /// </summary>
        /// <remarks>Overwrites existing files.</remarks>
        /// <param name="name">The file name.</param>
        /// <param name="data">The file data.</param>
        public void Write(string name, byte[] data)
        {
            Interlocked.Increment(ref _pendingWrites);
            lock (_writeLock)
            {
                try
                {
                    if (_mockWrite)
                    {
                        // file system latency (2 ms)
                        Task.Delay(2).GetAwaiter().GetResult();
                        // file system write time (200 MiB/s)
                        Task.Delay(data.Length / 209715200).GetAwaiter().GetResult();
                    }
                    else
                    {
                        File.WriteAllBytes(Path.Combine(_fileWritePath, name), data);
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _pendingWrites);
                }
                Interlocked.Increment(ref _totalWrites);
                Interlocked.Add(ref _totalBytesWritten, data.Length);
            }
        }
    }
}
