using Xunit;
using DrakeLambert.CircularArrayProblem;

namespace DrakeLambert.CircularArrayTests
{
    public class CircularArrayTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(5000)]
        public void Constructor_GivenCapacity_SetsCapacity(int capacity)
        {
            var circularArray = new CircularArray(capacity);

            Assert.Equal(capacity, circularArray.Capacity);
        }

        [Fact]
        public void Add_GivenSingleObject_InsertsElementAtBackOfArray()
        {
            var capacity = 10;
            var circularArray = new CircularArray(capacity);
            var testObject = new object();

            circularArray.Add(testObject);


            Assert.Same(testObject, circularArray.Get(0));
        }
    }
}
