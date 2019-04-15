using System;

namespace DrakeLambert.CircularArrayProblem
{
    public class CircularArray
    {
        private readonly object[] _objects;

        private readonly int _capacity;

        private int _headIndex = 0;

        private int _size = 0;

        /// <summary>
        /// Constructs a new CircularArray with the given capacity.
        /// </summary>
        /// <param name="capacity">
        /// The maximum number of elements that the CircularArray can hold.
        /// The capacity cannot be changed.
        /// </param>
        public CircularArray(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 1.");
            }
            _capacity = capacity;
            _objects = new object[_capacity];
        }

        /// <summary>
        /// Returns the fixed capacity of the CircularArray.
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// Returns the number of elements in the CircularArray.
        /// </summary>
        public int Size => _size;

        /// <summary>
        /// Returns the element at the given relative index. If the given item is not
        /// available, this method will return null. The relative index starts at 0
        /// for the oldest element in the CircularArray.
        /// </summary>
        /// <param name="index">The relative index of the element.</param>
        /// <returns>value</returns>
        public object Get(int index)
        {
            if (index < 0 || index >= _capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be greater than 0 and less than the capacity of the CircularArray.");
            }
            if (index >= _size)
            {
                return null;
            }
            return _objects[RealIndex(index)];
        }

        /// <summary>
        /// Adds an object to the back of the CircularArray.
        /// </summary>
        /// <param name="o">The new object</param>
        public void Add(object o)
        {
            _objects[RealIndex(_size)] = o;
            if (_size < _capacity)
            {
                _size++;
            }
            else
            {
                _headIndex = RealIndex(1);
            }
        }

        /// <summary>
        /// Returns the relative index of the given element if it is in the
        /// CircularArray or -1 if the element is not found.
        /// </summary>
        /// <param name="o">The object to find in the array</param>
        /// <returns>The index of the given element</returns>
        public int IndexOf(object o)
        {
            var index = -1;
            for (var i = 0; i < _size; i++)
            {
                if (_objects[RealIndex(i)].Equals(o))
                {
                    return i;
                }
            }
            return index;
        }

        private int RealIndex(int relativeIndex)
        {
            return (_headIndex + relativeIndex) % _capacity;
        }

        private int RelativeIndex(int realIndex)
        {
            var currentHeadIndex = _headIndex;
            if (realIndex >= currentHeadIndex)
            {
                return realIndex - currentHeadIndex;
            }
            else
            {
                return realIndex - currentHeadIndex + _capacity;
            }
        }
    }
}
