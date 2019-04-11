using Xunit;
using DrakeLambert.CircularArrayProblem;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace DrakeLambert.UnitTests
{
#pragma warning disable CA1707
    public class CircularArrayTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(5000)]
        public void Constructor_ValidCapacity_SetsInitialParameters(int capacity)
        {
            var circularArray = new CircularArray(capacity);

            Assert.Equal(capacity, circularArray.Capacity);
            Assert.Equal(0, circularArray.Size);
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(-1)]
        [InlineData(0)]
        public void Constructor_InvalidCapacity_ThrowsArgumentOutOfRangeException(int capacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(nameof(capacity), () => new CircularArray(capacity));
        }

        [Fact]
        public void Get_NoObjects_ReturnsNull()
        {
            var capacity = 10;
            var circularArray = new CircularArray(capacity);

            for (var i = 0; i < capacity; i++)
            {
                Assert.Null(circularArray.Get(0));
            }
        }

        [Fact]
        public void Add_MultipleObjects_IncrementsSize()
        {
            var capacity = 10;
            var circularArray = new CircularArray(capacity);

            for (var i = 1; i <= capacity; i++)
            {
                circularArray.Add(new object());
                Assert.Equal(i, circularArray.Size);
            }
        }

        [Fact]
        public void AddAndGet_MultipleObjectsOverCapacity_IncrementsSizeUntilCapacity()
        {
            var capacity = 10;
            var circularArray = new CircularArray(capacity);
            for (var i = 0; i < capacity; i++)
            {
                circularArray.Add(new object());
            }

            for (var i = 1; i <= capacity; i++)
            {
                circularArray.Add(new object());
                Assert.Equal(capacity, circularArray.Size);
            }
        }

        [Fact]
        public void AddAndGet_SingleObject_InsertsObjectAtBackOfArray()
        {
            var capacity = 10;
            var circularArray = new CircularArray(capacity);
            var testObject = new object();

            circularArray.Add(testObject);

            Assert.Same(testObject, circularArray.Get(0));
        }

        [Fact]
        public void AddAndGet_MultipleObjects_InsertsObjectsAtBackOfArray()
        {
            var capacity = 10;
            var circularArray = new CircularArray(capacity);
            var objects = Enumerable.Range(0, 10).Select(_ => new object()).ToArray();

            for (var i = 0; i < capacity; i++)
            {
                circularArray.Add(objects[i]);

                for (var j = 0; j <= i; j++)
                {
                    Assert.Same(objects[j], circularArray.Get(j));
                }
            }
        }

        [Fact]
        public void AddAndGet_MultipleObjectsOverCapacity_OverwritesOldestElements()
        {
            var capacity = 10;
            var objectCount = capacity * 2;
            var circularArray = new CircularArray(capacity);
            var addedObjects = new List<object>();

            for (var i = 0; i < 1; i++)
            {
                var testObject = Convert.ToString((char)(i + 97), new CultureInfo("en-US"));
                addedObjects.Add(testObject);
                circularArray.Add(testObject);

                var lastObjects = addedObjects.TakeLast(i + 1).ToArray();
                for (var j = 0; j < lastObjects.Length; j++)
                {
                    Assert.Same(lastObjects[j], circularArray.Get(j));
                }
            }
        }

        [Fact]
        public void IndexOf_NonExistentObject_ReturnsNegativeOne()
        {
            var capacity = 10;
            var circularArray = new CircularArray(capacity);
            var objectsToAdd = Enumerable.Range(0, capacity).Select(_ => new object()).ToArray();

            foreach (var item in objectsToAdd)
            {
                Assert.Equal(-1, circularArray.IndexOf(new object()));

                circularArray.Add(item);
            }
        }

        [Fact]
        public void IndexOf_ExistingObject_ReturnsIndex()
        {
            throw new NotImplementedException();
        }
    }
}
