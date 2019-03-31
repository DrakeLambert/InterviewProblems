namespace DrakeLambert.ScheduledFileIO
{
    public class OptimizedWriteScheduler : WriteScheduler
    {
        private readonly Device[] _devices;

        private object _scheduleLock = new object();

        public OptimizedWriteScheduler(Device[] devices)
        {
            _devices = devices;
        }

        public Device Write(string name, byte[] data)
        {
            Device selectedDevice = _devices[0];
            lock (_scheduleLock)
            {
                for (var i = 1; i < _devices.Length; i++)
                {
                    if (selectedDevice.PendingWrites > _devices[i].PendingWrites)
                    {
                        selectedDevice = _devices[i];
                    }
                }
            }
            selectedDevice.Write(name, data);
            return selectedDevice;
        }
    }
}
