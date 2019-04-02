using System.Linq;
using System.Threading;

namespace DrakeLambert.ScheduledFileIO
{
    public class OptimizedWriteScheduler : WriteScheduler
    {
        private readonly Device[] _devices;

        private readonly object _scheduleLock = new object();

        private int _nextDeviceIndex = 0;

        public OptimizedWriteScheduler(Device[] devices)
        {
            _devices = devices;
        }

        public Device Write(string name, byte[] data)
        {
            Device selectedDevice;
            lock (_scheduleLock)
            {
                selectedDevice = _devices[_nextDeviceIndex];
                // System.Console.WriteLine(_devices.Select(d => d.PendingWrites.ToString()).Aggregate((xs, x) => xs + ", " + x));
                for (var i = (_nextDeviceIndex + 1) % _devices.Length; i != _nextDeviceIndex; i = (i + 1) % _devices.Length)
                {
                    if (_devices[i].PendingWrites == 0)
                    {
                        selectedDevice = _devices[i];
                        break;
                    }
                    if (_devices[i].PendingWrites < selectedDevice.PendingWrites)
                    {
                        selectedDevice = _devices[i];
                    }
                }
                _nextDeviceIndex = (_nextDeviceIndex + 1) % _devices.Length;
            }
            selectedDevice.Write(name, data);
            return selectedDevice;
        }
    }
}
