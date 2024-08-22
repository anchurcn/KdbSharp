/*
 Copyright (C) 2024 Anchur
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/
using System.Collections;

namespace KdbSharp.Types;

public class KSimpleDictionary : IIndexable, IEnumerable<KeyValuePair<object, object>>
{
    public KSimpleDictionary(Array keys, Array values)
    {
        // Null check
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }
        if (keys.Length != values.Length)
        {
            throw new ArgumentException("Keys and values must be same length");
        }
        Keys = keys;
        Values = values;
    }

    public object this[int index] => throw new NotImplementedException();
    public object this[object key] => IndexValueByKey(key);

    private object IndexValueByKey(object key)
    {
        var index = Array.IndexOf(Keys, key);
        if (index == -1)
        {
            throw new KeyNotFoundException();
        }
        return Values.GetValue(index)!;
    }

    public Array Keys { get; }
    public Array Values { get; }

    public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
    {
        for (int i = 0; i < Keys.Length; i++)
        {
            yield return new KeyValuePair<object, object>(Keys.GetValue(i)!, Values.GetValue(i)!);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // TODO: move to extension method
    public Dictionary<TKey,TValue> ToDictionary<TKey, TValue>() where TKey: notnull
    {
        var dict = new Dictionary<TKey, TValue>();
        for (int i = 0; i < Keys.Length; i++)
        {
            dict.Add((TKey)Keys.GetValue(i)!, (TValue)Values.GetValue(i)!);
        }
        return dict;
    }
}
