using System.Linq;
using DrakeLambert.ScheduledFileIO;
using Moq;
using Xunit;

namespace DrakeLambert.ScheduledFileIOTests
{
    public class RoundRobinWriteSchedulerTests
    {
        [Fact]
        public void Write_WithOneDevice_WritesToDevice()
        {
            var fileName = "testName";
            var data = new byte[100];
            var deviceMock = new Mock<Device>();
            var scheduler = new RoundRobinWriteScheduler(new[] { deviceMock.Object });

            var selectedDevice = scheduler.Write(fileName, data);

            deviceMock.Verify(x => x.Write(fileName, data));
            Assert.Same(deviceMock.Object, selectedDevice);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(20)]
        public void Write_WithMultipleDevice_WritesToEachDeviceEqually(int deviceCount)
        {
            var filesPerDevice = 4;
            var fileName = "testName";
            var data = new byte[100];
            var deviceMocks = Enumerable.Range(0, deviceCount)
                .Select(_ => new Mock<Device>()).ToList();
            var devices = deviceMocks.Select(d => d.Object).ToArray();
            var scheduler = new RoundRobinWriteScheduler(devices);

            var totalWriteCount = filesPerDevice * deviceCount;
            for (var i = 0; i < totalWriteCount; i++)
            {
                scheduler.Write(fileName, data);
            }

            foreach (var deviceMock in deviceMocks)
            {
                System.Console.WriteLine("===================" + deviceMock.Invocations.Count());
                deviceMock.Verify(x => x.Write(fileName, data), Times.Exactly(filesPerDevice));
            }
        }
    }

    public class OptimizedWriteSchedulerTests
    {
        [Fact]
        public void Write_WithOneDevice_WritesToDevice()
        {
            var fileName = "testName";
            var data = new byte[100];
            var deviceMock = new Mock<Device>();
            var scheduler = new OptimizedWriteScheduler(new[] { deviceMock.Object });

            var selectedDevice = scheduler.Write(fileName, data);

            deviceMock.Verify(x => x.Write(fileName, data));
            Assert.Same(deviceMock.Object, selectedDevice);
        }

        [Fact]
        public void Write_WithBusyDevices_WritesToDeviceWithLeastPending()
        {
            var fileName = "testName";
            var data = new byte[100];
            var deviceMock1 = new Mock<Device>();
            deviceMock1.SetupGet(x => x.PendingWrites).Returns(2);
            var deviceMock2 = new Mock<Device>();
            deviceMock2.SetupGet(x => x.PendingWrites).Returns(0);
            var deviceMock3 = new Mock<Device>();
            deviceMock3.SetupGet(x => x.PendingWrites).Returns(1);
            var scheduler = new OptimizedWriteScheduler(new[] { deviceMock2.Object, deviceMock2.Object, deviceMock3.Object });

            var selectedDevice = scheduler.Write(fileName, data);

            deviceMock2.Verify(x => x.Write(fileName, data));
            Assert.Same(deviceMock2.Object, selectedDevice);
        }

        [Fact]
        public void Write_WithFreeDevices_WritesToFirstFreeDevice()
        {
            var fileName = "testName";
            var data = new byte[100];
            var deviceMock1 = new Mock<Device>();
            deviceMock1.SetupGet(x => x.PendingWrites).Returns(2);
            var deviceMock2 = new Mock<Device>();
            deviceMock2.SetupGet(x => x.PendingWrites).Returns(1);
            var deviceMock3 = new Mock<Device>();
            deviceMock3.SetupGet(x => x.PendingWrites).Returns(0);
            var deviceMock4 = new Mock<Device>();
            deviceMock4.SetupGet(x => x.PendingWrites).Returns(0);
            var scheduler = new OptimizedWriteScheduler(new[] { deviceMock2.Object, deviceMock2.Object, deviceMock3.Object });

            var selectedDevice = scheduler.Write(fileName, data);

            deviceMock3.Verify(x => x.Write(fileName, data));
            Assert.Same(deviceMock3.Object, selectedDevice);
        }
    }
}
