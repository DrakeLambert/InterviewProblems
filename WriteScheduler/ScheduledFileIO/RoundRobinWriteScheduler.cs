namespace DrakeLambert.ScheduledFileIO
{
    public class RoundRobinWriteScheduler : WriteScheduler
    {
        private readonly Device[] _devices;

        private readonly object _scheduleLock = new object();

        private int _nextDeviceIndex = 0;

        public RoundRobinWriteScheduler(Device[] devices)
        {
            _devices = devices;
        }

        public Device Write(string name, byte[] data)
        {
            Device selectedDevice;
            lock (_scheduleLock)
            {
                selectedDevice = _devices[_nextDeviceIndex];
                _nextDeviceIndex = (_nextDeviceIndex + 1) % _devices.Length;
            }
            selectedDevice.Write(name, data);
            return selectedDevice;
        }
    }
}
