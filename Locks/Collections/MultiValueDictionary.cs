namespace ThreadBound.Locks.Collections;

internal class MultiValueDictionary<K, V> where K : notnull
{
    readonly Dictionary<K, HashSet<V>> store = new();

    public bool Add(K key, V value)
    {
        if (!store.ContainsKey(key))
        {
            store[key] = new HashSet<V>();
        }
        return store[key].Add(value);
    }

    public IEnumerable<V> Get(K key)
    {
        if (!store.ContainsKey(key)) throw new KeyNotFoundException();
        return store[key];
    }

    public IEnumerable<V> GetOrDefault(K key)
    {
        if (!store.ContainsKey(key)) return new HashSet<V>();
        return store[key];
    }

    public void Remove(K key, V value)
    {
        if (!store.ContainsKey(key)) throw new KeyNotFoundException();
        store[key].Remove(value);
        if (store.Count == 0)
        {
            store.Remove(key);
        }
    }

    public void Clear(K key)
    {
        if (!store.ContainsKey(key)) throw new KeyNotFoundException();
        store.Remove(key);
    }

    public IEnumerable<KeyValuePair<K, V>> Flatten()
    {
        foreach (var kv in store)
        {
            foreach (var val in kv.Value)
            {
                yield return new KeyValuePair<K, V>(kv.Key, val);
            }
        }
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        foreach (var kv in store)
        {
            foreach (var val in kv.Value)
            {
                yield return new KeyValuePair<K, V>(kv.Key, val);
            }
        }
    }
}