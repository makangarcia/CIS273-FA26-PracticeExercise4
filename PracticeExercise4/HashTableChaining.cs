using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PracticeExercise4;

public class HashTableChaining<K, V> : IHashTable<K, V>
{
    List<HashEntry<K, V>>[] entryListArray;

    private int initialCapacity = 13;

    public readonly double MAX_LOAD_FACTOR = 0.6;

    public HashTableChaining()
    {
        // init all entry lists
        entryListArray = new List<HashEntry<K, V>>[initialCapacity];

        for(int i=0; i < entryListArray.Length; i++)
        {
            entryListArray[i] = new List<HashEntry<K, V>>();
        }
        
    }

    private int count;
    public int Count => count;

    public double LoadFactor => (double)count / entryListArray.Length;

    public bool Add(K key, V value)
    {
        if (key is null)
        {
            throw new NoNullAllowedException();
        }
        
        // resize if needed
        if(LoadFactor > MAX_LOAD_FACTOR)
        {
            Resize();
        }

        // get the hash of the key
        int hash = Hash(key);

        // find the correct array index / element
        int index = hash % entryListArray.Length;

        // check if key is already present
        foreach(var entry in entryListArray[index])
        {
            if (entry.Key.Equals(key))
            {
                entry.Value = value;
                return true;
            }
        }

        // add the new key/value pair to the correct list
       entryListArray[index].Add(new HashEntry<K, V>(key, value));
       count++;

        return false;
    }

    public bool ContainsKey(K key)
    {
        foreach (var entry in GetBucket(key))
        {
            if (entry.Key.Equals(key))
                return true;
        }
        return false;
    }

    public bool ContainsValue(V value)
    {
        foreach (var list in entryListArray)
        {
            foreach (var entry in list)
            {
                if(EqualityComparer<V>.Default.Equals(entry.Value, value))
                    return true;
            }
        }
        return false;
    }

    public V Get(K key)
    {
        // hash the key
        // find the correct index of the list
        // find the entry with the given key (in the correct list)
        foreach (var entry in GetBucket(key))
        {
            if (entry.Key.Equals(key))
                return entry.Value;
        }
        
        throw new KeyNotFoundException();
    }

    public List<K> GetKeys()
    {
        var keys = new List<K>();

        for(int i=0; i < entryListArray.Length; i++)
        {
            foreach(var entry in entryListArray[i])
            {
                keys.Add(entry.Key);
            }
        }
        return keys;
    }

    public List<V> GetValues()
    {
        var values = new List<V>();

        foreach (var list in entryListArray)
        {
            foreach (var entry in list)
            {
                values.Add(entry.Value);
            }
        }
        return values;
    }

    public bool Remove(K key)
    {
        var bucket = GetBucket(key);

        for (int i=0; i < bucket.Count; i++)
        {
            if (bucket[i].Key.Equals(key))
            {
                bucket.RemoveAt(i);
                count--;
                return true;
            }
        }
        return false;
    }


    private int Hash(K key)
    {
        // clear the sign bit (Math.Abs throws on int.MinValue)
        return key.GetHashCode() & 0x7FFFFFFF;
    }


    private List<HashEntry<K,V>> GetBucket(K key)
    {
        if (key is null)
            throw new NoNullAllowedException();
        
        int index = Hash(key) % entryListArray.Length;
        return entryListArray[index];
    }
    private void Resize()
    {
        int newSize = NextPrime(2 * entryListArray.Length);
        var newEntryListArray = new List<HashEntry<K, V>>[newSize];
        var oldEntryListArray = entryListArray;

        // initialize the new entry list array 
        for (int i = 0; i < newEntryListArray.Length; i++)
        {
            newEntryListArray[i] = new List<HashEntry<K, V>>();
        }

        entryListArray = newEntryListArray;
        count = 0;

        // re-add all the old buckets into the new array
        foreach (var list in oldEntryListArray)
        {
            foreach (var entry in list)
            {
                Add(entry.Key, entry.Value);
            }
        }

    }
    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n < 4) return true;           // 2 and 3
        if (n % 2 == 0) return false;

        for (int d = 3; (long)d * d <= n; d += 2)
        {
            if (n % d == 0) return false;
        }
        return true;
    }

    private static int NextPrime(int n)
    {
        if (n <= 2) return 2;
        int candidate = (n % 2 == 0) ? n + 1 : n;   // only odd numbers can be prime past 2
        while (!IsPrime(candidate))
        {
            candidate += 2;
        }
        return candidate;
    }
}


