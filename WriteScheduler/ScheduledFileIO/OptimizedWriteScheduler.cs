namespace DrakeLambert.ScheduledFileIO
{
    public class OptimizedWriteScheduler : WriteScheduler
    {
        private readonly Device[] _devices;

        private readonly object _scheduleLock = new object();

        public OptimizedWriteScheduler(Device[] devices)
        {
            _devices = devices;
        }

        public Device Write(string name, byte[] data)
        {
            Device selectedDevice = _devices[0];
            lock (_scheduleLock)
            {
                foreach (var device in _devices)
                {
                    if (device.PendingWrites == 0)
                    {
                        selectedDevice = device;
                        break;
                    }
                    if (device.PendingWrites < selectedDevice.PendingWrites)
                    {
                        selectedDevice = device;
                    }
                }
            }
            selectedDevice.Write(name, data);
            return selectedDevice;
        }
    }
}
