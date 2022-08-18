using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<TPriority, TItem> : IEnumerable<TItem>, IEnumerable<KeyValuePair<TPriority, TItem>>
{
    private readonly SortedDictionary<TPriority, Queue<TItem>> _storage;

    public int Count
    {
        get;
        private set;
    }

    public PriorityQueue() : this(Comparer<TPriority>.Default) { }

    public PriorityQueue(IComparer<TPriority> comparer)
    {
        _storage = new SortedDictionary<TPriority, Queue<TItem>>(comparer);
    }

    public void Enqueue(TPriority priority, TItem item)
    {
        if (!_storage.TryGetValue(priority, out var queue))
        {
            _storage[priority] = queue = new Queue<TItem>();
        }

        queue.Enqueue(item);
        Count++;
    }

    public TItem Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }

        var queue = _storage.FirstOrDefault();
        var item = queue.Value.Dequeue();

        if (queue.Value.Count == 0)
        {
            _storage.Remove(queue.Key);
        }
        Count--;
        
        return item;
    }

    public IEnumerator<KeyValuePair<TPriority, TItem>> GetEnumerator()
    {
        var items = _storage.SelectMany(pair => pair.Value, (pair, item) => new KeyValuePair<TPriority, TItem>(pair.Key, item));

        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
    {
        var items = _storage.SelectMany(pair => pair.Value);
        
        return items.GetEnumerator();
    }
}
