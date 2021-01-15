// Decompiled with JetBrains decompiler
// Type: MsgPack.MessagePackObjectDictionary
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace MsgPack
{
  [DebuggerTypeProxy(typeof (DictionaryDebuggerProxy<,>))]
  [Serializable]
  public class MessagePackObjectDictionary : IDictionary<MessagePackObject, MessagePackObject>, ICollection<KeyValuePair<MessagePackObject, MessagePackObject>>, IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>, IDictionary, ICollection, IEnumerable
  {
    private const int Threashold = 10;
    private const int ListInitialCapacity = 10;
    private const int DictionaryInitialCapacity = 20;
    private List<MessagePackObject> _keys;
    private List<MessagePackObject> _values;
    private Dictionary<MessagePackObject, MessagePackObject> _dictionary;
    private int _version;
    private bool _isFrozen;

    public bool IsFrozen
    {
      get
      {
        return this._isFrozen;
      }
    }

    public int Count
    {
      get
      {
        return this._dictionary != null ? this._dictionary.Count : this._keys.Count;
      }
    }

    public MessagePackObject this[MessagePackObject key]
    {
      get
      {
        if (key.IsNil)
          MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
        MessagePackObject messagePackObject;
        if (!this.TryGetValue(key, out messagePackObject))
          throw new KeyNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Key '{0}'({1} type) does not exist in this dictionary.", (object) key, (object) key.UnderlyingType));
        return messagePackObject;
      }
      set
      {
        if (key.IsNil)
          MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
        this.VerifyIsNotFrozen();
        this.AddCore(key, value, true);
      }
    }

    public MessagePackObjectDictionary.KeySet Keys
    {
      get
      {
        return new MessagePackObjectDictionary.KeySet(this);
      }
    }

    public MessagePackObjectDictionary.ValueCollection Values
    {
      get
      {
        return new MessagePackObjectDictionary.ValueCollection(this);
      }
    }

    ICollection<MessagePackObject> IDictionary<MessagePackObject, MessagePackObject>.Keys
    {
      get
      {
        return (ICollection<MessagePackObject>) this.Keys;
      }
    }

    ICollection<MessagePackObject> IDictionary<MessagePackObject, MessagePackObject>.Values
    {
      get
      {
        return (ICollection<MessagePackObject>) this.Values;
      }
    }

    bool ICollection<KeyValuePair<MessagePackObject, MessagePackObject>>.IsReadOnly
    {
      get
      {
        return this.IsFrozen;
      }
    }

    bool IDictionary.IsFixedSize
    {
      get
      {
        return this.IsFrozen;
      }
    }

    bool IDictionary.IsReadOnly
    {
      get
      {
        return this.IsFrozen;
      }
    }

    ICollection IDictionary.Keys
    {
      get
      {
        return (ICollection) this._keys;
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        return (ICollection) this.Values;
      }
    }

    object IDictionary.this[object key]
    {
      get
      {
        if (key == null)
          throw new ArgumentNullException(nameof (key));
        MessagePackObject key1 = MessagePackObjectDictionary.ValidateObjectArgument(key, nameof (key));
        if (key1.IsNil)
          MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
        MessagePackObject messagePackObject;
        return !this.TryGetValue(key1, out messagePackObject) ? (object) null : (object) messagePackObject;
      }
      set
      {
        if (key == null)
          throw new ArgumentNullException(nameof (key));
        this.VerifyIsNotFrozen();
        MessagePackObject index = MessagePackObjectDictionary.ValidateObjectArgument(key, nameof (key));
        if (index.IsNil)
          MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
        this[index] = MessagePackObjectDictionary.ValidateObjectArgument(value, nameof (value));
      }
    }

    bool ICollection.IsSynchronized
    {
      get
      {
        return false;
      }
    }

    object ICollection.SyncRoot
    {
      get
      {
        return (object) this;
      }
    }

    public MessagePackObjectDictionary()
    {
      this._keys = new List<MessagePackObject>(10);
      this._values = new List<MessagePackObject>(10);
    }

    public MessagePackObjectDictionary(int initialCapacity)
    {
      if (initialCapacity < 0)
        throw new ArgumentOutOfRangeException(nameof (initialCapacity));
      if (initialCapacity <= 10)
      {
        this._keys = new List<MessagePackObject>(initialCapacity);
        this._values = new List<MessagePackObject>(initialCapacity);
      }
      else
        this._dictionary = new Dictionary<MessagePackObject, MessagePackObject>(initialCapacity, (IEqualityComparer<MessagePackObject>) MessagePackObjectEqualityComparer.Instance);
    }

    public MessagePackObjectDictionary(
      IDictionary<MessagePackObject, MessagePackObject> dictionary)
    {
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      if (dictionary.Count <= 10)
      {
        this._keys = new List<MessagePackObject>(dictionary.Count);
        this._values = new List<MessagePackObject>(dictionary.Count);
      }
      else
        this._dictionary = new Dictionary<MessagePackObject, MessagePackObject>(dictionary.Count, (IEqualityComparer<MessagePackObject>) MessagePackObjectEqualityComparer.Instance);
      try
      {
        foreach (KeyValuePair<MessagePackObject, MessagePackObject> keyValuePair in (IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>) dictionary)
          this.AddCore(keyValuePair.Key, keyValuePair.Value, false);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException("Failed to copy specified dictionary.", nameof (dictionary), (Exception) ex);
      }
    }

    private static void ThrowKeyNotNilException(string parameterName)
    {
      throw new ArgumentNullException(parameterName, "Key cannot be nil.");
    }

    private static void ThrowDuplicatedKeyException(MessagePackObject key, string parameterName)
    {
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Key '{0}'({1} type) already exists in this dictionary.", (object) key, (object) key.UnderlyingType), parameterName);
    }

    private void VerifyIsNotFrozen()
    {
      if (this._isFrozen)
        throw new InvalidOperationException("This dictionary is frozen.");
    }

    [Conditional("DEBUG")]
    private void AssertInvariant()
    {
      Dictionary<MessagePackObject, MessagePackObject> dictionary = this._dictionary;
    }

    private static MessagePackObject ValidateObjectArgument(
      object obj,
      string parameterName)
    {
      MessagePackObject? nullable = MessagePackObjectDictionary.TryValidateObjectArgument(obj);
      if (!nullable.HasValue)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot convert '{1}' to {0}.", (object) typeof (MessagePackObject).Name, (object) obj.GetType()), parameterName);
      return nullable.Value;
    }

    private static MessagePackObject? TryValidateObjectArgument(object value)
    {
      switch (value)
      {
        case null:
          return new MessagePackObject?(MessagePackObject.Nil);
        case MessagePackObject messagePackObject:
          return new MessagePackObject?(messagePackObject);
        case byte[] numArray:
          return new MessagePackObject?((MessagePackObject) numArray);
        case string str:
          return new MessagePackObject?((MessagePackObject) str);
        case MessagePackString messagePackString:
          return new MessagePackObject?(new MessagePackObject(messagePackString));
        default:
          switch (Type.GetTypeCode(value.GetType()))
          {
            case TypeCode.Empty:
            case TypeCode.DBNull:
              return new MessagePackObject?(MessagePackObject.Nil);
            case TypeCode.Boolean:
              return new MessagePackObject?((MessagePackObject) (bool) value);
            case TypeCode.SByte:
              return new MessagePackObject?((MessagePackObject) (sbyte) value);
            case TypeCode.Byte:
              return new MessagePackObject?((MessagePackObject) (byte) value);
            case TypeCode.Int16:
              return new MessagePackObject?((MessagePackObject) (short) value);
            case TypeCode.UInt16:
              return new MessagePackObject?((MessagePackObject) (ushort) value);
            case TypeCode.Int32:
              return new MessagePackObject?((MessagePackObject) (int) value);
            case TypeCode.UInt32:
              return new MessagePackObject?((MessagePackObject) (uint) value);
            case TypeCode.Int64:
              return new MessagePackObject?((MessagePackObject) (long) value);
            case TypeCode.UInt64:
              return new MessagePackObject?((MessagePackObject) (ulong) value);
            case TypeCode.Single:
              return new MessagePackObject?((MessagePackObject) (float) value);
            case TypeCode.Double:
              return new MessagePackObject?((MessagePackObject) (double) value);
            case TypeCode.DateTime:
              return new MessagePackObject?((MessagePackObject) MessagePackConvert.FromDateTime((DateTime) value));
            case TypeCode.String:
              return new MessagePackObject?((MessagePackObject) value.ToString());
            default:
              return new MessagePackObject?();
          }
      }
    }

    public bool ContainsKey(MessagePackObject key)
    {
      if (key.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
      return this._dictionary != null ? this._dictionary.ContainsKey(key) : this._keys.Contains<MessagePackObject>(key, (IEqualityComparer<MessagePackObject>) MessagePackObjectEqualityComparer.Instance);
    }

    public bool ContainsValue(MessagePackObject value)
    {
      return this._dictionary != null ? this._dictionary.ContainsValue(value) : this._values.Contains<MessagePackObject>(value, (IEqualityComparer<MessagePackObject>) MessagePackObjectEqualityComparer.Instance);
    }

    bool ICollection<KeyValuePair<MessagePackObject, MessagePackObject>>.Contains(
      KeyValuePair<MessagePackObject, MessagePackObject> item)
    {
      MessagePackObject messagePackObject;
      return this.TryGetValue(item.Key, out messagePackObject) && item.Value == messagePackObject;
    }

    bool IDictionary.Contains(object key)
    {
      if (key == null)
        return false;
      MessagePackObject? nullable = MessagePackObjectDictionary.TryValidateObjectArgument(key);
      return !nullable.GetValueOrDefault().IsNil && this.ContainsKey(nullable.GetValueOrDefault());
    }

    public bool TryGetValue(MessagePackObject key, out MessagePackObject value)
    {
      if (key.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
      if (this._dictionary != null)
        return this._dictionary.TryGetValue(key, out value);
      int index = this._keys.FindIndex((Predicate<MessagePackObject>) (item => item == key));
      if (index < 0)
      {
        value = MessagePackObject.Nil;
        return false;
      }
      value = this._values[index];
      return true;
    }

    public void Add(MessagePackObject key, MessagePackObject value)
    {
      if (key.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
      this.VerifyIsNotFrozen();
      this.AddCore(key, value, false);
    }

    private void AddCore(MessagePackObject key, MessagePackObject value, bool allowOverwrite)
    {
      if (this._dictionary == null)
      {
        if (this._keys.Count < 10)
        {
          int index = this._keys.FindIndex((Predicate<MessagePackObject>) (item => item == key));
          if (index < 0)
          {
            this._keys.Add(key);
            this._values.Add(value);
          }
          else
          {
            if (!allowOverwrite)
              MessagePackObjectDictionary.ThrowDuplicatedKeyException(key, nameof (key));
            this._values[index] = value;
          }
          ++this._version;
          return;
        }
        if (this._keys.Count == 10 && allowOverwrite)
        {
          int index = this._keys.FindIndex((Predicate<MessagePackObject>) (item => item == key));
          if (0 <= index)
          {
            this._values[index] = value;
            ++this._version;
            return;
          }
        }
        this._dictionary = new Dictionary<MessagePackObject, MessagePackObject>(20, (IEqualityComparer<MessagePackObject>) MessagePackObjectEqualityComparer.Instance);
        for (int index = 0; index < this._keys.Count; ++index)
          this._dictionary.Add(this._keys[index], this._values[index]);
        this._keys = (List<MessagePackObject>) null;
        this._values = (List<MessagePackObject>) null;
      }
      if (allowOverwrite)
      {
        this._dictionary[key] = value;
      }
      else
      {
        try
        {
          this._dictionary.Add(key, value);
        }
        catch (ArgumentException ex)
        {
          MessagePackObjectDictionary.ThrowDuplicatedKeyException(key, nameof (key));
        }
      }
      ++this._version;
    }

    void ICollection<KeyValuePair<MessagePackObject, MessagePackObject>>.Add(
      KeyValuePair<MessagePackObject, MessagePackObject> item)
    {
      if (item.Key.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException("key");
      this.VerifyIsNotFrozen();
      this.AddCore(item.Key, item.Value, false);
    }

    void IDictionary.Add(object key, object value)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.VerifyIsNotFrozen();
      MessagePackObject key1 = MessagePackObjectDictionary.ValidateObjectArgument(key, nameof (key));
      if (key1.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
      this.AddCore(key1, MessagePackObjectDictionary.ValidateObjectArgument(value, nameof (value)), false);
    }

    public bool Remove(MessagePackObject key)
    {
      if (key.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
      this.VerifyIsNotFrozen();
      return this.RemoveCore(key, new MessagePackObject(), false);
    }

    private bool RemoveCore(MessagePackObject key, MessagePackObject value, bool checkValue)
    {
      if (this._dictionary == null)
      {
        int index = this._keys.FindIndex((Predicate<MessagePackObject>) (item => item == key));
        if (index < 0 || checkValue && this._values[index] != value)
          return false;
        this._keys.RemoveAt(index);
        this._values.RemoveAt(index);
      }
      else if (checkValue)
      {
        if (!((ICollection<KeyValuePair<MessagePackObject, MessagePackObject>>) this._dictionary).Remove(new KeyValuePair<MessagePackObject, MessagePackObject>(key, value)))
          return false;
      }
      else if (!this._dictionary.Remove(key))
        return false;
      ++this._version;
      return true;
    }

    bool ICollection<KeyValuePair<MessagePackObject, MessagePackObject>>.Remove(
      KeyValuePair<MessagePackObject, MessagePackObject> item)
    {
      if (item.Key.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException("key");
      this.VerifyIsNotFrozen();
      return this.RemoveCore(item.Key, item.Value, true);
    }

    void IDictionary.Remove(object key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.VerifyIsNotFrozen();
      MessagePackObject key1 = MessagePackObjectDictionary.ValidateObjectArgument(key, nameof (key));
      if (key1.IsNil)
        MessagePackObjectDictionary.ThrowKeyNotNilException(nameof (key));
      this.RemoveCore(key1, new MessagePackObject(), false);
    }

    public void Clear()
    {
      this.VerifyIsNotFrozen();
      if (this._dictionary == null)
      {
        this._keys.Clear();
        this._values.Clear();
      }
      else
        this._dictionary.Clear();
      ++this._version;
    }

    void ICollection<KeyValuePair<MessagePackObject, MessagePackObject>>.CopyTo(
      KeyValuePair<MessagePackObject, MessagePackObject>[] array,
      int arrayIndex)
    {
      CollectionOperation.CopyTo<KeyValuePair<MessagePackObject, MessagePackObject>>((IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>) this, this.Count, 0, array, arrayIndex, this.Count);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array is DictionaryEntry[] array1)
      {
        int count = this.Count;
        int arrayIndex = index;
        int length = array.Length;
        Func<KeyValuePair<MessagePackObject, MessagePackObject>, DictionaryEntry> converter = (Func<KeyValuePair<MessagePackObject, MessagePackObject>, DictionaryEntry>) (kv => new DictionaryEntry((object) kv.Key, (object) kv.Value));
        CollectionOperation.CopyTo<KeyValuePair<MessagePackObject, MessagePackObject>, DictionaryEntry>((IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>) this, count, 0, array1, arrayIndex, length, converter);
      }
      else
        CollectionOperation.CopyTo<KeyValuePair<MessagePackObject, MessagePackObject>>((IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>) this, this.Count, array, index);
    }

    public MessagePackObjectDictionary.Enumerator GetEnumerator()
    {
      return new MessagePackObjectDictionary.Enumerator(this);
    }

    IEnumerator<KeyValuePair<MessagePackObject, MessagePackObject>> IEnumerable<KeyValuePair<MessagePackObject, MessagePackObject>>.GetEnumerator()
    {
      return (IEnumerator<KeyValuePair<MessagePackObject, MessagePackObject>>) this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (IDictionaryEnumerator) new MessagePackObjectDictionary.DictionaryEnumerator(this);
    }

    public MessagePackObjectDictionary Freeze()
    {
      this._isFrozen = true;
      return this;
    }

    public MessagePackObjectDictionary AsFrozen()
    {
      return new MessagePackObjectDictionary((IDictionary<MessagePackObject, MessagePackObject>) this).Freeze();
    }

    public struct Enumerator : IEnumerator<KeyValuePair<MessagePackObject, MessagePackObject>>, IDisposable, IDictionaryEnumerator, IEnumerator
    {
      private const int BeforeHead = -1;
      private const int IsDictionary = -2;
      private const int End = -3;
      private readonly MessagePackObjectDictionary _underlying;
      private Dictionary<MessagePackObject, MessagePackObject>.Enumerator _enumerator;
      private KeyValuePair<MessagePackObject, MessagePackObject> _current;
      private int _position;
      private int _initialVersion;

      public KeyValuePair<MessagePackObject, MessagePackObject> Current
      {
        get
        {
          this.VerifyVersion();
          return this._current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.GetCurrentStrict();
        }
      }

      DictionaryEntry IDictionaryEnumerator.Entry
      {
        get
        {
          KeyValuePair<MessagePackObject, MessagePackObject> currentStrict = this.GetCurrentStrict();
          return new DictionaryEntry((object) currentStrict.Key, (object) currentStrict.Value);
        }
      }

      object IDictionaryEnumerator.Key
      {
        get
        {
          return (object) this.GetCurrentStrict().Key;
        }
      }

      object IDictionaryEnumerator.Value
      {
        get
        {
          return (object) this.GetCurrentStrict().Value;
        }
      }

      internal KeyValuePair<MessagePackObject, MessagePackObject> GetCurrentStrict()
      {
        this.VerifyVersion();
        if (this._position == -1 || this._position == -3)
          throw new InvalidOperationException("The enumerator is positioned before the first element of the collection or after the last element.");
        return this._current;
      }

      internal unsafe Enumerator(MessagePackObjectDictionary dictionary)
        : this()
      {
        *(MessagePackObjectDictionary.Enumerator*) ref this = new MessagePackObjectDictionary.Enumerator();
        this._underlying = dictionary;
        this.ResetCore();
      }

      internal void VerifyVersion()
      {
        if (this._underlying != null && this._underlying._version != this._initialVersion)
          throw new InvalidOperationException("The collection was modified after the enumerator was created.");
      }

      public void Dispose()
      {
        this._enumerator.Dispose();
      }

      public bool MoveNext()
      {
        if (this._position == -3)
          return false;
        if (this._position == -2)
        {
          if (!this._enumerator.MoveNext())
            return false;
          this._current = this._enumerator.Current;
          return true;
        }
        if (this._position == -1)
        {
          if (this._underlying._keys.Count == 0)
          {
            this._position = -3;
            return false;
          }
          this._position = 0;
        }
        else
          ++this._position;
        if (this._position == this._underlying._keys.Count)
        {
          this._position = -3;
          return false;
        }
        this._current = new KeyValuePair<MessagePackObject, MessagePackObject>(this._underlying._keys[this._position], this._underlying._values[this._position]);
        return true;
      }

      void IEnumerator.Reset()
      {
        this.ResetCore();
      }

      internal void ResetCore()
      {
        this._initialVersion = this._underlying._version;
        this._current = new KeyValuePair<MessagePackObject, MessagePackObject>();
        this._initialVersion = this._underlying._version;
        if (this._underlying._dictionary != null)
        {
          this._enumerator = this._underlying._dictionary.GetEnumerator();
          this._position = -2;
        }
        else
          this._position = -1;
      }
    }

    private struct DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
    {
      private IDictionaryEnumerator _underlying;

      public object Current
      {
        get
        {
          return (object) this._underlying.Entry;
        }
      }

      public DictionaryEntry Entry
      {
        get
        {
          return this._underlying.Entry;
        }
      }

      public object Key
      {
        get
        {
          return this.Entry.Key;
        }
      }

      public object Value
      {
        get
        {
          return this.Entry.Value;
        }
      }

      internal DictionaryEnumerator(MessagePackObjectDictionary dictionary)
        : this()
      {
        this._underlying = (IDictionaryEnumerator) new MessagePackObjectDictionary.Enumerator(dictionary);
      }

      public bool MoveNext()
      {
        return this._underlying.MoveNext();
      }

      void IEnumerator.Reset()
      {
        this._underlying.Reset();
      }
    }

    [DebuggerTypeProxy(typeof (CollectionDebuggerProxy<>))]
    [DebuggerDisplay("Count={Count}")]
    [Serializable]
    public sealed class KeySet : ISet<MessagePackObject>, ICollection<MessagePackObject>, IEnumerable<MessagePackObject>, ICollection, IEnumerable
    {
      private readonly MessagePackObjectDictionary _dictionary;

      public int Count
      {
        get
        {
          return this._dictionary.Count;
        }
      }

      bool ICollection<MessagePackObject>.IsReadOnly
      {
        get
        {
          return true;
        }
      }

      bool ICollection.IsSynchronized
      {
        get
        {
          return false;
        }
      }

      object ICollection.SyncRoot
      {
        get
        {
          return (object) this;
        }
      }

      internal KeySet(MessagePackObjectDictionary dictionary)
      {
        this._dictionary = dictionary;
      }

      public void CopyTo(MessagePackObject[] array)
      {
        if (array == null)
          throw new ArgumentNullException(nameof (array));
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, 0, array, 0, this.Count);
      }

      public void CopyTo(MessagePackObject[] array, int arrayIndex)
      {
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, 0, array, arrayIndex, this.Count);
      }

      public void CopyTo(int index, MessagePackObject[] array, int arrayIndex, int count)
      {
        if (array == null)
          throw new ArgumentNullException(nameof (array));
        if (index < 0)
          throw new ArgumentOutOfRangeException(nameof (index));
        if (0 < this.Count && this.Count <= index)
          throw new ArgumentException("Specified array is too small to complete copy operation.", nameof (array));
        if (arrayIndex < 0)
          throw new ArgumentOutOfRangeException(nameof (arrayIndex));
        if (count < 0)
          throw new ArgumentOutOfRangeException(nameof (count));
        if (array.Length - count <= arrayIndex)
          throw new ArgumentException("Specified array is too small to complete copy operation.", nameof (array));
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, index, array, arrayIndex, count);
      }

      void ICollection.CopyTo(Array array, int arrayIndex)
      {
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, array, arrayIndex);
      }

      bool ICollection<MessagePackObject>.Contains(
        MessagePackObject item)
      {
        return this._dictionary.ContainsKey(item);
      }

      void ICollection<MessagePackObject>.Add(MessagePackObject item)
      {
        throw new NotSupportedException();
      }

      void ICollection<MessagePackObject>.Clear()
      {
        throw new NotSupportedException();
      }

      bool ICollection<MessagePackObject>.Remove(MessagePackObject item)
      {
        throw new NotSupportedException();
      }

      bool ISet<MessagePackObject>.Add(MessagePackObject item)
      {
        throw new NotSupportedException();
      }

      void ISet<MessagePackObject>.ExceptWith(
        IEnumerable<MessagePackObject> other)
      {
        throw new NotSupportedException();
      }

      void ISet<MessagePackObject>.IntersectWith(
        IEnumerable<MessagePackObject> other)
      {
        throw new NotSupportedException();
      }

      public bool IsProperSubsetOf(IEnumerable<MessagePackObject> other)
      {
        return SetOperation.IsProperSubsetOf<MessagePackObject>((ISet<MessagePackObject>) this, other);
      }

      public bool IsProperSupersetOf(IEnumerable<MessagePackObject> other)
      {
        return SetOperation.IsProperSupersetOf<MessagePackObject>((ISet<MessagePackObject>) this, other);
      }

      public bool IsSubsetOf(IEnumerable<MessagePackObject> other)
      {
        return SetOperation.IsSubsetOf<MessagePackObject>((ISet<MessagePackObject>) this, other);
      }

      public bool IsSupersetOf(IEnumerable<MessagePackObject> other)
      {
        return SetOperation.IsSupersetOf<MessagePackObject>((ISet<MessagePackObject>) this, other);
      }

      public bool Overlaps(IEnumerable<MessagePackObject> other)
      {
        return SetOperation.Overlaps<MessagePackObject>((ISet<MessagePackObject>) this, other);
      }

      public bool SetEquals(IEnumerable<MessagePackObject> other)
      {
        return SetOperation.SetEquals<MessagePackObject>((ISet<MessagePackObject>) this, other);
      }

      void ISet<MessagePackObject>.SymmetricExceptWith(
        IEnumerable<MessagePackObject> other)
      {
        throw new NotSupportedException();
      }

      void ISet<MessagePackObject>.UnionWith(
        IEnumerable<MessagePackObject> other)
      {
        throw new NotSupportedException();
      }

      public MessagePackObjectDictionary.KeySet.Enumerator GetEnumerator()
      {
        return new MessagePackObjectDictionary.KeySet.Enumerator(this._dictionary);
      }

      IEnumerator<MessagePackObject> IEnumerable<MessagePackObject>.GetEnumerator()
      {
        return (IEnumerator<MessagePackObject>) this.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      public struct Enumerator : IEnumerator<MessagePackObject>, IDisposable, IEnumerator
      {
        private MessagePackObjectDictionary.Enumerator _underlying;

        public MessagePackObject Current
        {
          get
          {
            return this._underlying.Current.Key;
          }
        }

        object IEnumerator.Current
        {
          get
          {
            return (object) this._underlying.GetCurrentStrict().Key;
          }
        }

        internal Enumerator(MessagePackObjectDictionary dictionary)
        {
          this._underlying = dictionary.GetEnumerator();
        }

        public void Dispose()
        {
          this._underlying.Dispose();
        }

        public bool MoveNext()
        {
          this._underlying.VerifyVersion();
          return this._underlying.MoveNext();
        }

        void IEnumerator.Reset()
        {
          this._underlying.ResetCore();
        }
      }
    }

    [DebuggerDisplay("Count={Count}")]
    [DebuggerTypeProxy(typeof (CollectionDebuggerProxy<>))]
    [Serializable]
    public sealed class ValueCollection : ICollection<MessagePackObject>, IEnumerable<MessagePackObject>, ICollection, IEnumerable
    {
      private readonly MessagePackObjectDictionary _dictionary;

      public int Count
      {
        get
        {
          return this._dictionary.Count;
        }
      }

      bool ICollection<MessagePackObject>.IsReadOnly
      {
        get
        {
          return true;
        }
      }

      bool ICollection.IsSynchronized
      {
        get
        {
          return false;
        }
      }

      object ICollection.SyncRoot
      {
        get
        {
          return (object) this;
        }
      }

      internal ValueCollection(MessagePackObjectDictionary dictionary)
      {
        this._dictionary = dictionary;
      }

      public void CopyTo(MessagePackObject[] array)
      {
        if (array == null)
          throw new ArgumentNullException(nameof (array));
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, 0, array, 0, this.Count);
      }

      public void CopyTo(MessagePackObject[] array, int arrayIndex)
      {
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, 0, array, arrayIndex, this.Count);
      }

      public void CopyTo(int index, MessagePackObject[] array, int arrayIndex, int count)
      {
        if (array == null)
          throw new ArgumentNullException(nameof (array));
        if (index < 0)
          throw new ArgumentOutOfRangeException(nameof (index));
        if (0 < this.Count && this.Count <= index)
          throw new ArgumentException("Specified array is too small to complete copy operation.", nameof (array));
        if (arrayIndex < 0)
          throw new ArgumentOutOfRangeException(nameof (arrayIndex));
        if (count < 0)
          throw new ArgumentOutOfRangeException(nameof (count));
        if (array.Length - count <= arrayIndex)
          throw new ArgumentException("Specified array is too small to complete copy operation.", nameof (array));
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, index, array, arrayIndex, count);
      }

      void ICollection.CopyTo(Array array, int arrayIndex)
      {
        CollectionOperation.CopyTo<MessagePackObject>((IEnumerable<MessagePackObject>) this, this.Count, array, arrayIndex);
      }

      bool ICollection<MessagePackObject>.Contains(
        MessagePackObject item)
      {
        return this._dictionary.ContainsValue(item);
      }

      void ICollection<MessagePackObject>.Add(MessagePackObject item)
      {
        throw new NotSupportedException();
      }

      void ICollection<MessagePackObject>.Clear()
      {
        throw new NotSupportedException();
      }

      bool ICollection<MessagePackObject>.Remove(MessagePackObject item)
      {
        throw new NotSupportedException();
      }

      public MessagePackObjectDictionary.ValueCollection.Enumerator GetEnumerator()
      {
        return new MessagePackObjectDictionary.ValueCollection.Enumerator(this._dictionary);
      }

      IEnumerator<MessagePackObject> IEnumerable<MessagePackObject>.GetEnumerator()
      {
        return (IEnumerator<MessagePackObject>) this.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      public struct Enumerator : IEnumerator<MessagePackObject>, IDisposable, IEnumerator
      {
        private MessagePackObjectDictionary.Enumerator _underlying;

        public MessagePackObject Current
        {
          get
          {
            return this._underlying.Current.Value;
          }
        }

        object IEnumerator.Current
        {
          get
          {
            return (object) this._underlying.GetCurrentStrict().Value;
          }
        }

        internal Enumerator(MessagePackObjectDictionary dictionary)
        {
          this._underlying = dictionary.GetEnumerator();
        }

        public void Dispose()
        {
          this._underlying.Dispose();
        }

        public bool MoveNext()
        {
          this._underlying.VerifyVersion();
          return this._underlying.MoveNext();
        }

        void IEnumerator.Reset()
        {
          this._underlying.ResetCore();
        }
      }
    }
  }
}
