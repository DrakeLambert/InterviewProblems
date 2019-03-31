namespace DrakeLambert.ScheduledFileIO
{
    public interface Device
    {
        /// <summary>
        /// Returns the current number of pending writes for this device. This number
        /// should be incremented when a write is started and decremented when a
        /// write is done.
        /// </summary>
        int PendingWrites { get; }

        /// <summary>
        /// Returns the total number of writes for this device. This number should be
        /// incremented for every write.
        /// </summary>
        int TotalWrites { get; }

        /// <summary>
        /// Returns the total number of bytes written to this device. This number should be
        /// incremented for every write.
        /// </summary>
        int TotalBytesWritten { get; }

        /// <summary>
        /// Writes a new file to the device
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file's data</param>
        void Write(string name, byte[] data);
    }
}
