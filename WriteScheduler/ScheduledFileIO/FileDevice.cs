using System.IO;
using System.Threading;

namespace DrakeLambert.ScheduledFileIO
{
    public class FileDevice : Device
    {
        private int _totalWrites;
        private int _pendingWrites;
        private int _totalBytesWritten;

        private object _writeLock = new object();

        private string _fileWritePath;

        public FileDevice(string fileWritePath) => _fileWritePath = fileWritePath;

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
                    File.WriteAllBytes(Path.Combine(_fileWritePath, name), data);
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
