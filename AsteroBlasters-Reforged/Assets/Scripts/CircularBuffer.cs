/// <summary>
/// Class being the special kind of array, which after crossing its boundary starts counting from the beginning.
/// </summary>
/// <typeparam name="T">Type of data stored in the array</typeparam>
public class CircularBuffer<T>
{
    T[] buffer;
    int bufferSize;

    /// <summary>
    /// Special constructor creating new array with given size.
    /// </summary>
    /// <param name="bufferSize">Size of the array</param>
    public CircularBuffer(int bufferSize)
    {
        this.bufferSize = bufferSize;
        buffer = new T[bufferSize];
    }

    /// <summary>
    /// Method adding the given element to the array, if the given index crosses the boundary, it starts iterating from the beginning.
    /// </summary>
    /// <param name="item">Item to place in the array</param>
    /// <param name="index">Index at which, the item should be placed</param>
    public void Add(T item, int index) => buffer[index % bufferSize] = item;

    /// <summary>
    /// Method accessing the arry and returning item with given index. If given index crosses the boundary of the array, it starts iterating from the beginning.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T Get(int index) => buffer[index % bufferSize];

    /// <summary>
    /// Method replacing current array with the new one, of the same, fixed size.
    /// </summary>
    public void Clear() => buffer = new T[bufferSize];
}
