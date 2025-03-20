namespace Locks;

internal class KVP<K, V>
{
    public readonly K Key;
    public readonly V Val;

    public KVP(K key, V val)
    {
        this.Key = key;
        this.Val = val;
    }
}

internal class MultiValueDictionary<K, V>
{
    private readonly Dictionary<K, HashSet<V>> _dictionary = new Dictionary<K, HashSet<V>>();

    public bool Add(K key, V value)
    {
        if (!_dictionary.ContainsKey(key))
        {
            _dictionary[key] = new HashSet<V>();
        }

        return _dictionary[key].Add(value);
    }

    public IEnumerable<V> Get(K key)
    {
        if (!_dictionary.ContainsKey(key))
        {
            throw new KeyNotFoundException();
        }
        return _dictionary[key];
    }

    public IEnumerable<V> GetOrDefault(K key)
    {
        if (!_dictionary.ContainsKey(key))
        {
            return new HashSet<V>();
        }

        return _dictionary[key];
    }

    public void Remove(K key, V value)
    {
        if (!_dictionary.ContainsKey(key))
        {
            throw new KeyNotFoundException();
        }

        _dictionary[key].Remove(value);

        if (_dictionary.Count == 0)
        {
            _dictionary.Remove(key);
        }
    }

    public void Clear(K key)
    {
        if (!_dictionary.ContainsKey(key))
        {
            throw new KeyNotFoundException();
        }

        _dictionary.Remove(key);
    }


    public IEnumerable<KVP<K, V>> Flatten()
    {
        var result = new List<KVP<K, V>>();

        foreach (var kv in _dictionary)
        {
            foreach (var v in kv.Value)
            {
                result.Add(new KVP<K, V>(kv.Key, v));
            }
        }

        return result;
    }

    public IEnumerator<KVP<K, V>> GetEnumerator()
    {

        //return Flatten().GetEnumerator();

        foreach (var kv in _dictionary)
        {
            foreach (var v in kv.Value)
            {
                yield return new KVP<K, V>(kv.Key, v);
            }
        }
    }
}