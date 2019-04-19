using System.Linq;
using System.Threading.Tasks;
using DrakeLambert.CircularArrayProblem;
using Xunit;

namespace DrakeLambert.UnitTests
{
    public class CircularArrayConcurrentTests
    {
        [Fact]
        public void Add_ManyObjectsOnMultipleThreads_AddsAllObjects()
        {
            var capacity = 1000;
            var circularArray = new CircularArray<object>(capacity);
            var testObjects = Enumerable.Range(0, capacity).Select(_ => new object()).ToArray();

            Parallel.ForEach(testObjects, testObject =>
            {
                circularArray.Add(testObject);
            });

            Assert.All(testObjects, testObject =>
            {
                Assert.NotEqual(-1, circularArray.IndexOf(testObject));
            });
        }
    }
}
