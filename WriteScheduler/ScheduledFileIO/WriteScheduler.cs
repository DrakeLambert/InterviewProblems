namespace DrakeLambert.ScheduledFileIO
{
    public interface WriteScheduler
    {
        /// <summary>
        /// Writes a new file to the underlying set of devices.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="data">The file's data</param>
        /// <returns>The Device that received the file.</returns>
        Device Write(string name, byte[] data);
    }
}
